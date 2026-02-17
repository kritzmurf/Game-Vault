# Game Vault — Design Document

## Overview

Game Vault is a community-driven game database where users can browse games across platforms, view details, and vote on quality. The initial dataset targets PlayStation 1 titles released in North America, with support for additional platforms over time.

## Architecture

### Stack

| Layer | Technology | Purpose |
|-------|-----------|---------|
| Frontend | React + TypeScript (Vite) | Single-page application |
| Backend | ASP.NET Core (.NET 10) | REST API |
| Database | PostgreSQL 17 | Persistent storage |
| Infrastructure | Docker Compose | Local dev environment |
| CI/CD | GitHub Actions | CI, staging, release pipelines |

### Communication

The frontend and backend are separate processes. In development, Vite proxies `/api/*` requests to the backend. In production, both will be served behind a reverse proxy or from a single container.

### Project Structure

```
Game-Vault/
├── backend/GameVault.Api/
│   ├── Controllers/          — API endpoint controllers
│   ├── Models/               — Entity and DTO classes (future)
│   ├── Services/             — Business logic (future)
│   ├── Data/                 — DbContext, migrations (future)
│   └── Program.cs            — Application entry point
├── frontend/game-vault/
│   └── src/                  — React application source
├── scripts/
│   ├── pipeline/             — CI/CD build scripts
│   └── data/                 — Seed data files (future)
├── docs/                     — Project documentation
└── docker-compose.yml        — PostgreSQL + pgAdmin
```

Controllers follow a flat structure under `GameVault.Controllers` namespace. All controllers serve API routes — no MVC views.

## Data Model

### Game

Represents a single game entry in the database.

| Field | Type | Notes |
|-------|------|-------|
| Id | int | Primary key, auto-increment |
| Title | string | Game title |
| Platform | string | e.g. "PS1" — supports expansion to other platforms |
| ReleaseDate | DateOnly? | North America release date, nullable for unknown dates |
| Publisher | string? | Optional |
| Developer | string? | Optional |
| Description | string? | Game summary/description |
| CoverArtUrl | string? | URL to cover art image |
| Region | string | e.g. "NA" — initial focus is North America |

### User

Represents an authenticated user. No local passwords — authentication is handled entirely through OAuth providers.

| Field | Type | Notes |
|-------|------|-------|
| Id | int | Primary key, auto-increment |
| DisplayName | string | Shown publicly |
| Provider | string | e.g. "google", "github", "discord" |
| ProviderId | string | Unique ID from the OAuth provider |
| CreatedAt | DateTime | Account creation timestamp |

Unique constraint on (Provider, ProviderId) — one account per provider identity.

### Vote

Represents a user's rating of a game. Metacritic-style scoring on a 0.0–10.0 scale with one decimal point of granularity (e.g. 7.3, 9.1).

| Field | Type | Notes |
|-------|------|-------|
| Id | int | Primary key, auto-increment |
| UserId | int | Foreign key to User |
| GameId | int | Foreign key to Game |
| Score | decimal(3,1) | 0.0 to 10.0, one decimal place |
| CreatedAt | DateTime | When the vote was cast |

Unique constraint on (UserId, GameId) — one vote per user per game. The game's displayed score is the average of all user scores.

## Authentication

OAuth-based. Users sign in through an external provider (Google, GitHub, Discord are candidates). The backend handles the OAuth callback flow, creates or retrieves the user record, and issues a session or JWT token for subsequent API requests.

No local account registration, no password management, no email verification.

### Provider Selection

Launch provider:

- **Google** — broadest user base, first priority

Post-launch providers (added incrementally):

- **GitHub** — natural fit for a developer-oriented portfolio project
- **Discord** — common in gaming communities

## API Design

All endpoints are prefixed with `/api/`.

### Games

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/games` | List games (paginated, filterable by platform) |
| GET | `/api/games/{id}` | Get game details |
| GET | `/api/games/search?q=` | Search games by title |

### Auth

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/auth/login/{provider}` | Initiate OAuth flow |
| GET | `/api/auth/callback` | OAuth callback handler |
| GET | `/api/auth/me` | Get current user info |
| POST | `/api/auth/logout` | End session |

### Votes

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/games/{id}/vote` | Cast or update a vote (authenticated) |
| DELETE | `/api/games/{id}/vote` | Remove a vote (authenticated) |

## Data Seeding

The initial PS1 game dataset will be stored as a static JSON file in `scripts/data/`. A seed script reads this file and performs upserts into the database.

The seed script is idempotent — safe to run multiple times. It matches existing records by title + platform + region and inserts or updates accordingly.

Primary data source: **IGDB (Internet Game Database)**. Twitch-owned, comprehensive coverage of PS1 titles with structured metadata (titles, release dates, publishers, developers, cover art, descriptions). Requires a free Twitch developer account for API access. Data is fetched once via a script, exported to a static JSON file at `scripts/data/ps1-games.json`, and committed to the repo. The same approach extends to additional platforms post-launch.

## Deployment

Release builds produce a Docker image pushed to GitHub Container Registry (`ghcr.io/kritzmurf/game-vault`). The image contains the compiled backend serving the built frontend as static files.

Hosting target: **AWS App Runner**. Pulls the Docker image from ghcr.io, handles deployment, scaling, TLS, and load balancing. Estimated cost ~$5–10/month for low traffic. The production database will use **AWS RDS for PostgreSQL** to match the local dev stack.

## Open Questions

None currently — all major decisions resolved.
