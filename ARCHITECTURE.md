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

Value Object Representation Guideline (2025-08-31):
| Form | When To Use | Rationale |
|------|-------------|-----------|
| record struct (readonly) | Tiny primitives (identifiers, short numeric/value tokens) unlikely to be optional or used as dictionary keys frequently | Avoid null, reduce allocations, trivially copy by value |
| record class (sealed) | Textual / composite VOs, anything with normalization logic, or likely future extension | Reference semantics avoid large struct copies; easier to evolve; richer validation |

Rules:
1. Default to sealed record class unless the VO is a simple identity wrapper (Guid/Ulid) or very small immutable numeric-like token.
2. Do NOT use plain classes with manual equality unless case-insensitive or custom comparison semantics demand overriding; still declare as record class and override equality if needed.
3. Keep structs readonly; never expose parameterless constructors; provide static factories for invariants.
4. For case-insensitive equality (e.g. `HumanName`) override `Equals`/`GetHashCode` inside the record class rather than abandoning record syntax.

Applied:
* `UserId` → record struct.
* `EmailAddress` → sealed record class (case-sensitive canonical lowercase value).
* `Title`, `FirstName`, `LastName` → sealed record classes with case-insensitive equality & normalization (trim + space collapse).
* `HumanName` → sealed record class composing the above; case-insensitive across parts.

Consistency Decision (2025-08-31): Favor record struct only for identifier (`UserId`) to ensure non-null semantics & value semantics with no heap allocation; use sealed record classes for textual semantic VOs for clarity and reduced default(struct) invalid state risk.

Canonicalization & Creation Pattern (2025-08-31):
* Each textual VO uses a static `Create(...)` that validates + normalizes; optional `TryCreate/TryParse` where helpful.
* Email: trim + lowercase (canonical). Pragmatic validation (single '@', domain has dot, no whitespace).
* Title / FirstName / LastName: trim + collapse consecutive internal spaces; validate allowed Unicode letter/mark + specific punctuation (hyphen, apostrophes, period for Title).
* Case-insensitive equality implemented by overriding `Equals`/`GetHashCode` in each name component.

Deferred Concerns:
- Email uniqueness enforcement
- Authentication credentials & verification
- Locale / preference modeling

Rationale:
- Start with simplest possible identifier and layering, enabling rapid iteration. Migration path documented early to reduce coupling risk.