# Architecture

This document outlines the architecture of the Finman application, including its components, interactions, and deployment strategies.

## High-Level Architecture



## Libraries and frameworks

### DO NOT USE
The follwing libraries should not be considered as part of this solution
- FLuent Assertions

## Domain Project Conventions (User Domain Initial Slice)
Naming:
- Domain library: `src/UserDomain` (pure domain model, no infrastructure dependencies)
- Test project: `tests/UserDomain.Tests`

Identifiers:
- `UserId` will wrap a `Guid` temporarily to keep the foundational slice minimal.
- A later plan will evaluate migrating to ULID for improved chronological ordering properties without database sequences.

Value Objects Introduced:
- `UserId` (Guid wrapper)
- `EmailAddress`
- `Title`, `FirstName`, `LastName`, `HumanName`
- Timestamp handled via an `ITimeProvider` abstraction (UTC).

Deferred Concerns:
- Email uniqueness enforcement
- Authentication credentials & verification
- Locale / preference modeling

Rationale:
- Start with simplest possible identifier and layering, enabling rapid iteration. Migration path documented early to reduce coupling risk.