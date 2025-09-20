#!/usr/bin/env bash
set -euo pipefail

# Resolve repo root relative to this script
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"

# Load local env overrides if present
if [[ -f "$REPO_ROOT/.env" ]]; then
  set -a
  # shellcheck source=/dev/null
  source "$REPO_ROOT/.env"
  set +a
fi

log() {
  echo "[scripts] $*"
}

ensure_dotnet() {
  if ! command -v dotnet >/dev/null 2>&1; then
    log "dotnet SDK not found; please install .NET 8 before running this script" >&2
    exit 1
  fi
}

ensure_docker() {
  if ! command -v docker >/dev/null 2>&1; then
    log "docker CLI not found; please install Docker before running this script" >&2
    exit 1
  fi
}
