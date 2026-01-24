<!--
Sync Impact Report
==================
Version change: 0.0.0 → 1.0.0
Added sections:
  - Preamble
  - Core Principles (4 principles)
  - Governance
  - Compliance Review
Modified principles: None (initial creation)
Removed sections: None
Templates requiring updates:
  - .specify/templates/plan-template.md ✅ (Constitution Check section added)
  - .specify/templates/spec-template.md ✅ (no changes needed)
  - .specify/templates/tasks-template.md ✅ (no changes needed)
Follow-up TODOs: None
-->

# Edvantix Project Constitution

**Version**: 1.0.0
**Ratification Date**: 2026-01-25
**Last Amended Date**: 2026-01-25

## Preamble

This constitution establishes the non-negotiable principles that govern all development
activities within the Edvantix project. These principles ensure code quality, testing
rigor, user experience consistency, and performance excellence across the microservices
architecture and frontend applications.

All contributors, including AI agents, MUST adhere to these principles when proposing
changes, creating specifications, or implementing features.

---

## Principle 1: Code Quality First

**Statement**: All code MUST meet established quality standards before merging.

**Rules**:
- All code MUST compile without warnings in release mode
- All code MUST pass static analysis (CSharpier for C#, ESLint/Prettier for TypeScript)
- Public APIs MUST have XML documentation (C#) or JSDoc (TypeScript)
- Code MUST follow DDD patterns: aggregates maintain consistency boundaries
- No `any` type in TypeScript; prefer `unknown` with type narrowing
- Nullable reference types MUST be enabled; use `is null` / `is not null` checks
- Maximum cyclomatic complexity: 10 per method
- Maximum method length: 50 lines (excluding documentation)

**Rationale**: Consistent code quality reduces maintenance burden, prevents bugs, and
ensures the codebase remains accessible to all team members.

---

## Principle 2: Test-Driven Development

**Statement**: All features MUST be developed with tests written before implementation.

**Rules**:
- Tests MUST be written before implementation code (TDD)
- Test names MUST follow `GivenCondition_WhenAction_ThenExpectedResult` convention
- Minimum test coverage: 85% for domain and application layers
- Contract tests MUST exist for all API endpoints
- Integration tests MUST validate aggregate boundaries
- Tests MUST NOT depend on external services; use mocks or fakes
- Performance tests MUST validate response time requirements
- All tests MUST pass before any merge to main branch

**Rationale**: Test-driven development ensures requirements are understood before
implementation, reduces defects, and provides living documentation of system behavior.

---

## Principle 3: User Experience Consistency

**Statement**: All user-facing components MUST provide a consistent, accessible experience.

**Rules**:
- UI components MUST follow the established design system
- All interactive elements MUST be keyboard accessible
- WCAG 2.1 AA compliance is REQUIRED for all user interfaces
- Error messages MUST be user-friendly and actionable
- Loading states MUST be indicated for operations exceeding 200ms
- Form validation MUST provide immediate, inline feedback
- Navigation patterns MUST be consistent across all applications
- Responsive design MUST support viewport widths from 320px to 2560px

**Rationale**: Consistent user experience builds trust, reduces learning curves, and
ensures the product is accessible to users with disabilities.

---

## Principle 4: Performance Requirements

**Statement**: All components MUST meet defined performance thresholds.

**Rules**:
- API endpoints MUST respond within 200ms for p95 latency
- Database queries MUST complete within 100ms
- Frontend initial load (LCP) MUST be under 2.5 seconds
- Time to Interactive (TTI) MUST be under 3.5 seconds
- Memory usage MUST remain stable under sustained load (no leaks)
- Microservices MUST handle graceful degradation during dependency failures
- Async operations MUST use `async/await`; blocking calls are prohibited
- Resource-intensive operations MUST be debounced or throttled appropriately

**Rationale**: Performance directly impacts user satisfaction and system reliability.
Defined thresholds provide measurable targets for optimization.

---

## Governance

### Amendment Procedure

1. Propose amendment via pull request to this constitution file
2. Amendment MUST include rationale and impact assessment
3. Team review period: minimum 48 hours
4. Amendment requires approval from at least 2 team members
5. Version number MUST be updated according to semantic versioning:
   - **MAJOR**: Backward-incompatible principle removals or redefinitions
   - **MINOR**: New principle added or material expansion of existing guidance
   - **PATCH**: Clarifications, wording improvements, non-semantic refinements

### Versioning Policy

- Constitution version is independent of application version
- All specification and plan documents MUST reference the constitution version in use
- Breaking changes to principles MUST include migration guidance

### Compliance Review

- Constitution compliance MUST be verified at two gates:
  1. **Initial Gate**: Before research/design phase begins
  2. **Post-Design Gate**: After design is complete, before task generation
- Violations MUST be documented with justification in the Complexity Tracking section
- Unjustified violations MUST block feature progression

---

## Compliance Checklist

Use this checklist in plan documents for the Constitution Check section:

### Code Quality Gate
- [ ] Code compiles without warnings
- [ ] Static analysis passes (CSharpier/ESLint)
- [ ] Public APIs documented
- [ ] DDD patterns followed
- [ ] No TypeScript `any` types
- [ ] Nullable reference types handled

### Testing Gate
- [ ] Tests written before implementation
- [ ] Test naming convention followed
- [ ] Coverage targets met (85% domain/application)
- [ ] Contract tests for APIs
- [ ] Integration tests for aggregates

### UX Consistency Gate
- [ ] Design system followed
- [ ] Keyboard accessibility verified
- [ ] WCAG 2.1 AA compliance checked
- [ ] Error messages user-friendly
- [ ] Loading states implemented
- [ ] Responsive design validated

### Performance Gate
- [ ] API latency under 200ms (p95)
- [ ] Database queries under 100ms
- [ ] LCP under 2.5s
- [ ] TTI under 3.5s
- [ ] No blocking async calls
- [ ] Graceful degradation implemented

---

*This constitution is the authoritative source for project development standards.*
