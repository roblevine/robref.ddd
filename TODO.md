# Introduction

This document outlines the tasks and features to be implemented in the Finman application.
It serves as a roadmap for development, helping to track progress and prioritize work.

Items to be implemented will be listed at the high level only here. More detailed plans and tasks will be documented in separate PLAN-*.md files, stored in the "plans" folder. Each plan file will represent a single Analyse -> Plan -> Execute -> Review cycle.

## High-Level Features

### Foundational User Domain (Human Identity)
**Status**: IN PROGRESS (Value Objects phase: UserId + EmailAddress complete)  
**Plan Document**: [PLAN-0001-foundational-user-domain](plans/PLAN-0001-foundational-user-domain.md)  
**Started**: August 30, 2025  
**Description**: Establish a pure User aggregate (id, email, structured human name, createdAt, UserRegistered event) without auth credentials. Sets baseline for ownership & attribution; defers email uniqueness, auth, and locale to later plans.
