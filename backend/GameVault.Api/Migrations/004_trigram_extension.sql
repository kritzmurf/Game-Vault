-- 004_add_trigram_extension.sql
-- Enable trigram matching for fuzzy search
CREATE EXTENSION IF NOT EXISTS pg_trgm;
