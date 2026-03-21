-- 005_add_game_category_and_igdb_ids.sql
-- Add IGDB category for filtering (0=main_game, 1=dlc, 2=expansion,...)
-- Add IGDB identifiers for future parent/child game linking
-- See: https://api-docs.igdb.com/#game

ALTER TABLE games
    ADD COLUMN category             SMALLINT NOT NULL DEFAULT 0,
    ADD COLUMN igdb_id              INTEGER,
    ADD COLUMN parent_game_igdb_id  INTEGER;

CREATE INDEX idx_games_category ON games (category);
