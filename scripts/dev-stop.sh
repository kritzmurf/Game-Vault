#!/bin/bash
set -e

PROJECT_ROOT=$(dirname "$(readlink -f "$0")")/..

echo "=== Stopping Game Vault Dev Environment ==="

# Kill any running dotnet or vite dev processes for this project
BACKEND_PIDS=$(pgrep -f "GameVault.Api" 2>/dev/null || true)
if [ -n "$BACKEND_PIDS" ]; then
    echo "Stopping backend (PID: $BACKEND_PIDS)..."
    kill $BACKEND_PIDS 2>/dev/null || true
fi

FRONTEND_PIDS=$(pgrep -f "vite.*game-vault" 2>/dev/null || true)
if [ -n "$FRONTEND_PIDS" ]; then
    echo "Stopping frontend (PID: $FRONTEND_PIDS)..."
    kill $FRONTEND_PIDS 2>/dev/null || true
fi

# Stop Docker containers
echo "Stopping Docker containers..."
docker compose -f "$PROJECT_ROOT/docker-compose.yml" down

echo "=== All services stopped ==="
