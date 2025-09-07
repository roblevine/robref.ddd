# Session Notes

Log of session notes capturing decisions, rationale, and heuristics to maintain context across stateless interactions.

## 2025-09-01 - Phase 1.1 Domain Foundation Setup

### Decisions
- Created RobRef.DDD solution with Domain class library and xUnit test project
- Used .NET 8 as target framework (default from dotnet new)
- Placed Domain project in src/ and tests in tests/ following standard conventions
- Added project references from test to domain project
- Value object type strategy: record class for nullable/optional (Email, names), readonly record struct for required/never-null (UserId)
- All value objects immutable - changes create new instances, never mutate existing
- Email implemented as record class with regex validation, implicit conversions, 17 tests passing
- UserId implemented as readonly record struct with ULID (Cysharp library), factory methods, Parse/TryParse, 15 tests passing
- FirstName, LastName, Title implemented as record classes with length validation, cultural sensitivity (single chars), 54 tests total
- User aggregate root implemented as mutable entity class with factory pattern (Register/Create), domain behavior methods, ID-based equality, 13 tests
- IUserRepository interface implemented with async methods for domain contract definition (no tests needed - contract only)

### Rationale
- Following PLAN-0001 Phase 1.1 exactly as specified
- Standard .NET solution structure supports onion architecture layers
- Test-first approach requires test project setup from start

### Rejected Alternatives
- Single project approach - doesn't support DDD layering
- Different test framework - xUnit is .NET standard
- Username value object - not needed for shopfront domain
- PersonalInfo composite - individual fields simpler
- Base classes first - concrete-first approach more practical

### Pending Intents
- Skip base classes, build concrete value objects first (Email ✅, UserId ✅, FirstName/LastName/Title ✅, User ✅)
- Skip Username - not needed for shopfront app (Email + individual name fields sufficient)
- Skip PersonalInfo composite - individual fields simpler for shopfront domain
- Next: IUserRepository interface for domain layer
- Phase 1.2: Extract common patterns into base classes if needed later

### Heuristics
- Always update PLAN checkboxes when completing tasks
- Verify build success after each phase
- Maintain session notes after each significant milestone

## 2025-09-07 - Phase 5 EF Core Implementation Complete

### Decisions
- Implemented EF Core SQL Server persistence layer with ApplicationDbContext and UserEntityConfiguration
- Created value object converters for Email, FirstName, LastName, Title (nullable), UserId (ULID -> string(26))
- EfUserRepository implementation with proper entity tracking and change management
- Added both in-memory (AddInfrastructure) and EF Core (AddInfrastructureWithEfCore) DI configurations
- Created initial database migration with proper schema (Users table, unique email index)
- Comprehensive EF Core integration tests (38 passing) using InMemory provider for test isolation
- Fixed entity ordering by using .Value properties instead of value objects for LINQ queries
- Implemented proper duplicate email validation in repository layer

### Rationale
- EF Core SQL Server chosen as requested for production persistence  
- Value object conversions ensure proper domain model encapsulation while supporting database mapping
- Separate DI methods allow flexibility for testing (in-memory) vs production (SQL Server)
- InMemory provider for tests provides fast, isolated testing without database dependencies
- Manual email uniqueness validation needed as InMemory provider doesn't enforce unique constraints

### Rejected Alternatives
- Using EF Core's built-in unique constraint validation - not reliable with InMemory provider
- Sorting by value objects directly - caused IComparable exceptions in LINQ queries
- Single DI configuration method - flexibility needed for different environments

### Pending Intents
- Phase 6: Web API implementation (POST /api/users/register endpoint)
- Remove temporary Program.cs from Infrastructure when Web API project is created
- Consider adding database seeding and health checks for production deployment

### Heuristics
- Always test EF Core repositories with both in-memory and real database scenarios
- Use separate DbContext instances per test to avoid entity tracking conflicts
- Clear EF Core change tracker when testing object identity vs value equality

### Bootstrap Snippet
Working on PLAN-0001 User Domain Implementation. Completed Phases 1-5 (Domain, Application, Infrastructure with EF Core persistence). Full User domain with SQL Server persistence, migrations, and comprehensive test coverage (137 total tests passing). Ready for Phase 6 Web API implementation.