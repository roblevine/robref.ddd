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

### Phase 3: Application Layer (Use Cases)
6. **Application Project Setup**
   - [ ] Add Application class library project
   - [ ] Reference Domain project only
   - [ ] Setup application test project

7. **CQRS Foundation**
   - [ ] Command/Query interfaces
   - [ ] Base command handler pattern
   - [ ] Application service base classes

8. **User Registration Use Case**
   - [ ] RegisterUser command
   - [ ] RegisterUserHandler with validation
   - [ ] Application service coordination
   - [ ] Application layer tests

### Phase 4: Infrastructure Layer (External Concerns)
9. **Infrastructure Project Setup**
   - [ ] Add Infrastructure class library project
   - [ ] Reference Application and Domain projects
   - [ ] Setup infrastructure test project

10. **Repository Implementation**
    - [ ] In-memory repository implementation
    - [ ] Thread-safe implementation
    - [ ] Unique constraint enforcement
    - [ ] Repository integration tests

11. **Dependency Injection Setup**
    - [ ] Service registration patterns
    - [ ] Lifetime management
    - [ ] Configuration abstractions

### Phase 5: Presentation Layer (Separate Deliverable)
*Note: This phase can be implemented separately as a different project/solution*

12. **Console Application (Simple First)**
    - [ ] Console app for testing the domain/application layers
    - [ ] Manual registration workflow
    - [ ] Dependency injection container setup
    - [ ] End-to-end testing via console

### Future Phase: Web API (Separate Plan)
*To be documented in PLAN-0002: Web API Implementation*
- REST API endpoints
- DTO mapping and validation
- HTTP error handling
- API integration testing
- OpenAPI documentation

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

1. **PLAN-0002**: Authentication & JWT implementation
2. **PLAN-0003**: Product domain modeling  
3. **PLAN-0004**: Shopping cart functionality
4. **PLAN-0005**: Database persistence with EF Core

## Notes

This plan follows the established DDD patterns and maintains focus on the domain model. Each phase builds incrementally with comprehensive testing, ensuring a solid foundation for the online shopping platform.