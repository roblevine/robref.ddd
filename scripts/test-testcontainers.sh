#!/bin/bash

# TestContainers Test Runner
# This script runs the TestContainers tests

set -e

# Run the TestContainers tests with detailed logging
dotnet test --filter "Database=Testcontainers" --logger "console;verbosity=detailed" "$@"