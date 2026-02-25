#!/bin/bash
set -e

SCRIPT_DIR=$(dirname "$(readlink -f "$0")")

python -m venv "$SCRIPT_DIR/.venv"
source "$SCRIPT_DIR/.venv/bin/activate"
pip install -r "$SCRIPT_DIR/requirements.txt"

echo "Venv created at script/data/.venv"
echo "Run 'source scripts/data/.venv/bin/activate' to activate it"
