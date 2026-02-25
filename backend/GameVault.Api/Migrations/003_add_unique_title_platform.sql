-- 003_add_unique_title_platform.sql
-- add unique contraint on title + platform.
-- Required for seeding the database in seed-database.py
ALTER TABLE games
    ADD CONSTRAINT uq_games_title_platform UNIQUE (title, platform);
