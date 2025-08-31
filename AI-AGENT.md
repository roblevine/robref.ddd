# AI Agent Guidelines

This file provides comprehensive guidance for AI Agents (Claude, GitHub Copilot, Cursor, Windsurf, etc.) working with the Acme Store DDD example platform.

## Documentation Navigation

### Primary Documentation
1. **Project Overview**: Start with [README.md](README.md) for understanding project goals and structure
2. **Technical Details**: Refer to [ARCHITECTURE.md](ARCHITECTURE.md) for design patterns and service structure  
3. **Development Process**: Use [DEVELOPMENT.md](DEVELOPMENT.md) for workflow and operational guidance
4. **Current Work**: Check [TODO.md](TODO.md) for ongoing tasks and project status
5. **[plans/](plans/)**: Detailed document of implementation plans and architectural decision records for major features - as referenced in TODO.md

## Development Methodology

### Core behaviours
- At the beginning of every session, ensure you have read the documents referenced above
- Begin replies with "Hi Rob!". 
- Work in small, test-first increments; discuss before adding dependencies.
- Default mode is Propose-Only: do not make code edits, create files, or run commands without explicit approval.
- Allowed without approval: read-only actions (read/search code/docs, summarize findings, propose todos/plan).
- Implementation occurs only after explicit approval (“Proceed”, “Implement”, “Approved”) from Rob.
- If approval is unclear or not given, do not proceed.
- After approval: restate the plan (edits, tests, commands), execute, and report results.

### Core Workflow: Analyse → Plan → Execute → Review
1. **Analyse**: Break down requirements, understand existing codebase context
2. **Plan**: Document approach (create PLAN-*.md for major features)  
3. **Execute**: Implement in small, testable increments with comprehensive tests
4. **Review**: Verify functionality, update documentation, ensure no regressions

### Service Development Principles
- **Domain-Driven Design**: Focus on core domain logic. Implement strictly  by modelling entities, value types, and aggregate routes, etc. Prefer strongly-typed value objects with internal validation over native types.
- **Onion Architecture**: Emphasise separation of concerns, with core domain logic at the center

### Code Quality Standards
- **Test-First Development**: Write tests before implementing functionality (non-negotiable)
- **SOLID Principles**: Clear responsibilities, dependency inversion, clean interfaces
- **Meaningful Names**: Self-documenting code with clear variable and function names
- **Documentation Currency**: Update docs with any architectural or API changes

### Working Agreements
- **Dependency Approval**: Always discuss before adding new packages or frameworks
- **Clarification First**: Ask questions when multiple approaches are possible
- **Incremental Delivery**: Deliver features in small, reviewable, testable slices
- **Consistency**: Maintain existing patterns and coding styles throughout the codebase
- **Keeps documentation up to date**: Ensure all changes are reflected in relevant documentation, but always seek approval before making changes