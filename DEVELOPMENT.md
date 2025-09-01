# Development Guide

This document provides comprehensive development guidance for the RobRef.DDD application monorepo, including setup, workflow, and service development patterns.

## Development Workflow

### Methodology
We follow a **Test-First Analyse → Plan → Execute → Review** methodology:

1. **Analyse**: Understand requirements and break down problems
2. **Plan**: Document approach in PLAN-*.md files for major features
3. **Execute**: Implement in small, testable increments
4. **Review**: Verify functionality and update documentation

### Ways of working
- **Conventional Commits**: Use conventional commit messages for all changes (e.g. feat, fix, docs, chore)

### Code Quality Standards
- **Test-First Development**: Write tests before implementing functionality (non-negotiable)
- **SOLID Principles**: Clear responsibilities, dependency inversion, clean interfaces
- **Meaningful Names**: Self-documenting code with clear variable and function names
- **Documentation Currency**: Update docs with any architectural or API changes