# Infrastructure Tests

This project contains integration tests for the infrastructure layer, including repository implementations with both in-memory and SQL Server providers.

## Database Testing Approaches

We support two approaches for SQL Server integration testing:

### 1. Docker Compose (Traditional)
**Use when:** Local development, fast repeated test runs, shared team environment

**Setup:**
```bash
# Start SQL Server container
docker-compose up -d sqlserver

# Run Docker Compose tests
dotnet test --filter "Database=DockerCompose"
```

**Characteristics:**
- ✅ Fast repeated runs (shared container)
- ✅ Familiar Docker Compose workflow
- ⚠️ Manual container management required
- ⚠️ Tests may interfere if run in parallel
- ⚠️ Requires `docker-compose.yml` setup

### 2. TestContainers (Modern)
**Use when:** CI/CD pipelines, isolated testing, automatic test infrastructure

**Setup:**
```bash
# No manual setup required - containers managed automatically

# Run TestContainers tests
dotnet test --filter "Database=Testcontainers"
```

**Characteristics:**
- ✅ Automatic container lifecycle management
- ✅ Complete test isolation (each test class gets own container)
- ✅ No manual setup required
- ✅ Perfect for CI/CD environments
- ✅ Follows TestContainers standard patterns
- ⚠️ Slower startup (new container per test class)
- ⚠️ Requires Docker daemon access

## Running Tests

### Run All Integration Tests
```bash
dotnet test tests/RobRef.DDD.Infrastructure.Tests
```

### Run Specific Database Provider
```bash
# Docker Compose tests only
dotnet test --filter "Database=DockerCompose"

# TestContainers tests only
dotnet test --filter "Database=Testcontainers"
```

### Run In-Memory Tests Only
```bash
dotnet test --filter "FullyQualifiedName~InMemory"
```

## Test Configuration

### Docker Compose Configuration
Tests use `testsettings.local.json` for SQL Server connection settings:

```json
{
  "SqlServer": {
    "ConnectionString": "Server=localhost,1433;User Id=SA;Password=DevPassword123!;TrustServerCertificate=true;Connection Timeout=5;",
    "TestDatabasePrefix": "RobRefDDD_Test_"
  }
}
```

### TestContainers Configuration

TestContainers automatically manages SQL Server containers with:
- Image: `mcr.microsoft.com/mssql/server:2022-latest`
- Password: `TestPassword123!`
- Automatic port assignment
- Standard TestContainers cleanup patterns

#### Environment Variables (Optional)

For advanced scenarios, you can configure TestContainers behavior:

```bash
# Disable Ryuk resource reaper (useful in CI or devcontainers)
export TESTCONTAINERS_RYUK_DISABLED=true

# Override Docker host (useful for Docker Desktop)
export TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal
```

#### DevContainer/DooD Usage

When running in devcontainers or Docker-outside-of-Docker scenarios, use the provided script:

```bash
# Run TestContainers tests with proper DooD configuration
./scripts/test-testcontainers.sh
```

**Manual Configuration:**
1. **Required Environment Variables**: 
   - `TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal` (for Docker Desktop)
   - `TESTCONTAINERS_RYUK_DISABLED=true` (disable Ryuk in containers)
2. **Docker Socket**: Ensure `/var/run/docker.sock` is mounted in your devcontainer
3. **Manual Run**: `TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal TESTCONTAINERS_RYUK_DISABLED=true dotnet test --filter "Database=Testcontainers"`

## CI/CD Recommendations

### GitHub Actions Example
```yaml
- name: Run Docker Compose Tests (Fast Feedback)
  run: |
    docker-compose up -d sqlserver
    dotnet test --filter "Database=DockerCompose"
    docker-compose down

- name: Run TestContainers Tests (Isolation Verification)
  run: dotnet test --filter "Database=Testcontainers"
  env:
    TESTCONTAINERS_RYUK_DISABLED: true
```

### Local Development Workflow
```bash
# Quick feedback loop with Docker Compose
docker-compose up -d sqlserver
dotnet test --filter "Database=DockerCompose" --watch

# Full isolation testing with TestContainers
dotnet test --filter "Database=Testcontainers"
```

## Implementation Details

Both approaches inherit from `UserRepositoryIntegrationTestsBase`, ensuring identical test coverage:

- **EfUserRepositorySqlServerTests** - Docker Compose with conditional skipping
- **EfUserRepositoryTestcontainersTests** - TestContainers with automatic management

### Test Traits
- `[Trait("Database", "DockerCompose")]` - Traditional Docker Compose
- `[Trait("Database", "Testcontainers")]` - Modern TestContainers

### TestContainers Cleanup

The TestContainers implementation uses standard patterns for cleanup:

1. **Explicit Disposal**: `IAsyncLifetime.DisposeAsync()` ensures containers are properly stopped
2. **Ryuk Resource Reaper**: Automatically cleans up containers when tests complete (disabled in containerized environments)
3. **xUnit Fixtures**: Guarantee disposal even if tests fail
4. **Environment-Based Configuration**: Uses standard TestContainers environment variables

This provides robust cleanup without custom Docker command execution or complex retry logic.

## Troubleshooting

### TestContainers Issues

**Container not starting:**
- Check Docker daemon is running
- Verify port availability
- Check Docker socket permissions

**Cleanup issues in devcontainers:**
- Ensure `TESTCONTAINERS_RYUK_DISABLED=true` is set
- Verify Docker socket is mounted
- Check devcontainer has Docker access

**Connection failures:**
- TestContainers handles connection strings automatically
- Brief delays on container startup are normal
- Check firewall/network policies

This allows flexible test execution based on environment and requirements.