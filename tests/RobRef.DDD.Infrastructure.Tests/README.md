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

### 2. Testcontainers (Modern)
**Use when:** CI/CD pipelines, isolated testing, automatic test infrastructure

**Setup:**
```bash
# No manual setup required - containers managed automatically

# Run Testcontainers tests
dotnet test --filter "Database=Testcontainers"
```

**Characteristics:**
- ✅ Automatic container lifecycle management
- ✅ Complete test isolation (each test class gets own container)
- ✅ No manual setup required
- ✅ Perfect for CI/CD environments
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

# Testcontainers tests only
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

### Testcontainers Configuration
Testcontainers automatically manages SQL Server containers with:
- Image: `mcr.microsoft.com/mssql/server:2022-latest`
- Password: `TestPassword123!`
- Automatic port assignment
- Cleanup after tests complete

## CI/CD Recommendations

### GitHub Actions Example
```yaml
- name: Run Docker Compose Tests (Fast Feedback)
  run: |
    docker-compose up -d sqlserver
    dotnet test --filter "Database=DockerCompose"
    docker-compose down

- name: Run Testcontainers Tests (Isolation Verification)
  run: dotnet test --filter "Database=Testcontainers"
```

### Local Development Workflow
```bash
# Quick feedback loop with Docker Compose
docker-compose up -d sqlserver
dotnet test --filter "Database=DockerCompose" --watch

# Full isolation testing with Testcontainers
dotnet test --filter "Database=Testcontainers"
```

## Implementation Details

Both approaches inherit from `UserRepositoryIntegrationTestsBase`, ensuring identical test coverage:

- **EfUserRepositorySqlServerTests** - Docker Compose with conditional skipping
- **EfUserRepositoryTestcontainersTests** - Testcontainers with automatic management

### Test Traits
- `[Trait("Database", "DockerCompose")]` - Traditional Docker Compose
- `[Trait("Database", "Testcontainers")]` - Modern Testcontainers

This allows flexible test execution based on environment and requirements.