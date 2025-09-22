#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=scripts/common.sh
source "$SCRIPT_DIR/common.sh"

# Keep this script aligned with the solution layout; update if new test assemblies
# are introduced outside the main solution or require custom logic.

usage() {
  cat <<USAGE
Usage: $(basename "$0") [OPTIONS] [dotnet test options]

Run the test suite with optional database provider filtering.

Options:
  --docker-compose     Run only Docker Compose SQL Server tests
  --testcontainers     Run only Testcontainers SQL Server tests
  --in-memory          Run only in-memory tests
  -h, --help           Show this help

Examples:
  $(basename "$0")                           # Run all tests
  $(basename "$0") --docker-compose         # Run Docker Compose tests only
  $(basename "$0") --testcontainers          # Run Testcontainers tests only
  $(basename "$0") --in-memory               # Run in-memory tests only
  $(basename "$0") --no-build                # Run all tests without building
  $(basename "$0") --docker-compose --watch # Watch Docker Compose tests
USAGE
}

DOTNET_ARGS=()
DATABASE_FILTER=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    -h|--help)
      usage
      exit 0
      ;;
    --docker-compose)
      DATABASE_FILTER="--filter Database=DockerCompose"
      ;;
    --testcontainers)
      DATABASE_FILTER="--filter Database=Testcontainers"
      ;;
    --in-memory)
      DATABASE_FILTER="--filter FullyQualifiedName~InMemory"
      ;;
    *)
      DOTNET_ARGS+=("$1")
      ;;
  esac
  shift
done

ensure_dotnet

# Build command with optional database filter
CMD_ARGS=("$REPO_ROOT/RobRef.DDD.sln")
if [[ -n "$DATABASE_FILTER" ]]; then
  # shellcheck disable=SC2206
  CMD_ARGS+=($DATABASE_FILTER)
fi
if [[ ${#DOTNET_ARGS[@]} -gt 0 ]]; then
  CMD_ARGS+=("${DOTNET_ARGS[@]}")
fi

log "dotnet test ${CMD_ARGS[*]}"
dotnet test "${CMD_ARGS[@]}"
