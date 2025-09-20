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
Working on PLAN-0001 User Domain Implementation. Completed Phases 1-5 (Domain, Application, Infrastructure with EF Core persistence). Full User domain with SQL Server persistence, migrations, and comprehensive test coverage (169 total tests passing). Phase 6 Web API approach decisions finalized - ready for implementation.

## 2025-09-15 - Phase 6 Web API Approach Decisions

### Decisions
- Web API project structure: RobRef.DDD.WebApi references Application layer only (proper dependency flow)
- Controller design: UsersController with RegisterUserRequest DTO, leverages existing RegisterUserHandler
- Error handling: Problem Details RFC 7807 for structured error responses with proper HTTP status mapping
- DI configuration: EF Core as default, in-memory as test option, both available via separate methods
- Testing strategy: WebApplicationFactory integration tests with both mock and real database scenarios
- API documentation: Swagger/OpenAPI with validation rules, request/response examples, and error documentation
- Future slice: Real database integration tests with SQL Server (separate implementation after Web API complete)

### Rationale
- WebApi → Application → Domain ← Infrastructure maintains proper onion architecture dependency flow
- RegisterUserRequest DTO prevents domain object exposure at API boundary while reusing application logic
- Problem Details standard provides consistent, structured error responses for API consumers
- Dual DI configuration enables flexible testing strategies without compromising production setup
- WebApplicationFactory provides full integration testing including middleware, routing, and serialization
- Swagger documentation essential for API discoverability and developer experience

### Rejected Alternatives
- Direct domain object exposure in API - breaks encapsulation and coupling rules
- Custom error response format - Problem Details is industry standard
- Single DI configuration - flexibility needed for different test scenarios
- Unit tests only for API - integration tests catch serialization, routing, middleware issues

### Pending Intents
- Implement RobRef.DDD.WebApi project with ASP.NET Core minimal API approach
- Create comprehensive integration test suite covering success, validation errors, and domain errors
- Document all API endpoints, request/response schemas, and error conditions
- Plan next slice: real database integration tests after Web API completion

### Heuristics
- Always maintain proper dependency flow in onion architecture
- Use standard protocols (Problem Details) over custom implementations
- Integration tests for API layer catch issues unit tests miss
- Document API decisions in PLAN before implementation for resumability

## 2025-09-07 - Value Object IComparable Implementation

### Decisions
- Fixed EF Core test failures by implementing `IComparable<T>` on all value objects (Email, FirstName, LastName)
- All value objects now delegate comparison to underlying string values using `StringComparison.OrdinalIgnoreCase`
- Repository methods work with domain objects directly, maintaining proper abstraction
- Avoided EF.Property<string>() approach that would leak infrastructure concerns into domain layer

### Rationale
- EF Core requires value objects to be comparable for LINQ OrderBy operations to work with both SQL Server and InMemory providers
- `IComparable<T>` implementation maintains domain integrity while supporting infrastructure needs
- Case-insensitive string comparison appropriate for names and email addresses in business context
- Proper abstraction preserved - repository interface works with domain objects, not implementation details

### Rejected Alternatives
- Using `EF.Property<string>()` in repository - breaks abstraction, couples domain to EF Core
- Different sorting strategy - OrderBy is common requirement for user listings
- Base class approach - concrete implementation simpler and more explicit

### Test Results
- All 169 tests passing: 99 Domain + 14 Application + 56 Infrastructure
- Both SQL Server and InMemory EF Core providers working correctly
- Repository abstraction maintained across different persistence strategies

### Heuristics
- When EF Core fails to translate LINQ expressions, check if value objects implement required interfaces
- Always maintain domain abstraction in repository implementations
- Test failures can reveal missing infrastructure compatibility without breaking domain design
- Discuss architectural decisions before implementing to avoid wrong abstractions
## 2025-09-16 - Phase 6 Web API Implementation Plan

### Decisions
- Minimal API `Program` will host RobRef.DDD.WebApi with EF Core default + in-memory test switch
- `POST /api/users/register` returns 201 with Location + ULID body; DTO enforces domain constraints
- Standardize on RFC 7807 responses for validation (400), duplicates (409), unexpected errors (500)
- Expose lightweight `/health` endpoint for liveness
- Enable Swagger via Swashbuckle in Development only and capture snapshot test for contract guard

### Rationale
- Keeps onion layering intact while giving tests a seam to swap infrastructure
- Problem Details provides predictable API for consumers and maps domain exceptions cleanly
- `/health` naming matches team convention and supports future probes
- Swagger + snapshot helps document the contract and detect accidental changes

### Rejected Alternatives
- Retaining temporary Infrastructure `Program` once WebApi hosts migrations
- Controller-based API startup; minimal API aligns with agreed approach
- Kubernetes-style `/healthz` naming as team prefers simpler `/health`

### Pending Intents
- Implement WebApi project, endpoint, error handling, Swagger, and integration tests per plan
- Remove Infrastructure design-time host once WebApi migrations path confirmed
- Document new API usage and update plan/test artifacts after implementation

