# PLAN-0001: Foundational User Domain (Human Identity Only)

## Status
- Phase: PLANNING
- Scope agreed with Rob: Yes

## Goal
Introduce the foundational User domain (human identity only) using DDD and Onion Architecture. Establish core value objects, the `User` aggregate, domain events, and minimal application services, implemented test-first. No persistence beyond in-memory implementations. No authentication/authorization.

## Scope
- In scope:
  - Value Objects: `UserId` (VO backed by GUID), `Email` (validated), `PersonName` (title, firstName, lastName)
  - Entity/Aggregate: `User` (Id, Email, PersonName, Status)
  - Domain Events: `UserRegistered`, `UserEmailChanged`, `UserNameChanged`, `UserDeactivated`, `UserReactivated`
  - Application services for registration and profile maintenance
  - In-memory repository and event publisher for testing
- Out of scope (for now):
  - Passwords, authN/authZ, roles/permissions
  - External messaging/event bus integration
  - Database persistence and migrations
  - API/UI layers

## Domain Model
- Value Objects
  - `UserId`
    - Strongly-typed VO, internally backed by a GUID
    - Validates non-empty; provides parse/try-parse and new-id factory
  - `Email`
    - Normalized (e.g., trim, lowercase local/domain)
    - Validated with conservative pattern + length bounds
  - `PersonName`
    - `title` (free text, optional now), `firstName` (required), `lastName` (required)
    - Normalization and basic length/character checks

- Aggregate: `User`
  - State: `UserId Id`, `Email Email`, `PersonName Name`, `UserStatus Status`
  - Invariants:
    - Email required and unique at repository level
    - Name required
    - Status defaults to `Active`; when `Deactivated`, mutating operations are disallowed
  - Behaviors:
    - Register (factory)
    - ChangeEmail (no-op if same normalized email)
    - ChangeName (no-op if equivalent)
    - Deactivate / Reactivate (idempotent)
  - Events: Emitted on each meaningful state change

## Application Layer (Use Cases)
- `RegisterUser(email, personName) -> UserId`
- `ChangeUserEmail(userId, newEmail)`
- `ChangeUserName(userId, newName)`
- `DeactivateUser(userId)` / `ReactivateUser(userId)`

## Interfaces (Ports)
- `IUserRepository`
  - `GetById`, `GetByEmail`, `Add`, `Update`, `ExistsByEmail`
- `IDomainEventPublisher`
  - `Publish(IEnumerable<IDomainEvent> events)` (in-memory stub for now)

## Testing Strategy (Test-First)
- Value object specs: construction, equality, normalization, validation failures
- Aggregate specs: invariants, behaviors, idempotency, and domain event emission
- Application service specs: happy paths, duplicates, invalid input, deactivated state handling
- Use only approved assertion/style libs (avoid FluentAssertions)

## Deliverables
- `Finman.Domain.Users`: VOs, `User` aggregate, domain events
- `Finman.Application.Users`: application services and contracts
- `Finman.Infrastructure.InMemory`: in-memory repository and event publisher for tests
- Updated docs: this plan and `TODO.md`

## Incremental Checklist
- [ ] Define tests for `UserId`, `Email`, `PersonName`
- [ ] Implement `UserId` (VO backed by GUID), `Email`, `PersonName`
- [ ] Define tests for `User` aggregate invariants and events
- [ ] Implement `User` aggregate and events
- [ ] Define `IUserRepository` and `IDomainEventPublisher`
- [ ] Implement in-memory `UserRepository` and `DomainEventPublisher`
- [ ] Tests for `RegisterUser`; implement application service
- [ ] Tests for `ChangeUserEmail`; implement service
- [ ] Tests for `ChangeUserName`; implement service
- [ ] Tests for deactivate/reactivate; implement services
- [ ] Documentation review and updates

## Decisions
- `UserId` is a proper VO, internally backed by a GUID
- `PersonName` includes `title`, `firstName`, `lastName`
- Domain events are raised internally (no external bus yet)
- Use domain exceptions inside domain; application layer can adapt later
- No new dependencies without approval; do not use FluentAssertions