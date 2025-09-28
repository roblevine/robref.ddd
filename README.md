# RobRef.DDD - Simple example DDD .Net implementation

A simple example of Domain-Driven Design (DDD) principles applied in a .Net application, showcasing the key concepts and patterns.
This is based on a simple online shopping domain, involving users, products, and a shopping cart.

## Quick Start

### Prerequisites
- .NET 8 SDK
- Docker and Docker Compose

### Setup
1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd robref.ddd.claude
   ```

2. **Configure environment variables**
   ```bash
   # Copy the template and update with your values
   cp .env.template .env
   ```
   
   Edit the `.env` file and set a secure password for SQL Server:
   ```
   SQLSERVER_SA_PASSWORD=YourSecurePassword123!
   ```
   
   **Important:** The password must meet SQL Server complexity requirements:
   - At least 8 characters
   - Contains uppercase and lowercase letters
   - Contains numbers and special characters

3. **Start the database**
   ```bash
   ./scripts/start-sqlserver.sh
   ```

4. **Apply database migrations** (required for SQL Server setup)
   ```bash
   # Install Entity Framework CLI tool (if not already installed)
   dotnet tool install --global dotnet-ef

   # Apply migrations to create database schema
   cd bounded-contexts/users/src/RobRef.DDD.Users.WebApi
   dotnet ef database update
   cd ../../../../
   ```

5. **Run the tests**
   ```bash
   ./scripts/test.sh
   ```
   
   **Testing Options:**
   - In-memory tests (fastest): `dotnet test --filter "FullyQualifiedName~InMemory"`
   - Docker Compose tests: `docker-compose up -d sqlserver && dotnet test --filter "Database=DockerCompose"`
   - TestContainers tests (automatic): `dotnet test --filter "Database=Testcontainers"`

6. **Run the Web API**

   **Option A: In-Memory Storage (quickest for testing)**
   ```bash
   ./scripts/run-api-test.sh
   ```

   **Option B: With SQL Server (requires database setup)**
   ```bash
   # First start the database and apply migrations (if not done already)
   ./scripts/start-sqlserver.sh
   cd bounded-contexts/users/src/RobRef.DDD.Users.WebApi
   dotnet ef database update
   cd ../../../../

   # Then run the API
   ./scripts/run-api-dev.sh
   ```

   **Access the API:**
   - Swagger UI: `http://localhost:8080/swagger` (Testing) or `http://localhost:5000/swagger` (Development)
   - Health check: `GET /health`
   - User registration: `POST /api/users/register` with `{ "email": "jane@example.com", "firstName": "Jane", "lastName": "Doe" }`
   - Get all users: `GET /api/users`
   - Get user by email: `GET /api/users/by-email?email=jane@example.com`

## Development Container Setup

This project includes a VS Code development container with Docker-outside-of-Docker (DooD) support for seamless container development.

### Dev Container Features
- ✅ **.NET 8 SDK** - Pre-installed and configured
- ✅ **Node.js LTS** - For any frontend tooling
- ✅ **Docker-outside-of-Docker** - Access host Docker daemon from container
- ✅ **SQL Server Extension** - Database management and querying
- ✅ **SSH Key Support** - Automated SSH key setup for secure access

### Using Docker from Dev Container

The dev container is configured to use Docker-outside-of-Docker, allowing you to:
- Build and run Docker images using the host Docker daemon
- Use `docker-compose` commands directly
- Access the same Docker network as your host machine
- Run TestContainers for automated integration testing

**Test Docker functionality:**
```bash
# Verify Docker is working correctly
. .devcontainer/docker-test.sh
```

**Start SQL Server database:**
```bash
# From within the dev container
./scripts/start-sqlserver.sh
```

### Testing with TestContainers

The project includes TestContainers support for automatic integration testing with real databases:

