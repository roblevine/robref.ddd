# Architecture

This document outlines the architecture of the Acme Store application, including its components, interactions, and deployment strategies.

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
- `UserId` (readonly record struct) wrapping Guid
- `EmailAddress` (sealed record class)
- `Title`, `FirstName`, `LastName` (sealed record classes)
- `HumanName` (sealed record class aggregating the above)
- Timestamp handled via an `ITimeProvider` abstraction (UTC)

Consistency Decision (2025-08-31): Favor record struct only for identifier (`UserId`) to ensure non-null semantics & value semantics with no heap allocation; use sealed record classes for textual semantic VOs for clarity and reduced default(struct) invalid state risk.

Canonicalization & Creation Pattern (2025-08-31):
- Each textual VO uses a static `Create(string raw)` that validates + normalizes; optional `TryParse` where external parsing is useful (EmailAddress).
- Email normalization: trim + lowercase; pragmatic validation (single '@', domain contains dot, no whitespace). Full RFC deferred until needed.
- Other name components: trim; validation to restrict length & disallow invalid characters (to be implemented in subsequent slices).

Deferred Concerns:
- Email uniqueness enforcement
- Authentication credentials & verification
- Locale / preference modeling

Rationale:
- Start with simplest possible identifier and layering, enabling rapid iteration. Migration path documented early to reduce coupling risk.