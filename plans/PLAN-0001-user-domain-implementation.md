# PLAN-0001: User Domain Implementation

**Status:** PLANNING  
**Started:** August 30, 2025  
**Approach:** Test-First Development, Domain-Driven Design, Onion Architecture

## Overview

Implement the User domain for the online shopping platform, starting with core domain entities and building outward through application services to infrastructure. This follows strict DDD principles with value objects, aggregate roots, and domain services.

## Scope & Boundaries

### ✅ In Scope
- User aggregate root with proper domain modeling
- Value objects: Email, Username, PersonalInfo (Title, FirstName, LastName)
- Domain validation and business rules
- User registration use case via application service
- Repository pattern with in-memory implementation
- REST API endpoint: `POST /api/users/register`
- Complete test coverage (unit, integration, acceptance)
- .NET solution structure following onion architecture

### ❌ Out of Scope (Future Plans)
- Database persistence (EF Core implementation)
- Authentication/JWT tokens (separate authentication service)
- Password management (handled by authentication service)
- User profile updates
- Email verification workflow

## Architecture Overview

```
src/
├── RobRef.DDD.Domain/              # Core domain layer
│   ├── Users/
│   │   ├── User.cs                 # Aggregate root
│   │   ├── UserId.cs              # Strongly-typed ID
│   │   ├── Email.cs               # Value object
│   │   ├── Title.cs               # Value object (optional)
│   │   ├── FirstName.cs           # Value object
│   │   ├── LastName.cs            # Value object
│   │   └── IUserRepository.cs     # Repository interface
│   └── Common/                     # (Skipped - concrete-first approach)
│       ├── Entity.cs              # Base entity (not implemented)
│       ├── ValueObject.cs         # Base value object (not implemented)
│       └── DomainException.cs     # Domain exceptions (not implemented)
├── RobRef.DDD.Application/         # Application layer
│   ├── Users/
│   │   ├── Commands/
│   │   │   ├── RegisterUser.cs    # Command
│   │   │   └── RegisterUserHandler.cs # Command handler
│   │   └── Services/
│   │       └── UserApplicationService.cs
│   └── Common/
│       └── ICommand.cs            # CQRS interfaces
├── RobRef.DDD.Infrastructure/      # Infrastructure layer
│   ├── Persistence/
│   │   └── InMemoryUserRepository.cs
│   └── Configuration/
│       └── DependencyInjection.cs
└── RobRef.DDD.WebApi/             # Presentation layer
    ├── Controllers/
    │   └── UsersController.cs
    ├── DTOs/
    │   └── RegisterUserRequest.cs
    └── Program.cs
```

## Implementation Steps

### Phase 1: Domain Foundation (Core First)
1. **Setup Domain Project**
   - [x] Create solution file
   - [x] Add Domain class library project only
   - [x] Setup test project for domain testing

2. **Domain Base Classes** *(Skipped - concrete-first approach)*
   - [~] Implement base Entity class *(deferred - extract patterns later if needed)*
   - [~] Implement base ValueObject class *(deferred - extract patterns later if needed)*
   - [~] Create DomainException for domain-specific errors *(deferred - extract patterns later if needed)*

### Phase 2: User Domain Model (Test-First)
3. **Core Value Objects**
   - [x] Email: Validation, immutability, equality
   - [x] Title, FirstName, LastName: Individual value objects

4. **User Aggregate Root**
   - [x] UserId strongly-typed identifier
   - [x] User entity with value object composition
   - [x] Domain validation rules
   - [x] Factory method for user creation
   - [x] Domain unit tests (>90% coverage)

5. **Domain Services & Interfaces**
   - [x] IUserRepository interface (domain layer only)
   - [x] Domain-focused methods (FindByEmail, Save, etc.)
   - [x] No infrastructure concerns in domain

### Phase 3: Application Layer (Use Cases) ✅
6. **Application Project Setup**
   - [x] Add Application class library project
   - [x] Reference Domain project only
   - [x] Setup application test project

7. **CQRS Foundation**
   - [x] Command/Query interfaces
   - [x] Base command handler pattern
   - [x] Application service base classes

