#!/bin/bash

# TestContainers Test Runner
# This script configures TestContainers for DevContainer/DooD environments and runs the tests

set -e

# Configure TestContainers for DevContainer/DooD environments
export TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal
export TESTCONTAINERS_RYUK_DISABLED=true

echo "Running TestContainers tests with DooD configuration..."
echo "Environment variables set:"
echo "  TESTCONTAINERS_HOST_OVERRIDE=$TESTCONTAINERS_HOST_OVERRIDE"
echo "  TESTCONTAINERS_RYUK_DISABLED=$TESTCONTAINERS_RYUK_DISABLED"
echo ""

# Run the TestContainers tests
dotnet test --filter "Database=Testcontainers" "$@"