**Key Features:**
- ✅ **Automatic Cleanup** - Containers are automatically started and stopped
- ✅ **DevContainer Ready** - Automatically configures for Docker-outside-of-Docker scenarios
- ✅ **CI/CD Friendly** - Works in containerized CI environments
- ✅ **Standard Patterns** - Uses official TestContainers best practices

**Environment Configuration:**
For devcontainer/DooD environments, use the provided script:
```bash
# Run TestContainers tests with proper DooD configuration
./scripts/test-testcontainers.sh
```

Or set environment variables manually:
```bash
# Required for Docker Desktop and devcontainer environments
export TESTCONTAINERS_HOST_OVERRIDE=host.docker.internal
export TESTCONTAINERS_RYUK_DISABLED=true

# Then run the tests
dotnet test --filter "Database=Testcontainers"
```

### Helper Scripts

The `scripts/` directory contains convenience wrappers around common workflows:
- `./scripts/build.sh` — runs `dotnet build` for the full solution
- `./scripts/test.sh` — executes the full test suite
- `./scripts/run-api-dev.sh` — runs the Web API in Development (SQL Server)
- `./scripts/run-api-test.sh` — runs the Web API in Testing (in-memory)
- `./scripts/start-sqlserver.sh` — starts the SQL Server Docker container (optional `--logs` to tail output)

All scripts automatically load variables from `.env` when present. Use the `--help` flag on any script to see available options.

> **Heads-up:** When you add new projects (especially test assemblies) or change default tooling conventions, update these helper scripts so they stay in sync with the repository structure.

## Repository Structure

### Solution Entry Points

Developers can choose the solution file that matches the slice of the system they want to work on:
- `RobRef.DDD.sln` — full monorepo view with every bounded context plus shared tooling and architectural fitness tests
- `bounded-contexts/users/RobRef.DDD.Users.sln` — users context, shared abstractions, and the architectural fitness suite for quick feedback without product projects
- `bounded-contexts/products/RobRef.DDD.Products.sln` — products context with shared abstractions; use the root solution when you need the cross-context fitness tests

This monorepo hosts multiple bounded contexts. Each context carries its own Domain, Application, Infrastructure, Web API, and test projects so it can be built, tested, and deployed independently.

```
bounded-contexts/
├── users/
│   ├── src/
│   │   ├── RobRef.DDD.Users.Domain/
│   │   ├── RobRef.DDD.Users.Application/
│   │   ├── RobRef.DDD.Users.Infrastructure/
│   │   └── RobRef.DDD.Users.WebApi/
│   ├── tests/
│       ├── RobRef.DDD.Users.Domain.Tests/
│       ├── RobRef.DDD.Users.Application.Tests/
│       ├── RobRef.DDD.Users.Infrastructure.Tests/
│       └── RobRef.DDD.Users.WebApi.Tests/
│   └── RobRef.DDD.Users.sln          # Context-specific solution
├── products/ (planned)
│   └── …
└── shared/ (reserved for future cross-context libraries, if required)

scripts/
plans/
```

Top-level tooling (e.g. `scripts/`, `plans/`, `Directory.Build.props`) continues to apply across every bounded context.

### SSH Access Setup

To enable SSH access to the dev container:
1. Copy your public SSH key to `.devcontainer/.ssh-public-key`
2. The setup script will automatically configure SSH access during container creation

### Current Implementation Status
- ✅ **Users bounded context** (`bounded-contexts/users`)
  - Domain aggregate, value objects, and repository abstractions complete
  - Application CQRS command/query handlers with full queryside functionality
  - Infrastructure EF Core persistence plus in-memory option with comprehensive tests
  - Web API with registration + query endpoints (see PLAN-0001)
- 🚧 **Products bounded context** planned (PLAN-0002 pending)
- 🔜 **Additional bounded contexts** (e.g. shopping cart, authentication) will follow the same structure

**Ready to contribute?** Start with the [Development Guide](DEVELOPMENT.md) and check the [TODO](TODO.md) for current tasks and plans.
