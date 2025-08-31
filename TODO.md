# Introduction

This document outlines the tasks and features to be implemented in the Acme Store application.
It serves as a roadmap for development, helping to track progress and prioritize work.

Items to be implemented will be listed at the high level only here. More detailed plans and tasks will be documented in separate PLAN-*.md files, stored in the "plans" folder. Each plan file will represent a single Analyse -> Plan -> Execute -> Review cycle.

## High-Level Features

### Foundational User Domain (Human Identity)
**Status**: PLANNING
**Plan Document**: [PLAN-0001-foundational-user-domain](plans/PLAN-0001-foundational-user-domain.md)  
**Started**: yes
**Description**: Define VOs (`UserId` backed by GUID, `Email`, `PersonName` with title/first/last), `User` aggregate, events, and app services; test-first with in-memory infra.

Progress update:
- Value objects implemented with tests
- `User` aggregate implemented with domain events and tests
