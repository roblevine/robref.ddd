# Architecture

This document outlines the architecture of the RobRef.DDD application, implementing Domain-Driven Design with Onion Architecture.

## High-Level Architecture

### Domain Layer Design Principles
- **Value Objects**: Immutable types representing domain concepts
- **Aggregate Roots**: Entities that maintain consistency boundaries
- **Domain Services**: Stateless services for complex domain operations

### Value Object Implementation Strategy
- **record class**: For nullable/optional domain concepts (Email, FirstName, LastName, Title, PersonalInfo)
- **readonly record struct**: For required/never-null domain concepts (Username, UserId)
- **Immutability**: All value objects immutable - changes create new instances
- **Validation**: Constructor validation with domain-specific rules
- **Equality**: Structural equality based on all properties (automatic with records)

## Libraries and frameworks

### DO NOT USE
The following libraries should not be considered as part of this solution
- Fluent Assertions