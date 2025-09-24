# Introduction

This document outlines the tasks and features to be implemented in the RobRef.DDD application.
It serves as a roadmap for development, helping to track progress and prioritize work.

Items to be implemented will be listed at the high level only here. More detailed plans and tasks will be documented in separate PLAN-*.md files, stored in the "plans" folder. Each bounded context (Users, Products, etc.) owns its own plan stream, keeping Analyse → Plan → Execute → Review cycles independent.

## High-Level Features

### Users Bounded Context — Domain Implementation
**Status**: COMPLETE
**Plan Document**: [PLAN-0001-user-domain-implementation](plans/PLAN-0001-user-domain-implementation.md)  
**Started**: August 30, 2025  
**Description**: Implement core User domain with DDD principles, including user registration, value objects (Email, Username, Password), and REST API endpoint. Follows test-first development and onion architecture. 

### Products Bounded Context — Domain Implementation
**Status**: PLANNING
**Plan Document**: PLAN-0002-product-domain-implementation (to be added)  
**Description**: Establish the Product domain as an independently deployable bounded context. Scope to cover core product catalogue modelling and supporting APIs.

## Maintenance Tasks

- Revisit Testcontainers cleanup fallback to see if it can be simplified once Ryuk supports devcontainer DooD reliably
