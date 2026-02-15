#!/bin/bash
set -e

PROJECT_ROOT=$(dirname "$(readlink -f "$0")")/..

echo "=== Game Vault Dev Environment ==="
echo ""

# Start database
echo "[1/3] Starting PostgreSQL and pgAdmin..."
docker compose -f "$PROJECT_ROOT/docker-compose.yml" up -d --wait
echo "      PostgreSQL: localhost:${DB_PORT:-5432}"
echo "      pgAdmin:    http://localhost:${PGADMIN_PORT:-5050}"
echo ""

# Start backend
echo "[2/3] Starting backend (ASP.NET Core)..."
dotnet run --project "$PROJECT_ROOT/backend/GameVault.Api" &
BACKEND_PID=$!
echo "      API: http://localhost:5000"
echo "      PID: $BACKEND_PID"
echo ""

# Start frontend
echo "[3/3] Starting frontend (Vite + React)..."
cd "$PROJECT_ROOT/frontend/game-vault"
npm run dev &
FRONTEND_PID=$!
echo "      App: http://localhost:5173"
echo "      PID: $FRONTEND_PID"
echo ""

echo "=== All services running ==="
echo "Press Ctrl+C to stop backend and frontend."
echo "Run scripts/dev-stop.sh to also tear down Docker containers."
echo ""

# Wait for either process to exit, then clean up both
cleanup() {
    echo ""
    echo "Shutting down..."
    kill $BACKEND_PID 2>/dev/null
    kill $FRONTEND_PID 2>/dev/null
    wait $BACKEND_PID 2>/dev/null
    wait $FRONTEND_PID 2>/dev/null
    echo "Backend and frontend stopped. Docker containers still running."
    echo "Run scripts/dev-stop.sh to stop everything."
}

trap cleanup SIGINT SIGTERM
wait
