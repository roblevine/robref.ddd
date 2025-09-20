#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=scripts/common.sh
source "$SCRIPT_DIR/common.sh"

usage() {
  cat <<USAGE
Usage: $(basename "$0") [--logs]

Start the SQL Server container defined in docker-compose.yml.

Options:
  --logs        Tail container logs after startup
  -h, --help    Show this help
USAGE
}

FOLLOW_LOGS=0
while [[ $# -gt 0 ]]; do
  case "$1" in
    --logs)
      FOLLOW_LOGS=1
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    *)
      log "Unknown option: $1" >&2
      usage
      exit 1
      ;;
  esac
  shift
done

ensure_docker

log "docker compose up -d sqlserver"
docker compose -f "$REPO_ROOT/docker-compose.yml" up -d sqlserver

if [[ $FOLLOW_LOGS -eq 1 ]]; then
  log "docker compose logs -f sqlserver"
  docker compose -f "$REPO_ROOT/docker-compose.yml" logs -f sqlserver
fi
