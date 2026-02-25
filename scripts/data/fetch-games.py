#!/usr/bin/env python3
"""Fetch game data from Twitch IGDB and export to JSON"""


"""Data Layout Example:

  Game: Crash Bandicoot (id: 123)
  involved_companies: [456, 789]

  Query /involved_companies for IDs 456 and 789:

  { id: 456, game: 123, company: 101, publisher: true,  developer: false }
  { id: 789, game: 123, company: 202, publisher: false, developer: true  }

  Query /companies for IDs 101 and 202:

  { id: 101, name: "Sony Computer Entertainment" }
  { id: 202, name: "Naughty Dog" }
"""

""" Big Idea

    1)  Fetch all games for a platform in batches so as not to overload Twitch's 
        data limits.

    2)  Fetch all companies involved in each game.

    3) Map fetched games to the games we have fetched.
"""
# external
import json
import os
import sys
import time
import requests
from datetime import datetime, timezone
from dotenv import load_dotenv

# internal
from igdb_platforms import IgdbPlatforms

# load env vars
load_dotenv(os.path.join(os.path.dirname(__file__), "..","..",".env"))

# Constants
TWITCH_AUTH_URL = "https://id.twitch.tv/oauth2/token"
IGDB_GAMES_URL = "https://api.igdb.com/v4/games"
IGDB_COVERS_URL = "https://api.igdb.com/v4/covers"
IGDB_COMPANIES_URL = "https://api.igdb.com/v4/companies"
IGDB_INVOLVED_COMPANIES_URL = "https://api.igdb.com/v4/involved_companies"

BATCH_SIZE = 500
REQUEST_TIMEOUT = 30
RATE_LIMIT_BUFFER = 0.25

# Platforms we are currently fetching
PLATFORMS = {
    IgdbPlatforms.PS1,
}

''' (For when we want to go 'full send')
PLATFORMS = set(IgdbPlatforms)
'''

def authenticate(client_id, client_secret):
    """Get Oauth token from Twitch"""
    response = requests.post(TWITCH_AUTH_URL, params = {
        "client_id": client_id,
        "client_secret": client_secret,
        "grant_type": "client_credentials"
    }, timeout=REQUEST_TIMEOUT)
    response.raise_for_status()
    return response.json()["access_token"]

def fetch_games(headers, platform):
    """Fetch all games for a platform from IGDB in batches."""
    all_games = []
    offset = 0

    while True:
        body = (
            f"fields name, summary, first_release_date, cover, involved_companies;\n"
            f"where platforms = ({platform.value});\n"
            f"limit {BATCH_SIZE};\n"
            f"offset {offset};\n"
        )
        #   entries are so massive that IGDB uses 'POST' the way you'd expect them to use 'GET'. The
        #   reasoning is because the body could become enormous, and GET requests put everything in the URL.
        #   We run the risk, depending on query, of overflowing the URL length limit, so Twitch uses Post instead.
        response = requests.post(
            IGDB_GAMES_URL, headers=headers, data=body, timeout=REQUEST_TIMEOUT
        )
        response.raise_for_status()
        batch = response.json()

        if not batch:
            break #no more data to process

        all_games.extend(batch)
        print(f"  Fetch {len(all_games)} games...")
        offset += BATCH_SIZE
        time.sleep(RATE_LIMIT_BUFFER) #Lets not mess with Twitch's rate limiting and pace ourselves

    return all_games

def fetch_cover_art(headers, cover_ids):
    """Fetch cover art"""
    covers = {}
    for i in range(0, len(cover_ids), BATCH_SIZE):
        batch_ids = cover_ids[i:i + BATCH_SIZE]
        ids_str = ",".join(str(cid) for cid in batch_ids)
        body = (
            f"fields game, url;\n"
            f"where id = ({ids_str});\n"
            f"limit {BATCH_SIZE};\n"
        )

        response = requests.post(
            IGDB_COVERS_URL, headers=headers, data=body, timeout=REQUEST_TIMEOUT
        )
        response.raise_for_status()

        for cover in response.json():
            url = cover.get("url", "")
            if url.startswith("//"): # support protocol relative format
                url = "https:" + url
            url = url.replace("t_thumb", "t_cover_big")
            covers[cover["game"]] = url

        time.sleep(RATE_LIMIT_BUFFER)

    return covers

