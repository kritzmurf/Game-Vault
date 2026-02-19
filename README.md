# Game Vault

A community-driven game database with user voting. Browse games across platforms, view details, and vote on quality. Built with ASP.NET Core, React, and PostgreSQL.

The initial dataset focuses on PlayStation 1 titles released in North America, with support for additional platforms over time.

## Prerequisites

- [.NET SDK 10+](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Docker](https://docs.docker.com/get-docker/)

## Quick Start

1. Copy `.env.example` to `.env` and fill in your local passwords
2. Start everything:
   ```bash
   ./scripts/dev-start.sh
   ```
3. Services will be available at:
   - **Frontend:** http://localhost:5173
   - **Backend API:** http://localhost:5000
   - **pgAdmin:** http://localhost:5050

Database migrations run automatically on backend startup.

## Stopping

```bash
# Ctrl+C stops the backend and frontend
# Then tear down Docker containers:
./scripts/dev-stop.sh
```

## Project Structure

```
Game-Vault/
├── backend/GameVault.Api/   # ASP.NET Core Web API
├── frontend/game-vault/     # React + TypeScript (Vite)
├── scripts/                 # Dev scripts and seed data
├── docs/                    # Design and roadmap documentation
└── docker-compose.yml       # PostgreSQL + pgAdmin
```

## Documentation

- [Design Document](docs/DESIGN.md) — architecture, data model, API design
- [Roadmap](docs/ROADMAP.md) — epics and task tracking
