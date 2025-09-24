#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=scripts/common.sh
source "$SCRIPT_DIR/common.sh"

usage() {
  cat <<USAGE
Usage: $(basename "$0") [options] [-- <dotnet run args>]

Run the Web API in Testing environment (uses in-memory persistence).

Options:
  --watch        Use dotnet watch run
  -h, --help     Show this help

Arguments after -- are passed straight to dotnet run.
USAGE
}

WATCH=0
PASSTHROUGH=()

while [[ $# -gt 0 ]]; do
  case "$1" in
    --watch)
      WATCH=1
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    --)
      shift
      PASSTHROUGH=("$@")
      break
      ;;
    *)
      PASSTHROUGH+=("$1")
      ;;
  esac
  shift
done

ensure_dotnet
PROJECT="$REPO_ROOT/bounded-contexts/users/src/RobRef.DDD.Users.WebApi/RobRef.DDD.Users.WebApi.csproj"

if [[ $WATCH -eq 1 ]]; then
  if [[ ${#PASSTHROUGH[@]} -gt 0 ]]; then
    log "ASPNETCORE_ENVIRONMENT=Testing dotnet watch --project $PROJECT run -- ${PASSTHROUGH[*]}"
    ASPNETCORE_ENVIRONMENT=Testing dotnet watch --project "$PROJECT" run -- "${PASSTHROUGH[@]}"
  else
    log "ASPNETCORE_ENVIRONMENT=Testing dotnet watch --project $PROJECT run"
    ASPNETCORE_ENVIRONMENT=Testing dotnet watch --project "$PROJECT" run
  fi
else
  if [[ ${#PASSTHROUGH[@]} -gt 0 ]]; then
    log "ASPNETCORE_ENVIRONMENT=Testing dotnet run --project $PROJECT -- ${PASSTHROUGH[*]}"
    ASPNETCORE_ENVIRONMENT=Testing dotnet run --project "$PROJECT" -- "${PASSTHROUGH[@]}"
  else
    log "ASPNETCORE_ENVIRONMENT=Testing dotnet run --project $PROJECT"
    ASPNETCORE_ENVIRONMENT=Testing dotnet run --project "$PROJECT"
  fi
fi
