# PLAN-0001: Foundational User Domain (Human Identity Only)

Status: DRAFT  
Started: August 30, 2025  
Approach: Analyse → Plan → Execute → Review (Test-First, DDD, Onion/Hexagonal principles)

## 1. Context
Original concept mixed authentication (password hashing) with core domain identity. Decision: separate concerns. This plan establishes a pure domain representation of a human User (ownership & attribution anchor) without authentication secrets. Authentication / credentials will live in a separate bounded context (e.g. Auth / Identity Provider) and relate via `UserId`.

## 2. Goals / Objectives
Deliver a minimal, high‑quality User aggregate capturing stable personal identity attributes:
* Unique identifier
* Email (for contact & uniqueness)
* Structured human name (title, first name, last name)
* Creation timestamp
* Domain event on creation (`UserRegistered`)

Exclude passwords, hashes, login flows, verification steps.

## 3. Scope
In Scope:
* Value Objects: `UserId`, `EmailAddress`, `Title` (optional), `FirstName`, `LastName`, `HumanName` (composite), `CreatedAt` (wrapper or direct UTC instant)
* Aggregate: `User`
* Factory / Command semantics: `RegisterUser` (create)
* Domain Event: `UserRegistered`
* Interfaces: `IUserRepository`, optional `ITimeProvider`
* In-memory fakes for tests

Out of Scope (future plans):
* Authentication credentials (passwords, external IdPs, tokens)
* Email verification workflow
* Profile enrichment (avatars, preferences)
* Persistence / database schema
* Messaging infrastructure integration

## 4. Ubiquitous Language
| Term | Definition |
|------|------------|
| User | A human actor owning financial entities in the system |
| Registration | Creation of a new User aggregate with validated attributes |
| Human Name | Structured name components (Title?, FirstName, LastName) |
| Title | Optional honorific (free-text, lightly validated) |

## 5. Domain Model
Value Objects (summary):
* `UserId`: readonly record struct wrapping a Guid (temporary; future ULID migration planable).
* `EmailAddress`: sealed record class; lowercased & trimmed canonical form; RFC5322-light validation.
* `Title`: sealed record class; optional, free-text (1–30 chars), trimmed, disallow digits/control chars.
* `FirstName` / `LastName`: sealed record classes; 1–100 chars, Unicode letters plus hyphen & apostrophe; trimmed.
* `HumanName`: sealed record class composing Title (nullable), FirstName, LastName; provides `Display()`.
* `CreatedAt`: UTC instant (may use `DateTimeOffset` or wrapper `UtcTimestamp`).

Aggregate `User`:
* Fields: id, email, humanName, createdAt
* Invariants:
	- Email required (uniqueness deferred)
	- First & last name required (title optional)
	- createdAt set via time provider (or system UTC fallback) at creation
* Behavior:
	- Static factory: `User Register(EmailAddress email, HumanName name, ITimeProvider clock)`
	- Emits `UserRegistered`

Domain Event `UserRegistered`:
* Data: userId, email, firstName, lastName, title?, occurredAt

## 6. Architectural Decisions
* No password or auth data in User aggregate (separation of bounded contexts: User vs Auth).
* GUID chosen initially (simplicity, zero additional code). Future plan will consider ULID migration for chronological sort benefits.
* Time abstraction (`ITimeProvider.UtcNow`) to make timestamps deterministic in tests.
* Keep validation logic inside value object constructors / factories for strong invariants.
* Avoid external dependencies initially (lightweight regex only). Can revisit for email or ULID libs later with explicit approval.

## 7. Incremental Delivery Slices (Trackable)
- [x] 1. Project scaffolding (domain library + test project) & failing smoke test
- [x] 2. Implement `UserId` + tests
- [ ] 3. Implement `EmailAddress` + tests
- [ ] 4. Implement name components (`Title`, `FirstName`, `LastName`) + composite `HumanName` + tests
- [ ] 5. Introduce `ITimeProvider` + simple implementation + tests
- [ ] 6. Implement `User` factory with invariants + tests (no event yet)
- [ ] 7. Add `UserRegistered` event + emission test
- [ ] 8. Refactor & documentation polish; mark plan COMPLETE

