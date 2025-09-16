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
   docker-compose up -d
   ```

4. **Run the tests**
   ```bash
   dotnet test
   ```

5. **Run the Web API (development)**
   ```bash
   dotnet run --project src/RobRef.DDD.WebApi --no-launch-profile
   ```
   - In `Development` the API uses the in-memory infrastructure by default; set `ConnectionStrings__SqlServer` to switch to SQL Server
   - Swagger UI available at `http://localhost:5000/swagger` (HTTP) or `https://localhost:5001/swagger` (HTTPS)
   - Try a registration request: `POST /api/users/register` with `{ "email": "jane@example.com", "firstName": "Jane", "lastName": "Doe" }`

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

**Test Docker functionality:**
```bash
# Verify Docker is working correctly
. .devcontainer/docker-test.sh
```

**Start SQL Server database:**
```bash
# From within the dev container
docker-compose up -d sqlserver
```

### SSH Access Setup

To enable SSH access to the dev container:
1. Copy your public SSH key to `.devcontainer/.ssh-public-key`
2. The setup script will automatically configure SSH access during container creation

### Current Implementation Status
- ✅ **Domain Layer**: User aggregate with value objects (Email, Names, etc.)
- ✅ **Application Layer**: CQRS command/query handlers
- ✅ **Infrastructure Layer**: EF Core with SQL Server persistence
- ✅ **Presentation Layer**: Minimal Web API for user registration (see PLAN-0001)

**Ready to contribute?** Start with the [Development Guide](DEVELOPMENT.md) and check the [TODO](TODO.md) for current tasks and plans.
