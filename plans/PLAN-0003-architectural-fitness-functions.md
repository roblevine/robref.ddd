# PLAN-0003: Architectural Fitness Functions

**Status:** COMPLETE
**Started:** September 28, 2025
**Completed:** September 28, 2025  
**Approach:** Test-first verification of architectural constraints using automated fitness functions

## Overview

Introduce a dedicated architectural test suite that enforces our onion architecture and bounded context isolation policies. The suite lives in a new `RobRef.DDD.Architecture.Tests` project and runs automatically with the rest of the solution tests.

## Objectives

- Guard onion dependencies (Domain ← Application ← Infrastructure ← WebApi)
- Ensure bounded contexts do not reference each other directly
- Validate shared CQRS contracts are the only cross-context dependency in the application layer
- Keep the checks lightweight and fast so they run on every build

## Implementation Phases

### Phase 1: Foundation
- [x] Update documentation to describe the architectural fitness strategy
- [x] Mark PLAN-0002 as paused while this work completes
- [x] Create `RobRef.DDD.Architecture.Tests` xUnit project and add to solution
- [x] Reference relevant bounded context assemblies and add `NetArchTest.Rules`
- [x] Baseline solution build to confirm setup

### Phase 2: Core Fitness Functions
- [x] Add tests enforcing onion layer dependency rules for Users context
- [x] Add tests preventing cross-context references outside of shared libs
- [x] Add tests confirming application layer uses shared CQRS interfaces only
- [x] Document test coverage inside the new project README (if needed)

### Phase 3: Validation & Automation
- [x] Run full test suite and ensure architectural checks execute in CI scripts
- [x] Capture decisions and heuristics in `SESSION-NOTES.md`
- [x] Re-enable PLAN-0002 once safeguards are in place

## Risks & Mitigations

- **False positives from assembly scans**: Keep rules explicit and scoped per context; add helper methods as needed.
- **Test brittleness**: Encapsulate type lists in utility classes so project moves require minimal updates.
- **Performance**: Limit reflection to relevant assemblies and run locally before committing.

## Exit Criteria

- Architectural tests fail when intentional dependency violations are introduced
- `dotnet test` runs include the architecture project without significant time penalty
- Documentation and session notes reflect the new safeguards
- PLAN-0002 resumes work with guardrails in place
