# Architecture

This document outlines the architecture of the Acme Store application, including its components, interactions, and deployment strategies.

## High-Level Architecture

We follow Onion Architecture with Domain-Driven Design at the core:

- Core Domain (center)
  - Pure domain model: entities, value objects, aggregates, domain events
  - No external dependencies
- Application (use cases)
  - Orchestrates domain behaviors and transactions
  - Depends inward on domain; exposes ports (interfaces)
- Infrastructure (adapters)
  - Implementations for persistence, messaging, event publishing
  - Depends on application/domain ports
- Presentation (API/UI)
  - Endpoints, controllers, or UI; depends on application layer

All dependencies point inward toward the Domain. The Domain never depends on outer layers.

## Current Solution Structure

- src/AcmeStore.Domain
  - Users/ValueObjects: `UserId`, `Email`, `PersonName`
  - Target framework: net8.0
- tests/AcmeStore.Domain.Tests
  - xUnit test project for the domain
  - Test-first specs for value objects

Planned projects (to be added in PLAN-0001 increments):
- AcmeStore.Application (use cases, ports)
- AcmeStore.Infrastructure.InMemory (in-memory repository/event publisher for tests)
- AcmeStore.Api (HTTP endpoints) [out of scope for PLAN-0001]

## Domain-Driven Design Building Blocks

- Value Objects
  - `UserId` (VO backed by GUID), `Email` (normalized/validated), `PersonName` (title, first, last)
- Aggregates/Entities
  - `User` aggregate (Id, Email, Name, Status) [planned]
- Domain Events
  - `UserRegistered`, `UserEmailChanged`, `UserNameChanged`, `UserDeactivated`, `UserReactivated` [planned]

## Testing Strategy

- Test-first (unit specs) using xUnit
- Domain tests do not rely on infrastructure
- Application tests will use in-memory adapters for fast feedback

## Libraries and frameworks

### DO NOT USE
The following libraries should not be considered as part of this solution
- FluentAssertions