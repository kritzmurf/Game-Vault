-- 002_create_games.sql
-- Create a new table named 'games'. 
-- Model defined at Models/Game.cs
CREATE TABLE games (
    id              SERIAL PRIMARY KEY,
    title           VARCHAR(255) NOT NULL,
    platform        VARCHAR(50) NOT NULL,
    release_date    DATE,
    publisher       VARCHAR(255),
    developer       VARCHAR(255),
    description     TEXT,
    cover_art_url   TEXT,
    region          VARCHAR(10) NOT NULL
);
