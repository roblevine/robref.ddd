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

### Current Implementation Status
- ✅ **Domain Layer**: User aggregate with value objects (Email, Names, etc.)
- ✅ **Application Layer**: CQRS command/query handlers
- ✅ **Infrastructure Layer**: EF Core with SQL Server persistence
- 🔄 **Presentation Layer**: Web API (planned - see PLAN-0001)

**Ready to contribute?** Start with the [Development Guide](DEVELOPMENT.md) and check the [TODO](TODO.md) for current tasks and plans.