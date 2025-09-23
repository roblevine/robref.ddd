# Development Guide

This document provides comprehensive development guidance for the RobRef.DDD application monorepo, including setup, workflow, and service development patterns.

## Development Workflow

### Methodology
We follow a **Test-First Analyse → Plan → Execute → Review** methodology:

1. **Analyse**: Understand requirements and break down problems
2. **Plan**: Document approach in PLAN-*.md files for major features
3. **Execute**: Implement in small, testable increments
4. **Review**: Verify functionality and update documentation

### Ways of working
- **Conventional Commits**: Use conventional commit messages for all changes (e.g. feat, fix, docs, chore)

### Code Quality Standards
- **Test-First Development**: Write tests before implementing functionality (non-negotiable)
- **Warnings as Errors**: All projects must have `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` enabled
- **SOLID Principles**: Clear responsibilities, dependency inversion, clean interfaces
- **Meaningful Names**: Self-documenting code with clear variable and function names
- **Documentation Currency**: Update docs with any architectural or API changes

## Development Environment

### Dev Container Setup
The project uses VS Code Dev Containers with Docker-outside-of-Docker (DooD) for consistent development environments:

- **Base Image**: `mcr.microsoft.com/devcontainers/dotnet:8.0`
- **Docker Access**: Full Docker and Docker Compose functionality via host daemon
- **Auto-setup**: SSH keys, tools, and extensions configured automatically
- **Database**: SQL Server accessible via Docker Compose

### Docker Integration
- **Docker-outside-of-Docker**: Access host Docker daemon from within dev container
- **Database Services**: Run SQL Server via `docker-compose up -d sqlserver`
- **Container Testing**: Use `.devcontainer/docker-test.sh` to verify Docker functionality
- **Network Access**: Seamless connectivity between dev container and Docker services

### Tools and Extensions
- **C# DevKit**: Full .NET development support
- **SQL Server Extension**: Database management and query execution
- **Container Tools**: Docker and container management within VS Code

## Development Patterns

### EF Core Development Practices
- **Value Object Constants**: Always define length/max length constants in value objects for EF configuration
- **Value Object Comparability**: Implement `IComparable<T>` on all value objects for LINQ sorting support
- **Entity Configuration**: Use separate `IEntityTypeConfiguration<T>` classes for each aggregate root
- **Repository Testing**: Use EF Core InMemory provider for isolated integration tests
- **Change Tracking**: Clear EF change tracker when testing object identity vs value equality
- **Migration Naming**: Use descriptive migration names reflecting the schema changes
- **Constraint Enforcement**: Manual validation in repositories when InMemory provider doesn't enforce constraints
- **DI Separation**: Provide separate DI methods for different persistence strategies (in-memory, SQL Server)
- **Domain Abstraction**: Repository methods work with domain objects, never leak EF Core concerns

### Database Development Workflow
1. **Add/Modify Value Objects**: Include length constants for database constraints
2. **Update Entity Configuration**: Apply constraints using value object constants
3. **Write Repository Tests**: Create comprehensive integration tests with InMemory provider
4. **Generate Migration**: Use `dotnet ef migrations add` with descriptive names
5. **Verify Migration**: Review generated SQL for correct schema and constraints
6. **Test Migration**: Ensure tests pass with both InMemory and real database scenarios

### Testing Strategies

#### Database Testing Approaches
We support multiple testing strategies for different scenarios:

**1. In-Memory Testing (Unit/Integration)**
- Fast execution for TDD workflows
- Good for testing business logic and basic repository operations
- Does not enforce all database constraints

**2. Docker Compose Testing (Local Development)**
- Shared SQL Server container for repeated test runs
- Manual setup required: `docker-compose up -d sqlserver`
- Good for local development with fast repeated execution

**3. TestContainers Testing (CI/Isolation)**
- Automatic container lifecycle management
- Complete test isolation with dedicated containers
- Ideal for CI/CD pipelines and ensuring test independence

#### TestContainers Configuration

**DevContainer/DooD Setup:**
TestContainers automatically detects containerized environments and configures itself appropriately. For manual control:

```bash
# Disable Ryuk resource reaper (useful in CI or devcontainers)
export TESTCONTAINERS_RYUK_DISABLED=true

# Override Docker host (useful for Docker Desktop)
export TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal
```

**Environment Detection:**
- Automatic detection of Docker-outside-of-Docker scenarios
- Ryuk disabled automatically in containerized environments
- Standard TestContainers environment variables supported

**Running Different Test Types:**
```bash
# Run all tests
dotnet test

# Run only in-memory tests (fastest)
dotnet test --filter "FullyQualifiedName~InMemory"

# Run Docker Compose tests (manual setup)
docker-compose up -d sqlserver
dotnet test --filter "Database=DockerCompose"

# Run TestContainers tests (DevContainer script)
./scripts/test-testcontainers.sh

# Run TestContainers tests (manual environment setup)
TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal TESTCONTAINERS_RYUK_DISABLED=true dotnet test --filter "Database=Testcontainers"
```