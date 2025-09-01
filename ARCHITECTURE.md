# Architecture

This document outlines the architecture of the RobRef.DDD application, implementing Domain-Driven Design with Onion Architecture.

## High-Level Architecture

### Service Development Principles
- **Domain-Driven Design**: Focus on core domain logic. Implement strictly  by modelling entities, value types, and aggregate routes, etc. Prefer strongly-typed value objects with internal validation over native types.
- **Onion Architecture**: Emphasise separation of concerns, with core domain logic at the center

### Domain Layer Design Principles
- **Value Objects**: Immutable types representing domain concepts
- **Aggregate Roots**: Entities that maintain consistency boundaries
- **Domain Services**: Stateless services for complex domain operations

### Value Object Implementation Strategy
- **record class**: For nullable/optional domain concepts (Email, FirstName, LastName, Title)
- **readonly record struct**: For required/never-null domain concepts (UserId)
- **Immutability**: All value objects immutable - changes create new instances
- **Validation**: Constructor validation with domain-specific rules
- **Equality**: Structural equality based on all properties (automatic with records)
- **Identity**: UserId uses ULID for sortable, time-based unique identifiers (Cysharp library)

## Libraries and frameworks

### DO NOT USE
The following libraries should not be considered as part of this solution
- Fluent Assertions