8. **User Registration Use Case**
   - [x] RegisterUser command
   - [x] RegisterUserHandler with validation
   - [x] Application service coordination
   - [x] Application layer tests

### Phase 4: Infrastructure Layer (External Concerns) ✅
9. **Infrastructure Project Setup**
   - [x] Add Infrastructure class library project
   - [x] Reference Application and Domain projects
   - [x] Setup infrastructure test project

10. **Repository Implementation**
    - [x] In-memory repository implementation
    - [x] Thread-safe implementation
    - [x] Unique constraint enforcement
    - [x] Repository integration tests

11. **Dependency Injection Setup**
    - [x] Service registration patterns
    - [x] Lifetime management
    - [x] Configuration abstractions

### Phase 5: Real Persistence (Complete Domain Service)
12. **EF Core Implementation**
    - [ ] Entity Framework Core setup
    - [ ] User entity configuration and mapping
    - [ ] Value object conversions (Email, Names, etc.)
    - [ ] EfUserRepository implementation
    - [ ] Database migrations
    - [ ] Integration tests with real database

13. **Database Integration**
    - [ ] Connection string configuration
    - [ ] Database initialization and seeding
    - [ ] Transaction handling
    - [ ] Performance optimization
    - [ ] Database integration tests

### Phase 6: Presentation Layer (Complete User Service)
14. **Web API Implementation**
    - [ ] WebAPI project setup
    - [ ] UsersController with POST /api/users/register
    - [ ] DTO mapping and validation
    - [ ] HTTP error handling and responses
    - [ ] API integration tests
    - [ ] OpenAPI documentation

## Technical Decisions

### Domain Modeling
- **Aggregate Design**: User as aggregate root with value object composition
- **Identity**: Strongly-typed UserId (readonly record struct, Guid-based)
- **Value Objects**: record class for optional types (Email, names), readonly record struct for required types (Username, UserId)
- **Validation**: Domain-level validation in value objects and entities
- **Immutability**: Value objects immutable by design - changes create new instances

### Application Architecture  
- **CQRS**: Command/Query separation for scalability
- **Use Cases**: Application services coordinate domain operations
- **Validation**: Multi-layer validation (domain + application)

### Infrastructure Choices
- **Repository**: Abstract data access behind domain interfaces
- **Persistence**: In-memory for initial implementation
- **DI Container**: Built-in .NET DI container
- **API**: ASP.NET Core Web API with minimal configuration

## Success Criteria

### Functional Requirements
- ✅ User can register with email, title, first name, last name
- ✅ Email format validation enforced
- ✅ Name validation (required first/last name, optional title)
- ✅ Appropriate error messages for validation failures
- ✅ Registration returns success confirmation

### Technical Requirements
- ✅ All domain logic covered by unit tests
- ✅ Integration tests verify complete registration flow
- ✅ Code follows SOLID principles
- ✅ Proper separation of concerns across layers
- ✅ No infrastructure concerns leak into domain
- ✅ API follows REST conventions

### Quality Gates
- ✅ All tests pass (unit + integration)
- ✅ Code coverage >90% on domain layer
- ✅ No compiler warnings
- ✅ Architecture decision records updated
- ✅ README updated with usage examples

## Risk Mitigation

### Technical Risks
- **Complexity**: Start simple, add complexity incrementally
- **Over-engineering**: Focus on current requirements, not future speculation  
- **Testing**: Write tests first to ensure good design

### Implementation Risks
- **Dependencies**: Use minimal external dependencies initially
- **Performance**: Profile registration flow, optimize if needed
- **Security**: Ensure proper validation without exposing internal structure

## Next Steps After Completion

This plan delivers a complete User domain service with real persistence and Web API. Future bounded contexts:

1. **PLAN-0002**: Product Domain Implementation (separate bounded context)
2. **PLAN-0003**: Shopping Cart Domain Implementation (separate bounded context) 
3. **PLAN-0004**: Authentication Service Implementation (separate bounded context)

## Notes

This plan follows the established DDD patterns and maintains focus on the domain model. Each phase builds incrementally with comprehensive testing, ensuring a solid foundation for the online shopping platform.