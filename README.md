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
   cd src/RobRef.DDD.WebApi
   dotnet ef database update
   cd ../..
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
   cd src/RobRef.DDD.WebApi
   dotnet ef database update
   cd ../..

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

### SSH Access Setup

To enable SSH access to the dev container:
1. Copy your public SSH key to `.devcontainer/.ssh-public-key`
2. The setup script will automatically configure SSH access during container creation

### Current Implementation Status
- ✅ **Domain Layer**: User aggregate with value objects (Email, Names, etc.)
- ✅ **Application Layer**: CQRS command/query handlers with full queryside functionality
- ✅ **Infrastructure Layer**: EF Core with SQL Server persistence and in-memory testing
- ✅ **Presentation Layer**: Complete Web API with user registration and query endpoints (see PLAN-0001)

**Ready to contribute?** Start with the [Development Guide](DEVELOPMENT.md) and check the [TODO](TODO.md) for current tasks and plans.
