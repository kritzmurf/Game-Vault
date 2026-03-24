using Npgsql;

namespace GameVault.Api.Tests.Helpers;

public static class TestDataHelper
{
    public static void TruncateGames(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "TRUNCATE TABLE games RESTART IDENTITY CASCADE";
        cmd.ExecuteNonQuery();
    }

    public static void SeedStandardGames(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        var games = new[]
        {
            // PS1 - main games (category 0)
            ("Final Fantasy VII", "PS1", "1997-01-31", "Square", "Square", "An RPG about Cloud Strife", 0, 1001, (int?)null),
            ("Final Fantasy VIII", "PS1", "1999-02-11", "Square", "Square", "An RPG about Squall", 0, 1002, (int?)null),
            ("Final Fantasy Tactics", "PS1", "1997-06-20", "Square", "Square", "A tactical RPG", 0, 1003, (int?)null),
            ("Crash Bandicoot", "PS1", "1996-09-09", "Sony", "Naughty Dog", "A platformer", 0, 1004, (int?)null),
            ("Metal Gear Solid", "PS1", "1998-09-03", "Konami", "Konami", "A stealth game", 0, 1005, (int?)null),
            ("Xenogears", "PS1", "1998-02-11", "Square", "Square", "A mecha RPG", 0, 1006, (int?)null),
            ("Resident Evil 2", "PS1", "1998-01-21", "Capcom", "Capcom", "Survival horror", 0, 1007, (int?)null),
            ("Castlevania SotN", "PS1", "1997-03-20", "Konami", "Konami", "Action RPG", 0, 1008, (int?)null),
            ("Spyro the Dragon", "PS1", "1998-09-09", "Sony", "Insomniac", "A platformer", 0, 1009, (int?)null),
            ("Tekken 3", "PS1", "1998-03-26", "Namco", "Namco", "Fighting game", 0, 1010, (int?)null),
            // PS2 - main games
            ("Final Fantasy X", "PS2", "2001-07-19", "Square", "Square", "An RPG about Tidus", 0, 2001, (int?)null),
            ("Metal Gear Solid 3", "PS2", "2004-11-17", "Konami", "Konami", "A stealth game", 0, 2002, (int?)null),
            ("Kingdom Hearts", "PS2", "2002-03-28", "Square", "Square", "Action RPG", 0, 2003, (int?)null),
            ("Shadow of the Colossus", "PS2", "2005-10-18", "Sony", "Team Ico", "Action adventure", 0, 2004, (int?)null),
            ("God of War", "PS2", "2005-03-22", "Sony", "Santa Monica", "Action game", 0, 2005, (int?)null),
            // SNES - main games
            ("Chrono Trigger", "SNES", "1995-03-11", "Square", "Square", "A time-travel RPG", 0, 3001, (int?)null),
            ("Super Mario World", "SNES", "1990-11-21", "Nintendo", "Nintendo", "A platformer", 0, 3002, (int?)null),
            ("A Link to the Past", "SNES", "1991-11-21", "Nintendo", "Nintendo", "Action adventure", 0, 3003, (int?)null),
            ("Super Metroid", "SNES", "1994-03-19", "Nintendo", "Nintendo R&D1", "Action adventure", 0, 3004, (int?)null),
            ("Earthbound", "SNES", "1994-08-27", "Nintendo", "Ape/HAL", "An RPG", 0, 3005, (int?)null),
            ("Secret of Mana", "SNES", "1993-08-06", "Square", "Square", "Action RPG", 0, 3006, (int?)null),
            ("Mega Man X", "SNES", "1993-12-17", "Capcom", "Capcom", "Action platformer", 0, 3007, (int?)null),
            ("Donkey Kong Country", "SNES", "1994-11-21", "Nintendo", "Rare", "A platformer", 0, 3008, (int?)null),
            ("Star Fox", "SNES", "1993-02-21", "Nintendo", "Nintendo/Argonaut", "Rail shooter", 0, 3009, (int?)null),
            ("F-Zero", "SNES", "1990-11-21", "Nintendo", "Nintendo", "Racing game", 0, 3010, (int?)null),
            // Non-main-game entries (should be filtered out by category)
            ("FF VII: Advent Children DLC", "PS1", "2000-01-01", "Square", "Square", "DLC content", 1, 9001, (int?)1001),
            ("FFX International Expansion", "PS2", "2002-01-01", "Square", "Square", "Expansion", 2, 9002, (int?)2001),
        };

        foreach (var g in games)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO games (title, platform, release_date, publisher, developer, description,
                                   cover_art_url, region, category, igdb_id, parent_game_igdb_id)
                VALUES (@title, @platform, @releaseDate::date, @publisher, @developer, @description,
                        NULL, 'NA', @category, @igdbId, @parentGameIgdbId)";
            cmd.Parameters.AddWithValue("title", g.Item1);
            cmd.Parameters.AddWithValue("platform", g.Item2);
            cmd.Parameters.AddWithValue("releaseDate", g.Item3);
            cmd.Parameters.AddWithValue("publisher", g.Item4);
            cmd.Parameters.AddWithValue("developer", g.Item5);
            cmd.Parameters.AddWithValue("description", g.Item6);
            cmd.Parameters.AddWithValue("category", g.Item7);
            cmd.Parameters.AddWithValue("igdbId", g.Item8);
            cmd.Parameters.AddWithValue("parentGameIgdbId", (object?)g.Item9 ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }
    }
}
