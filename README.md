# Game Vault

A community-driven game database with user voting. Browse games across platforms, view details, and vote on quality. Built with ASP.NET Core, React, and PostgreSQL.

The initial dataset focuses on PlayStation 1 titles released in North America, with support for additional platforms over time.

## Prerequisites

- [.NET SDK 10+](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Docker](https://docs.docker.com/get-docker/)

## Quick Start

### 1. Environment setup

```bash
cp .env.example .env
# Edit .env with your preferred passwords
```

### 2. Start the database

```bash
docker compose up -d
```

- **PostgreSQL** — `localhost:5432`
- **pgAdmin** — `http://localhost:5050`

### 3. Run the backend

```bash
dotnet run --project backend/GameVault.Api
```

- **API** — `http://localhost:5000`
- **Test** — `curl http://localhost:5000/api/hello`

### 4. Run the frontend

```bash
cd frontend/game-vault
npm install
npm run dev
```

- **App** — `http://localhost:5173`

## Project Structure

```
Game-Vault/
├── backend/GameVault.Api/   # ASP.NET Core Web API
├── frontend/game-vault/     # React + TypeScript (Vite)
├── scripts/                 # DB seed scripts (future)
└── docker-compose.yml       # PostgreSQL + pgAdmin
```
