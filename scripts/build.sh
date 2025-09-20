#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=scripts/common.sh
source "$SCRIPT_DIR/common.sh"

usage() {
  cat <<USAGE
Usage: $(basename "$0") [dotnet build options]

Build the RobRef.DDD solution.

Examples:
  $(basename "$0")
  $(basename "$0") --configuration Release --no-restore
USAGE
}

DOTNET_ARGS=()
while [[ $# -gt 0 ]]; do
  case "$1" in
    -h|--help)
      usage
      exit 0
      ;;
    *)
      DOTNET_ARGS+=("$1")
      ;;
  esac
  shift
done

ensure_dotnet

log "dotnet build ${DOTNET_ARGS[*]}"
if [[ ${#DOTNET_ARGS[@]} -gt 0 ]]; then
  dotnet build "$REPO_ROOT/RobRef.DDD.sln" "${DOTNET_ARGS[@]}"
else
  dotnet build "$REPO_ROOT/RobRef.DDD.sln"
fi
