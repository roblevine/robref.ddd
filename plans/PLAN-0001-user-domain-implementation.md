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
│   │   ├── Username.cs            # Value object  
│   │   ├── Title.cs               # Value object (optional)
│   │   ├── FirstName.cs           # Value object
│   │   ├── LastName.cs            # Value object
│   │   ├── PersonalInfo.cs        # Value object composite
│   │   └── IUserRepository.cs     # Repository interface
│   └── Common/
│       ├── Entity.cs              # Base entity
│       ├── ValueObject.cs         # Base value object
│       └── DomainException.cs     # Domain exceptions
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

### Phase 1: Project Structure & Domain Foundation
1. **Setup .NET Solution**
   - [ ] Create solution file
   - [ ] Add class library projects (Domain, Application, Infrastructure)
   - [ ] Add Web API project
   - [ ] Configure project dependencies

2. **Domain Foundation**
   - [ ] Implement base Entity and ValueObject classes
   - [ ] Create DomainException for domain-specific errors
   - [ ] Setup project references following onion architecture

### Phase 2: User Domain Model
3. **Core Value Objects (Test-First)**
   - [ ] Email: Validation, immutability, equality
   - [ ] Username: Length rules, character restrictions, uniqueness
   - [ ] PersonalInfo: Title (optional), FirstName, LastName with validation

4. **User Aggregate Root (Test-First)**
   - [ ] UserId strongly-typed identifier
   - [ ] User entity with value object composition
   - [ ] Domain validation rules
   - [ ] Factory method for user creation
   - [ ] Encapsulation of business logic

5. **Repository Interface**
   - [ ] IUserRepository with async methods
   - [ ] Domain-focused methods (FindByEmail, Save, etc.)
   - [ ] No infrastructure concerns

### Phase 3: Application Layer
6. **CQRS Infrastructure**
   - [ ] Command/Query interfaces
   - [ ] Base command handler pattern

7. **User Registration Use Case (Test-First)**
   - [ ] RegisterUser command
   - [ ] RegisterUserHandler with validation
   - [ ] Application service coordination
   - [ ] Integration with repository

### Phase 4: Infrastructure Layer
8. **In-Memory Repository (Test-First)**
   - [ ] Thread-safe implementation
   - [ ] Unique constraint enforcement
   - [ ] Proper async/await patterns

9. **Dependency Injection Setup**
   - [ ] Service registration
   - [ ] Lifetime management
   - [ ] Configuration patterns

### Phase 5: Web API Layer
10. **REST API (Test-First)**
    - [ ] UsersController with registration endpoint
    - [ ] DTO mapping
    - [ ] Error handling middleware
    - [ ] HTTP status code mapping

11. **Integration Testing**
    - [ ] End-to-end registration flow
    - [ ] Error scenarios
    - [ ] API contract validation

### Phase 6: Acceptance Criteria Validation
12. **Comprehensive Testing**
    - [ ] Domain unit tests (>90% coverage)
    - [ ] Application service tests
    - [ ] Infrastructure tests
    - [ ] API integration tests
    - [ ] Performance validation

## Technical Decisions

### Domain Modeling
- **Aggregate Design**: User as aggregate root with value object composition
- **Identity**: Strongly-typed UserId (Guid-based)
- **Validation**: Domain-level validation in value objects and entities
- **Immutability**: Value objects immutable by design

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
- ✅ User can register with email, username, title, first name, last name
- ✅ Email format validation enforced
- ✅ Username uniqueness enforced  
- ✅ PersonalInfo validation (required first/last name, optional title)
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