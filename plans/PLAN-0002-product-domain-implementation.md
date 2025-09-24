# PLAN-0002: Product Domain Implementation

**Status:** PLANNING  
**Started:** September 26, 2025  
**Approach:** Test-First Development, Domain-Driven Design, Onion Architecture, Incremental slices

## Overview

Establish the Product bounded context as an independently deployable service within the RobRef.DDD monorepo. Deliver end-to-end functionality for maintaining a catalogue of financial products, starting with read/write APIs over a simple product model (id, name, description).

## Scope & Boundaries

### ✅ In Scope
- Product aggregate root with value objects for identity and core attributes
- Repository abstraction dedicated to the Product context
- Application use cases for creating, retrieving, and listing products
- Infrastructure persistence (in-memory + EF Core SQL Server) and migrations
- Minimal API exposing product create/query endpoints
- Comprehensive automated tests across all layers

### ❌ Out of Scope (Future Work)
- Internationalisation/localisation of product content
- Pricing, eligibility, or availability rules
- Product selection / user-product associations
- Search/filtering beyond simple list/get operations
- Eventing or integration with external catalogue sources

## High-Level Architecture

```
bounded-contexts/products/
├── src/
│   ├── RobRef.DDD.Products.Domain/
│   ├── RobRef.DDD.Products.Application/
│   ├── RobRef.DDD.Products.Infrastructure/
│   └── RobRef.DDD.Products.WebApi/
└── tests/
    ├── RobRef.DDD.Products.Domain.Tests/
    ├── RobRef.DDD.Products.Application.Tests/
    ├── RobRef.DDD.Products.Infrastructure.Tests/
    └── RobRef.DDD.Products.WebApi.Tests/
```

Each project mirrors the Users bounded context to preserve autonomy and consistency.

## Implementation Phases

### Phase 1: Foundation & Domain Modelling
1. **Scaffold Context**
   - [ ] Create solution `RobRef.DDD.Products.sln` and empty project skeletons
   - [ ] Wire projects into root solution and helper scripts (build/test)
2. **Value Objects & Aggregate**
   - [ ] Implement `ProductId` (ULID), `ProductName`, `ProductDescription`
   - [ ] Implement `Product` aggregate with factory and invariants
   - [ ] Author domain unit tests covering validation and behaviours
3. **Repository Abstraction**
   - [ ] Define `IProductRepository` interface with required operations

### Phase 2: Application Layer
1. **CQRS Contracts**
   - [ ] Reuse/extract common command/query interfaces or replicate within Products
2. **Use Cases**
   - [ ] `CreateProduct` command + handler (duplicate guard, validation)
   - [ ] `GetProductById` query + handler
   - [ ] `ListProducts` query + handler (basic ordering)
3. **Application Service**
   - [ ] `ProductApplicationService` orchestrating repository access
4. **Tests**
   - [ ] Unit tests for handlers with in-memory/dummy repos

### Phase 3: Infrastructure Layer
1. **In-Memory Repository**
   - [ ] Implement thread-safe in-memory repository satisfying interface
   - [ ] Integration tests verifying behaviour
2. **EF Core Persistence**
   - [ ] Add DbContext (`ProductsApplicationDbContext`) + entity configuration
   - [ ] Implement `EfProductRepository`
   - [ ] Create initial migration and ensure schema matches domain constraints
   - [ ] Tests: In-memory provider + SQL Server/Testcontainers (reuse fixtures)
3. **Dependency Injection**
   - [ ] Provide `AddProductsInfrastructureSqlServer` / `AddProductsInfrastructureInMemory`

### Phase 4: Web API Layer
1. **Project Setup**
   - [ ] Configure minimal API host with swagger + problem details
2. **Endpoints**
   - [ ] `POST /api/products` for creation
   - [ ] `GET /api/products/{id}` for single retrieval
   - [ ] `GET /api/products` for listing
3. **Contracts & Validation**
   - [ ] Request/response DTOs, endpoint filters, exception mapping
4. **Integration Tests**
   - [ ] WebApplicationFactory-based tests covering success, validation, duplicate product name (if constrained)
   - [ ] Snapshot swagger test (optional)

### Phase 5: Documentation & Tooling
- [ ] Update README, ARCHITECTURE, TODO with Product context status
- [ ] Extend scripts (`build.sh`, `test.sh`, run scripts) with product-specific options if needed
- [ ] Update SESSION-NOTES with decisions and rationale

## Success Criteria
- Domain invariants enforced via value objects and aggregate tests
- Application handlers validated with comprehensive unit coverage
- EF Core schema aligns with domain constraints; migrations applied cleanly
- Repository tests pass across in-memory and SQL Server providers
- API endpoints deliver expected behaviour with full integration test coverage
- All commands/tests succeed via helper scripts (`./scripts/build.sh`, `./scripts/test.sh`)
- Documentation reflects new bounded context and usage instructions

## Risks & Mitigations
- **Schema divergence:** Mitigate by encoding constraints via value objects and tests before generating migrations.
- **Test duplication:** Evaluate shared testing utilities after first slice; extract only when duplication becomes burdensome.
- **Cross-context coupling:** Maintain strict namespace and project boundaries; avoid shared code without explicit justification.

## Next Steps
- Obtain approval on this plan.
- Once approved, execute Phase 1 tasks starting with project scaffolding and domain modelling.
