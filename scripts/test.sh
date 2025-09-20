#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=scripts/common.sh
source "$SCRIPT_DIR/common.sh"

# Keep this script aligned with the solution layout; update if new test assemblies
# are introduced outside the main solution or require custom logic.

usage() {
  cat <<USAGE
Usage: $(basename "$0") [dotnet test options]

Run the full test suite.

Examples:
  $(basename "$0")
  $(basename "$0") --no-build
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

log "dotnet test ${DOTNET_ARGS[*]}"
if [[ ${#DOTNET_ARGS[@]} -gt 0 ]]; then
  dotnet test "$REPO_ROOT/RobRef.DDD.sln" "${DOTNET_ARGS[@]}"
else
  dotnet test "$REPO_ROOT/RobRef.DDD.sln"
fi
