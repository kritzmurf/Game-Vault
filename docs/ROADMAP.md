# Game Vault — Roadmap

## How to use this file

- Check off tasks as they are completed: `- [ ]` → `- [x]`
- Add new epics by copying the epic template at the bottom of this file
- Add tasks under any epic as needed
- Tasks marked with `[BLOCKED]` have dependencies noted in their description

---

## Epic 1: Database Foundation

Connect the backend to PostgreSQL via Entity Framework Core.

- [ ] Add EF Core NuGet packages (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- [ ] Create `DbContext` class
- [ ] Configure connection string in `appsettings.Development.json`
- [ ] Wire up DbContext in `Program.cs` via dependency injection
- [ ] Run initial EF migration to verify connectivity
- [ ] Document database setup in README if needed

## Epic 2: Game Data & Seeding

Define the game schema, build API endpoints, and populate with PS1 data.

- [ ] Design `Game` model (title, platform, release date, region, description, cover art URL, etc.)
- [ ] Create EF migration for the games table
- [ ] Build `GamesController` with read endpoints (`GET /api/games`, `GET /api/games/{id}`)
- [ ] Register a Twitch developer application (client type: Confidential)
- [ ] Store Twitch Client ID and Client Secret securely (not in repo)
- [ ] Write IGDB data fetch script (authenticate, query PS1 games in batches, resolve covers/companies)
- [ ] Export PS1 North America dataset to `scripts/data/ps1-games.json`
- [ ] Write seed script to upsert JSON data into the database
- [ ] Add pagination support to the game list endpoint

## Epic 3: Frontend — Browse & View

Build the UI for browsing and viewing game details.

- [ ] Set up React Router for page navigation
- [ ] Build home page with platform navigation (initially just "PlayStation")
- [ ] Build console/platform page showing available game list for that platform
- [ ] Implement pagination or infinite scroll on the game list
- [ ] Build individual game detail page
- [ ] Add search bar (global, accessible from any page)
- [ ] Build search results page (displays matching games or "no results found")
- [ ] Build search API endpoint (`GET /api/games/search?q=`) `[BLOCKED]` Requires Epic 2
- [ ] Add loading and error states across all pages
- [ ] Basic responsive layout and styling

## Epic 4: User System

OAuth-based authentication. No local account management — users sign in via external providers.

- [ ] Add ASP.NET Core authentication packages
- [ ] Configure OAuth provider(s) with client ID/secret
- [ ] Design `User` model (ID, display name, provider, provider ID, created date) and create migration
- [ ] Implement OAuth callback and session/token handling on the backend
- [ ] Build sign-in UI on the frontend
- [ ] Protect relevant API routes (voting, etc.)

## Epic 5: Voting System

Allow authenticated users to vote on games.

- [ ] Design `Vote` model (score: 0.0–10.0, one decimal granularity) and create migration
- [ ] Build vote endpoints (`POST /api/games/{id}/vote`, `DELETE /api/games/{id}/vote`)
- [ ] Enforce one-vote-per-user-per-game logic
- [ ] Implement average score calculation for game display
- [ ] Display vote results on game detail and list pages
- [ ] `[BLOCKED]` Requires Epic 2 (game data) and Epic 4 (user system)

## Epic 6: Pipeline & Infrastructure

CI/CD, deployment, and operational concerns.

- [x] Set up GitHub Actions CI workflow (PR to develop)
- [x] Set up GitHub Actions Staging workflow (PR to main)
- [x] Set up GitHub Actions Release workflow (push to main)
- [x] Configure `RELEASE_PAT` secret on GitHub
- [x] Add branch protection rules
- [ ] Write Dockerfile for production build
- [ ] Implement real release steps (Docker image push to ghcr.io, GitHub Release creation)
- [ ] Set up AWS App Runner service pointing to ghcr.io image
- [ ] Set up AWS RDS PostgreSQL instance for production
- [ ] Configure production environment variables and secrets in App Runner
- [ ] Add `.vite/` to `.gitignore`

## Epic 7: Multi-Platform Expansion

Post-launch. Extend Game Vault beyond PS1 to support additional consoles and platforms.

- [ ] Add platform selection/filtering to the frontend
- [ ] Fetch and export data for additional platforms from IGDB
- [ ] Run seed script for new platform datasets
- [ ] Update game list and detail pages to display platform context
- [ ] Consider platform-specific metadata (e.g. generation, manufacturer)

---

## Epic Template

Copy this section to add a new epic.

```
## Epic N: Title

Brief description of the epic's goal.

- [ ] Task description
- [ ] Task description
- [ ] `[BLOCKED]` Task with dependency — requires Epic X
```
