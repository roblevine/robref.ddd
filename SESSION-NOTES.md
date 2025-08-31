# Session Notes Log

Purpose: Preserve nuanced conversational intent (rationale, rejected alternatives, heuristics) across stateless chat resets without bloating plan files. Acts as a transient narrative ledger; the authoritative decisions still live in plan + architecture docs.

Conventions:
* One dated section per active working day / session.
* Keep each bullet <= 120 chars; terse & scannable.
* Categories: Decisions, Rationale, Rejected Alternatives, Pending Intents, Heuristics, Bootstrap Snippet.
* When a Decision becomes stable, ensure it is also reflected in the relevant PLAN or ARCHITECTURE file.
* Periodically (e.g. after 5–7 sessions) fold older entries into plan revision history and trim here.

---
## 2025-08-31
Decisions:
* VO shapes: UserId = record struct; others (EmailAddress, Title, FirstName, LastName, HumanName) sealed record classes.
* EmailAddress canonicalization: trim + lowercase; pragmatic validation (single '@', domain has dot, no whitespace).
* Defer email uniqueness, ULID migration, locale, middle name.
* Event timestamp source will be ITimeProvider -> CreatedAt -> UserRegistered.OccurredAt.

Rationale:
* Record struct for id avoids null + reduces allocations; textual VOs rarely hot enough to optimize further.
* Full RFC email validation premature; simpler invariant set suffices short-term.
* Deferring uniqueness keeps slice focused on pure domain modeling.

Rejected Alternatives:
* Full RFC 5322 regex for email — complexity & false negatives vs current needs.
* Using only strings for identifiers — weak invariants & parsing overhead pushed downstream.

Pending Intents:
* Add failing tests for Title next session before implementation.
* Introduce ITimeProvider before User aggregate & event emission slice.
* Document display formatting rules in HumanName when implemented.

Heuristics:
* Normalize early inside VOs; consumers assume canonical forms.
* Add failing tests before each new VO implementation.
* Keep plan slim; SESSION-NOTES carries ephemeral nuance.

Bootstrap Snippet (paste into new chat to restore context):
"We are on PLAN-0001 (Foundational User Domain) status IN PROGRESS. Completed: scaffolding, UserId, EmailAddress (tests green). Next: add failing tests + implement Title, then FirstName/LastName, then HumanName; add ITimeProvider before User aggregate & UserRegistered event. See SESSION-NOTES 2025-08-31 for recent decisions."
