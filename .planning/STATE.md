# Project State

## Project Reference

See: .planning/PROJECT.md (updated 2026-03-18)

**Core value:** Менеджер школы может создать расписание группы, студент видит свои уроки и баланс, учитель видит свои занятия — всё в рамках одной школы с изолированными данными.
**Current focus:** Phase 1 — Organizations RBAC Core

## Current Position

Phase: 1 of 5 (Organizations — RBAC Core)
Plan: 0 of 4 in current phase
Status: Ready to plan
Last activity: 2026-03-18 — Roadmap created; 26 requirements mapped across 5 phases

Progress: [░░░░░░░░░░] 0%

## Performance Metrics

**Velocity:**
- Total plans completed: 0
- Average duration: -
- Total execution time: 0 hours

**By Phase:**

| Phase | Plans | Total | Avg/Plan |
|-------|-------|-------|----------|
| - | - | - | - |

**Recent Trend:**
- Last 5 plans: -
- Trend: -

*Updated after each plan completion*

## Accumulated Context

### Decisions

Decisions are logged in PROJECT.md Key Decisions table.
Recent decisions affecting current work:

- [Roadmap]: Organizations split into two phases — Phase 1 (RBAC domain + tenant isolation) and Phase 2 (gRPC + permission cache). This ensures ITenantContext and the architecture test are the very first deliverable, with the caching layer following once the domain model is stable.
- [Roadmap]: Scheduling split into two phases — Phase 3 (slots + groups + views) and Phase 4 (attendance + outbox). The outbox event contract for Payments must be finalized before Phase 5 begins.
- [Research flag]: Group ownership confirmed in Scheduling (not Organizations) — groups are scheduling constructs, not org-structure constructs. Confirm before Phase 3 schema design.
- [Research flag]: Permission string registry validation mechanism (how AssignPermissionsToRole validates strings) needs design decision in Phase 1 before Phase 3 and Phase 5 define their permission catalogues.

### Pending Todos

None yet.

### Blockers/Concerns

- [Pre-Phase 1]: Multi-school user token model — v1 assumes one tenant_id per JWT claim. If v1.x allows school switching, Phase 1 schema must be forward-compatible. Flag for Phase 1 plan.

## Session Continuity

Last session: 2026-03-18
Stopped at: Roadmap created and written to disk. STATE.md and REQUIREMENTS.md traceability updated. Ready to run /gsd:plan-phase 1.
Resume file: None