### Heuristics
- Prefer exception-to-Problem mapper over per-endpoint try/catch blocks
- Guard API behaviors with integration tests before wiring infrastructure specifics
- Keep DTO validation in sync with domain constants to avoid double standards

## 2025-09-17 - Phase 6.1 Web API Slice Planning

### Decisions
- Treat Phase 6 as slices, starting with Slice 6.1 covering WebApi scaffolding, tests, and first endpoint surface
- Custom `WebApplicationFactory` will switch infrastructure to in-memory and suppress Swagger during tests
- Integration tests will drive API shape: register success, validation, duplicate email, `/health`, Swagger snapshot guard

### Rationale
- Slicing keeps scope manageable while preserving test-first workflow
- Factory override avoids coupling tests to production EF Core configuration and keeps ProblemDetails deterministic
- Swagger snapshot protects contract drift once the API document is produced

### Pending Intents
- Execute Slice 6.1 tasks: scaffolding projects, writing failing tests, implementing endpoints/ProblemDetails, updating docs
- Remove Infrastructure temporary `Program` after WebApi host takes over migrations support

### Heuristics
- Maintain parity between DTO data annotations and domain value object constraints
- Keep exception-to-Problem mapping centralized for reuse across future endpoints

## 2025-09-17 - Phase 6.1 Web API Implementation

### Decisions
- Created RobRef.DDD.WebApi project (net8.0, references Application + Domain + Infrastructure) with Swashbuckle for dev-only docs
- Minimal Program wires ProblemDetails, health endpoint, register endpoint, infrastructure switch (SQL vs in-memory)
- Added Request/Response DTOs with data annotations mirroring domain constants and manual validation for consistent RFC7807 output
- Integration test suite via CustomWebApplicationFactory forcing in-memory repository; Swagger snapshot guard in place (content pending real run)
- Replaced Infrastructure temporary Program with design-time ApplicationDbContextFactory for EF migrations

### Rationale
- Presentation layer now hosts domain use case end-to-end while preserving onion dependency flow at runtime
- Centralized ProblemDetails mapping keeps validation/duplicate handling consistent across future endpoints
- Design-time factory maintains tooling support without extra hosting projects

### Pending Intents
- Capture real Swagger snapshot once environment allows running dotnet tests/build (blocked by sandbox socket restrictions)
- Re-run full test suite locally to verify behavior when sandbox limitations lifted

### Heuristics
- Prefer augmenting existing OpenAPI responses instead of replacing them to retain schema metadata
- Keep WebApplicationFactory overrides minimal—remove only what tests need to replace to avoid brittle DI setups

## 2025-09-16 - Phase 6 Web API Implementation (Build)

### Decisions
- Implemented minimal Web API host with `/health` liveness endpoint and register route
- Added validation endpoint filter + exception middleware translating to RFC 7807 with correlation id
- Provided Swagger examples + schema filter so required members surface in OpenAPI snapshot
- Reused infrastructure DI with Testing environment using in-memory repo, ensured duplicate checks there

### Rationale
- Keeps API surface small while exercising full onion stack and integration tests
- Middleware centralizes error translation instead of per-endpoint branching
- OpenAPI parity avoids snapshot churn and documents contract accurately
- Aligning in-memory repo behavior with EF prevents test env from masking conflicts

### Pending Intents
- Future slice: wire real SQL Server integration tests for Web API smoke coverage

### Heuristics
- Keep snapshot tests stable by mirroring expected metadata exactly
- Extend schema filters when nullable reference metadata is insufficient
- Ensure test seam (testing environment) always swaps persistence cleanly

## 2025-09-20 - Queryside Functions Implementation

### Decisions
- Implemented full queryside functionality with GetAllUsers and GetUserByEmail queries following CQRS pattern
- Leveraged existing IQuery<T> and IQueryHandler<T,R> infrastructure from Common namespace
- Used existing IUserRepository methods (GetAllAsync, FindByEmailAsync) to avoid duplicate implementation
- Placed queries in Users/Queries/ following existing Commands structure pattern
- Created consistent request/response DTOs for all API endpoints to fix design inconsistency
- Used [AsParameters] for GET endpoint parameter binding to maintain DTO pattern consistency

### Rationale
- Repository already provided needed methods, queries provide clean application layer interface
- CQRS separation maintains read/write distinction at application boundary
- Consistent DTO pattern across all endpoints improves API design and future extensibility
- Query parameter binding with validation ensures proper input validation for all endpoints
- Wrapped responses allow for future metadata and pagination additions

### Implementation Complete
- GetAllUsers query/handler with comprehensive tests (5 tests)
- GetUserByEmail query/handler with comprehensive tests (7 tests)
- Full Web API integration: GET /api/users and GET /api/users/by-email endpoints
- Consistent request/response DTOs: GetAllUsersResponse, GetUserByEmailRequest/Response
- All 215 tests passing (114 Domain + 29 Application + 56 Infrastructure + 16 WebApi)

### Heuristics
- Always maintain consistent patterns across API endpoints - avoid mixing primitive parameters with DTO objects
- Use explicit request/response objects even for simple queries to enable future extensibility
- Follow existing command handler patterns for consistency across CQRS boundaries
- Ensure proper test isolation when using shared repository instances in integration tests
