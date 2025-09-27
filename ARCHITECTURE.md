# Architecture

This document outlines the architecture of the RobRef.DDD monorepo, implementing Domain-Driven Design with Onion Architecture across multiple bounded contexts. Each bounded context (Users today, Products forthcoming) carries an identical inner architecture so it can evolve, build, and deploy independently.

## High-Level Architecture

### Onion Architecture Layers

The application follows **Onion Architecture** with clear dependency rules: inner layers define interfaces, outer layers implement them. Dependencies point inward only.

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                   │
│              (Controllers, DTOs, WebAPI)                │
├─────────────────────────────────────────────────────────┤
│                  Infrastructure Layer                   │
│        (EF Core, Repositories, External Services)       │
├─────────────────────────────────────────────────────────┤
│                   Application Layer                     │
│         (Commands, Handlers, Use Cases, CQRS)           │
├─────────────────────────────────────────────────────────┤
│                     Domain Layer                        │
│    (Entities, Value Objects, Aggregates, Interfaces)    │
└─────────────────────────────────────────────────────────┘
```

#### 1. **Domain Layer** (Core)
- **Purpose**: Pure business logic, no external dependencies
- **Contains**: Entities, Value Objects, Aggregates, Domain Services, Repository Interfaces
- **Dependencies**: None (self-contained)
- **Example**: `Users` context → `User`, `Email`, `IUserRepository`

#### 2. **Application Layer** 
- **Purpose**: Orchestrate domain operations, implement use cases
- **Contains**: Commands, Queries, Handlers, Application Services, DTOs
- **Dependencies**: Domain layer only
- **Example**: `Users` context → `RegisterUserHandler`, `RegisterUserCommand`
- **DI Extensions**: Context-scoped DI registrations (e.g. `AddUsersApplication`)

#### 3. **Infrastructure Layer**
- **Purpose**: External concerns (database, files, APIs, frameworks)
- **Contains**: Repository implementations, Data contexts, External service clients, EF Core configurations
- **Dependencies**: Application + Domain layers
- **Example**: `Users` context → `UsersInMemoryRepository`, `UsersEfRepository`, `UsersApplicationDbContext`
- **DI Extensions**: Hosts compose via context-specific helpers (e.g. `AddUsersInfrastructureSqlServer`, `AddUsersInfrastructureInMemory`)

#### 4. **Presentation Layer** 
- **Purpose**: User interface, API endpoints, serialization
- **Contains**: Minimal API endpoints, request DTOs, validation filters, exception mapping
- **Dependencies**: Application + Domain layers, plus DI extensions from Infrastructure for runtime wiring
- **Example**: `Users` context → Minimal API host, `RegisterUserRequest`, exception middleware

### Service Development Principles
- **Domain-Driven Design**: Focus on core domain logic. Implement strictly by modelling entities, value types, and aggregate roots, etc. Prefer strongly-typed value objects with internal validation over native types.
- **Dependency Inversion**: Inner layers define interfaces, outer layers implement them
- **Separation of Concerns**: Each layer has single, well-defined responsibilities

## Bounded Context Layout

Repository layout reinforces isolation at the filesystem level:

```
bounded-contexts/
├── <context-name>/
│   ├── src/
│   │   ├── RobRef.DDD.<Context>.Domain/
│   │   ├── RobRef.DDD.<Context>.Application/
│   │   ├── RobRef.DDD.<Context>.Infrastructure/
│   │   └── RobRef.DDD.<Context>.WebApi/
│   └── tests/
│       ├── RobRef.DDD.<Context>.Domain.Tests/
│       ├── RobRef.DDD.<Context>.Application.Tests/
│       ├── RobRef.DDD.<Context>.Infrastructure.Tests/
│       └── RobRef.DDD.<Context>.WebApi.Tests/
```

The `Users` bounded context currently implements the full stack; `Products` will replicate the structure next. Shared tooling lives at repo root (`scripts/`, `plans/`, `Directory.Build.props`).

## Shared Components

While bounded contexts maintain autonomy, certain cross-cutting technical concerns are shared to avoid duplication and ensure consistency:

### Application Layer Shared Contracts

Located in `shared/RobRef.DDD.Application/Common/`:

- **CQRS Interfaces**: `ICommand`, `ICommand<TResult>`, `ICommandHandler<TCommand>`, `ICommandHandler<TCommand, TResult>`
- **Query Interfaces**: `IQuery<TResult>`, `IQueryHandler<TQuery, TResult>`
- **Consistency**: All handlers follow `HandleAsync(T, CancellationToken)` signature pattern
- **Documentation**: Comprehensive XML documentation for IntelliSense support

### Shared Component Principles

- **Technical-only**: Shared components contain no business logic, only technical contracts
- **Evolutionary**: Patterns extracted after second implementation to avoid premature abstraction  
- **Bounded**: Shared dependencies limited to well-defined technical concerns (CQRS, infrastructure patterns)
- **Versioned together**: All shared components evolve with solution to maintain compatibility

### Architectural Boundaries

Bounded contexts may only reference:
- Their own internal projects (within `bounded-contexts/<context>/`)
- Shared technical libraries (within `shared/`)
- External NuGet packages

Cross-bounded-context references are **strictly forbidden** to maintain autonomy.

## Implementation Details

### Domain Layer Design Principles
- **Value Objects**: Immutable types representing domain concepts
- **Aggregate Roots**: Entities that maintain consistency boundaries (e.g., User with factory methods)
- **Domain Services**: Stateless services for complex domain operations
- **Factory Pattern**: Static factory methods for aggregate creation (Register/Create separation)

### Value Object Implementation Strategy
- **record class**: For nullable/optional domain concepts (Email, FirstName, LastName, Title)
- **readonly record struct**: For required/never-null domain concepts (UserId)
- **Immutability**: All value objects immutable - changes create new instances
- **Validation**: Constructor validation with domain-specific rules
- **Equality**: Structural equality based on all properties (automatic with records)
- **Comparability**: All value objects implement `IComparable<T>` delegating to underlying values for EF Core sorting support
- **Identity**: UserId uses ULID for sortable, time-based unique identifiers (Cysharp library)
- **Length Constants**: All value objects define MaxLength/Length constants for database constraints

### Dependency Rules
1. **Domain** depends on nothing
2. **Application** depends only on Domain
3. **Infrastructure** depends on Application + Domain
4. **Presentation** composes Application + Domain services and consumes Infrastructure DI extensions for runtime wiring


### Presentation Layer Implementation
- **Hosting**: Minimal API hosted by `Program` with environment-based DI (Testing -> in-memory, others -> SQL Server)
- **Validation**: Request DTOs use data annotations enforced via `ValidationEndpointFilter` for consistent RFC 7807 responses
- **Error Handling**: Context-specific middleware maps domain/application exceptions to Problem Details with correlation ids
- **Documentation**: Swagger/OpenAPI provided via Swashbuckle with curated examples and schema filter to surface required members
- **Endpoints**: `Users` context exposes `/api/users/*`; future contexts follow similar patterns

### EF Core Persistence Patterns
- **DbContext**: Single context per bounded context (e.g., `ApplicationDbContext`)
- **Entity Configuration**: Separate configuration classes implementing `IEntityTypeConfiguration<T>`
- **Value Object Conversion**: Explicit converters for all value objects using domain constants
- **Value Object Sorting**: All value objects implement `IComparable<T>` to support LINQ OrderBy operations
- **Repository Implementation**: EF Core repositories implementing domain interfaces
- **Dependency Injection**: Separate DI methods for different environments (in-memory vs SQL Server)
- **Database Constraints**: Unique indexes and proper column constraints defined in entity configuration
- **Migration Management**: Code-first migrations with descriptive names and proper rollback support

### Testing Strategy
- **Domain Layer**: Unit tests for business logic, value object behavior, aggregate invariants
- **Application Layer**: Unit tests for use case handlers, integration tests for complete flows
- **Infrastructure Layer**: Integration tests for database operations using InMemory provider for isolation
- **Presentation Layer**: API integration tests, end-to-end scenarios

## Libraries and frameworks

### DO NOT USE
The following libraries should not be considered as part of this solution
- Fluent Assertions