def fetch_companies(headers, involved_company_ids):
    """ Fetch involved companies and their relevant data
        See: Data Layout for details
    """
    # Get companies
    involved = []
    for i in range(0, len(involved_company_ids), BATCH_SIZE):
        batch_ids = involved_company_ids[i:i + BATCH_SIZE]
        # create comma delimited list of company ids
        ids_str = ",".join(str(cid) for cid in batch_ids)
        body = (
            f"fields game, company, publisher, developer;\n"
            f"where id = ({ids_str}); limit {BATCH_SIZE};\n"
        )
        response = requests.post(
            IGDB_INVOLVED_COMPANIES_URL, headers=headers, data=body, timeout=REQUEST_TIMEOUT
        )
        response.raise_for_status()
        involved.extend(response.json())
        time.sleep(RATE_LIMIT_BUFFER)

    # get company names
    #NOTE: set: remove duplicate entries
    company_ids = ( list(set(involved_company["company"]
                    for involved_company in involved
                    if "company" in involved_company)))
    company_names = {}
    for i in range(0, len(company_ids), BATCH_SIZE):
        batch_ids = company_ids[i:i + BATCH_SIZE]
        ids_str = ",".join(str(company_id) for company_id in batch_ids)
        body = (
            f"fields name;\n"
            f"where id = ({ids_str});\n"
            f"limit {BATCH_SIZE};\n"
        )

        response = requests.post(
            IGDB_COMPANIES_URL, headers=headers, data=body, timeout=REQUEST_TIMEOUT
        )
        response.raise_for_status()
        for company in response.json():
            company_names[company["id"]] = company["name"]
        time.sleep(RATE_LIMIT_BUFFER)

    # build lookup map for each game
    publishers = {}
    developers = {}
    for involved_company in involved:
        game_id = involved_company.get("game")
        company_id = involved_company.get("company")
        name = company_names.get(company_id)
        if not name:
            continue
        if involved_company.get("publisher"):
            publishers[game_id] = name
        if involved_company.get("developer"):
            developers[game_id] = name
    
    return publishers, developers

def main():
    """Fetch game data from IGDB and export it as JSON."""
    client_id = os.environ.get("TWITCH_CLIENT_ID")
    client_secret = os.environ.get("TWITCH_CLIENT_SECRET")

    if not client_id or not client_secret:
        print("ERROR: TWITCH_CLIENT_ID and TWITCH_CLIENT_SECRET must be set.")
        sys.exit(1)

    print("Authenticating with Twitch...")
    token = authenticate(client_id, client_secret)
    headers = { #OAuth 2.0 format
        "Client-ID": client_id,
        "Authorization": f"Bearer {token}",
    }

    for platform in PLATFORMS:
        print(f"\nFetching  {platform.name} games...")
        games = fetch_games(headers, platform)
        print(f"Total: {len(games)} games")

        #Get IDs 
        cover_ids = [game["cover"] for game in games if "cover" in game]
        involved_ids = []
        for game in games:
            involved_ids.extend(game.get("involved_companies", []))

        print("Fetching cover art...")
        covers = fetch_cover_art(headers, cover_ids)

        print("Fetching publishers and developers...")
        publishers, developers = fetch_companies(headers, list(set(involved_ids)))

        # Form data into data set format
        print("Building dataset...")
        dataset = []
        for game in games:
            release_date = None
            if "first_release_date" in game:
                dt = datetime.fromtimestamp(game["first_release_date"], tz=timezone.utc)
                release_date = dt.strftime("%Y-%m-%d")

            dataset.append({
                "title": game.get("name", "Unknown"),
                "platform": platform.name,
                "releaseDate": release_date,
                "publisher": publishers.get(game["id"]),
                "developer": developers.get(game["id"]),
                "description": game.get("summary"),
                "coverArtUrl": covers.get(game["id"]),
                "region": "NA",
                })

        dataset.sort(key=lambda x: x["title"])

        #Write to file
        output_dir = os.path.join(os.path.dirname(__file__), "output")
        os.makedirs(output_dir, exist_ok=True)
        filename = f"{platform.name.lower()}-games.json"
        output_path = os.path.join(output_dir, filename)
        with open(output_path, "w", encoding="utf-8") as file:
            json.dump(dataset, file, indent=2)

        print(f"Wrote {len(dataset)} games to {output_path}")


if __name__ == "__main__":
    main()