## 8. Test Plan (Initial List)
Value Objects:
- [ ] EmailAddress_AcceptsValidSamples
- [ ] EmailAddress_NormalizesToLowerCase
- [ ] EmailAddress_RejectsInvalidFormats
- [x] UserId_GeneratesNewGuid
- [x] UserId_RejectsInvalidGuidString
- [ ] FirstName_RejectsEmptyOrTooLong
- [ ] LastName_RejectsEmptyOrTooLong
- [ ] Title_AcceptsAllowedValuesOrNull
- [ ] Title_RejectsDisallowedValue
- [ ] HumanName_DisplayFormatsCorrectly (with & without title)

Aggregate:
- [ ] RegisterUser_Succeeds_WithValidData
- [ ] RegisterUser_Emits_UserRegistered_Event
- [ ] RegisterUser_Sets_CreatedAt_From_TimeProvider

Event:
- [ ] UserRegistered_ContainsExpectedData

Support:
- (None in this slice; uniqueness deferred)

## 9. Open Questions & Decisions
1. Title enumeration? DECISION: Free-text (trimmed) with validation: 1–30 chars, disallow digits/control chars; optional. No fixed list.
2. Middle name support now? DECISION: No; defer (not modeled in this slice).
3. Email uniqueness enforcement? DECISION: Deferred; accept potential duplicates short-term (documented risk) to keep slice minimal.
4. Locale / language preferences capture? DECISION: No; defer.

## 10. Acceptance Criteria
* All tests in section 8 implemented & passing.
* No password or auth logic present in domain layer.
* Plan updated with resolutions to open questions (or explicitly deferred) before marking COMPLETE.
* `TODO.md` updated: status IN PROGRESS on start, COMPLETE on finish.

## 11. Definition of Done
* Code merged with green test suite.
* Documentation reflects separation of User vs Auth contexts.
* Plan file status changed from DRAFT to COMPLETE with decision notes.

---
Revision History:
* 2025-08-30: Rewritten to remove password/auth concerns and add structured human name fields.
* 2025-08-30: Adjusted scope (removed email uniqueness for this slice) & synchronized tests.
* 2025-08-30: Added checkbox task tracking for slices & tests.
* 2025-08-31: Decided VO forms: UserId as record struct, other VOs as sealed record classes.

## 12. Task Checklist (Track Progress)

Legend: [ ] Pending  [x] Done

Planning & Documentation
- [x] Create foundational user domain plan file
- [x] Align TODO entry & feature status (IN PROGRESS)
- [x] Document GUID temporary decision & future ULID migration
- [ ] Update plan to COMPLETE (after implementation & test pass)

Infrastructure / Scaffolding
- [ ] Create domain project `src/UserDomain`
- [ ] Create test project `tests/UserDomain.Tests`
- [ ] Add base test infrastructure (test runner config)
- [ ] Commit initial failing smoke test

Value Objects
- [x] Implement `UserId` (GUID wrapper)
- [x] Tests: UserId_GeneratesNewGuid & UserId_RejectsInvalidGuidString
- [ ] Implement `EmailAddress`
- [ ] Tests: Accepts / Normalizes / Rejects invalid
- [ ] Implement `Title`
- [ ] Tests: Accepts allowed / Rejects disallowed
- [ ] Implement `FirstName`
- [ ] Implement `LastName`
- [ ] Tests: Name length & validation
- [ ] Implement `HumanName` composite + Display()
- [ ] Tests: HumanName display formatting
- [ ] Implement `ITimeProvider` abstraction + simple system implementation
- [ ] Tests: Time provider used in creation

Aggregate & Events
- [ ] Implement `User` aggregate factory
- [ ] Emit `UserRegistered` event
- [ ] Tests: RegisterUser_Succeeds_WithValidData
- [ ] Tests: RegisterUser_Emits_UserRegistered_Event
- [ ] Tests: RegisterUser_Sets_CreatedAt_From_TimeProvider
- [ ] Tests: UserRegistered_ContainsExpectedData

Refinement
- [ ] Code cleanup / refactor pass
- [ ] Update plan revision history & mark COMPLETE
- [ ] Update TODO status to COMPLETE
