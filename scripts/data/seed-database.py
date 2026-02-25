#!/usr/bin/env python3
"""Read JSON game data files and seed the PostgreSQL database with them"""
# external
import json
import glob
import os
import sys
import psycopg2
from dotenv import load_dotenv

# load env vars
load_dotenv(os.path.join(os.path.dirname(__file__), "..", "..",".env"))

def parse_connection_string(connection_string):
    """ ConnectionString__DefaultConnection is formatted for Npgsql in the C# environment,
        becuase thats what will be doing the heavy lifting. rather than have a duplicate
        entry, we modify the Npgsql formatting here so that the same variable serves both tools

        Npgsql format:  Host=<HOST>;Port=<PORT>;Database=<DATABASE>;Username=<USERNAME>;Password=<PASSWORD>
        psycopg2 format:host=<HOST> port=<PORT> dbname=<DATABASE> user=<USER> password=<PASSWORD>
        """
    keyword_map = {
        "Host": "host",
        "Port": "port",
        "Database": "dbname",
        "Username": "user",
        "Password": "password",
    }
    substrings = {}
    for substring in connection_string.split(";"):
        if "=" in substring:
            raw_key, value = substring.split("=", 1)
            key = keyword_map.get(raw_key.strip())
            if key:
                substrings[key] = value.strip()
    return " ".join(f"{key}={value}" for key, value in substrings.items())
            

def main():
    """Seed database"""
    connection_string = os.environ.get("ConnectionStrings__DefaultConnection")
    if not connection_string:
        print("ERROR: ConnectionString__DefaultConnection not set in .env file. Aborting")
        sys.exit(1)
    
    output_dir = os.path.join(os.path.dirname(__file__), "output")
    json_files = glob.glob(os.path.join(output_dir, "*-games.json"))

    if not json_files:
        print("No game data found in output directory. Run fetch-games.py to seed directory. Aborting")
        sys.exit(1)

    print(f"Found {len(json_files)} platform files. Seeding...")

    pg_connection_string = parse_connection_string(connection_string)
    connection = psycopg2.connect(pg_connection_string)
    cursor = connection.cursor()

    total_processed = 0
    for json_file in json_files:
        with open(json_file, "r", encoding="utf-8") as file:
            games = json.load(file)

        platform = os.path.basename(json_file).replace("-games.json", "")
        print(f" Seeding {platform}: {len(games)} games...")

        for game in games:
            cursor.execute(
                """ INSERT INTO games 
                        (title, platform, release_date, publisher, developer, description, cover_art_url, region)
                    VALUES 
                        (%s, %s, %s, %s, %s, %s, %s, %s)
                    ON CONFLICT (title, platform) DO UPDATE SET
                        release_date = EXCLUDED.release_date,
                        publisher = EXCLUDED.publisher,
                        developer = EXCLUDED.developer,
                        description = EXCLUDED.description,
                        cover_art_url = EXCLUDED.cover_art_url,
                        region = EXCLUDED.region
                """,
                (game.get("title"), game.get("platform"), game.get("releaseDate"),
                 game.get("publisher"), game.get("developer"), game.get("description"),
                 game.get("coverArtUrl"), game.get("region")
                )
            )
        total_processed += len(games)

    connection.commit()
    cursor.close()
    connection.close()
    print(f"\nDone. Processed {total_processed} games.")

if __name__ == "__main__":
    main()
