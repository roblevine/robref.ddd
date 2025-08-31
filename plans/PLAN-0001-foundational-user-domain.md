# PLAN-0001: Foundational User Domain (Human Identity Only)

Status: IN PROGRESS  
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
* `Title`: sealed record class; optional, free-text (1–30 chars), trimmed & internal space collapse; allowed chars: letters, space, hyphen, apostrophes, period; disallow digits/symbols/control.
* `FirstName` / `LastName`: sealed record classes; 1–100 chars, Unicode letters + combining marks, internal spaces (collapsed), hyphen, apostrophes.
* `HumanName`: sealed record class composing `Title?`, `FirstName`, `LastName`; case-insensitive equality across parts; display formatting.
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
High‑level slice order (may adjust as insights emerge):
1. Project scaffolding (domain library + test project) & failing smoke test  ✅
2. Implement `UserId` + tests  ✅
3. Implement `EmailAddress` + tests  ✅
4. Implement `Title`, `FirstName`, `LastName` VOs + tests  ✅
5. Implement `HumanName` composite (wrapping the three) + tests  ✅
6. Introduce `ITimeProvider` abstraction + tests  🚧 (next)
7. Implement `User` aggregate registration factory + domain event emission + tests
8. Polish / documentation pass & plan completion

Value Object Test Checklist:
✔ UserId_GeneratesNewGuid
✔ UserId_RejectsInvalidGuidString
✔ EmailAddress_AcceptsValidSamples
✔ EmailAddress_NormalizesToLowerCase
✔ EmailAddress_RejectsInvalidFormats
✔ Title_AcceptsValidSamples
✔ Title_RejectsInvalidValues
✔ FirstName_AcceptsValidSamples
✔ FirstName_RejectsInvalidValues
✔ LastName_AcceptsValidSamples
✔ LastName_RejectsInvalidValues
✔ HumanName_AcceptsValidBasicName
✔ HumanName_AcceptsTitleOptional
✔ HumanName_AcceptsHyphenApostropheAccents
✔ HumanName_RejectsInvalidCharacters
✔ HumanName_TrimsAndCollapsesWhitespace
✔ HumanName_Equality_IgnoresCase

Aggregate Registration:
⬜ RegisterUser_Succeeds_WithValidData
⬜ RegisterUser_Emits_UserRegistered_Event
⬜ RegisterUser_Sets_CreatedAt_From_TimeProvider

Event:
⬜ UserRegistered_ContainsExpectedData

Support / Infrastructure (deferred where noted):
* Email uniqueness enforcement – deferred
* Persistence abstraction – deferred

HumanName & Component Specification (2025‑08‑31):
Title:
* Optional free‑text; trim + collapse spaces; empty => null (absence)
* Length: 1–30
* Allowed: letters, space (internal), hyphen, apostrophe (' or ’), period
* Disallowed: digits, other punctuation, control chars
* Equality: case-insensitive

FirstName / LastName:
* Required
* Length: 1–100
* Normalization: trim + collapse internal spaces
* Allowed: Unicode letters + combining marks, internal spaces, hyphen, apostrophes (' or ’)
* Disallowed: digits, other punctuation, control chars
* Equality: case-insensitive

HumanName:
* Composition: `Title?`, `FirstName`, `LastName`
* Equality: case-insensitive across parts
* Display: `"Title FirstName LastName"` or `"FirstName LastName"`
* Apostrophes preserved, no canonicalization
* Deferred: middle names, suffixes, locale particles, mononyms, apostrophe normalization, configurable constraints

Rationale Summary:
* Case‑insensitive equality reduces duplicate logical identities differing only in casing while preserving original display casing.
* Avoid premature locale/title enumeration complexity; free‑text Title lowers friction while still bounded by validation.

Risks / Mitigations:
* Diverse global name formats – mitigated by deferring advanced parsing & focusing on minimally lossy representation.
* Potential need for mononym users – will revisit before first real user ingestion source integration.

Current Slice Status: Name component & HumanName VOs complete (tests green); next slice: introduce time provider.

## 9. Open Questions & Decisions
1. Title enumeration? DECISION: Free-text (trimmed) with validation: 1–30 chars, disallow digits/control chars; optional. No fixed list.
2. Middle name support now? DECISION: No; defer (not modeled in this slice).
3. Email uniqueness enforcement? DECISION: Deferred; accept potential duplicates short-term (documented risk) to keep slice minimal.
4. Locale / language preferences capture? DECISION: No; defer.
5. Email normalization & validation scope? DECISION (2025-08-31): Trim + lowercase canonicalization; pragmatic validation (single '@', domain contains dot, no whitespace). Full RFC compliance deferred until a concrete need (avoid premature complexity).
6. Event timestamp source? DECISION (planned): `UserRegistered.OccurredAt` will equal aggregate `CreatedAt` captured from `ITimeProvider` during registration.

## 10. Acceptance Criteria
* All tests in Section 7 checklist implemented & passing.
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
* 2025-08-31: Implemented EmailAddress VO & tests (slice 7.3) – normalized lower + trim, pragmatic validation.

## 12. Task Checklist (Meta Only)

Legend: [ ] Pending  [x] Done

Planning & Documentation Meta
- [x] Create foundational user domain plan file
- [x] Align TODO entry & feature status (IN PROGRESS)
- [x] Document GUID temporary decision & future ULID migration
- [x] Record EmailAddress implementation & decision details
- [ ] Update plan to COMPLETE (after all domain components & tests pass)

Execution Progress Source of Truth: See Section 7 (Incremental Delivery Slices).
