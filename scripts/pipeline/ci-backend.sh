#!/bin/bash

# Backend Build Script intended for use by the CI pipeline
set -e 

PROJECT_ROOT=$(dirname "$(readlink -f "$0")")/../..
BACKEND="$PROJECT_ROOT/backend/GameVault.Api"

echo "=== Restoring Backend Dependencies ==="
dotnet restore $BACKEND

echo "=== Building Backend ==="
dotnet build $BACKEND --no-restore

echo "=== Running tests ==="
dotnet test $BACKEND --no-build
