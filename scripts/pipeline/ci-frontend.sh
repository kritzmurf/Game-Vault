#!/bin/bash

# Frontend Build Script intended for use by the CI pipeline
set -e

PROJECT_ROOT=$(dirname "$(readlink -f "$0")")/../..
FRONTEND="$PROJECT_ROOT/frontend/game-vault"

cd "$FRONTEND"

echo "=== Installing Frontend Dependencies ==="
npm ci

echo "=== Type Checking Frontend ==="
npx tsc --noEmit

echo "=== Linting Frontend ==="
npm run lint
