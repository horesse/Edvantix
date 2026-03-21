---
gsd_state_version: 1.0
milestone: v1.0
milestone_name: milestone
status: unknown
stopped_at: Completed 03-08-PLAN.md
last_updated: "2026-03-21T12:12:19.460Z"
progress:
  total_phases: 5
  completed_phases: 2
  total_plans: 16
  completed_plans: 12
---

# Project State

## Project Reference

See: .planning/PROJECT.md (updated 2026-03-18)

**Core value:** Менеджер школы может создать расписание группы, студент видит свои уроки и баланс, учитель видит свои занятия — всё в рамках одной школы с изолированными данными.
**Current focus:** Phase 03 — scheduling-slots-and-views

## Current Position

Phase: 03 (scheduling-slots-and-views) — EXECUTING
Plan: 6 of 9

## Performance Metrics

**Velocity:**

- Total plans completed: 3
- Average duration: 10 min
- Total execution time: 0.42 hours

**By Phase:**

| Phase | Plans | Total | Avg/Plan |
|-------|-------|-------|----------|
| - | - | - | - |

**Recent Trend:**

- Last 5 plans: -
- Trend: -

*Updated after each plan completion*
| Phase 01-organizations-rbac-core P01 | 15 | 2 tasks | 38 files |
| Phase 01-organizations-rbac-core P02 | 5 | 2 tasks | 15 files |
| Phase 01-organizations-rbac-core P03 | 12 | 2 tasks | 21 files |
| Phase 01-organizations-rbac-core P04 | 9 | 2 tasks | 21 files |
| Phase 02-organizations-permission-cache P01 | 9 | 2 tasks | 19 files |
| Phase 02-organizations-permission-cache P03 | 6 | 1 tasks | 7 files |
| Phase 02-organizations-permission-cache P02 | 8 | 2 tasks | 17 files |
| Phase 03-scheduling-slots-and-views P07 | 2 | 2 tasks | 5 files |
| Phase 03-scheduling-slots-and-views P01 | 18 | 2 tasks | 21 files |
| Phase 03-scheduling-slots-and-views P02 | 13 | 2 tasks | 19 files |
| Phase 03-scheduling-slots-and-views P04 | 2 | 2 tasks | 13 files |
| Phase 03-scheduling-slots-and-views P08 | 8 | 2 tasks | 23 files |

## Accumulated Context

### Decisions

Decisions are logged in PROJECT.md Key Decisions table.
Recent decisions affecting current work:

- [Roadmap]: Organizations split into two phases — Phase 1 (RBAC domain + tenant isolation) and Phase 2 (gRPC + permission cache). This ensures ITenantContext and the architecture test are the very first deliverable, with the caching layer following once the domain model is stable.
- [Roadmap]: Scheduling split into two phases — Phase 3 (slots + groups + views) and Phase 4 (attendance + outbox). The outbox event contract for Payments must be finalized before Phase 5 begins.
- [Research flag]: Group ownership confirmed in Scheduling (not Organizations) — groups are scheduling constructs, not org-structure constructs. Confirm before Phase 3 schema design.
- [Research flag]: Permission string registry validation mechanism (how AssignPermissionsToRole validates strings) needs design decision in Phase 1 before Phase 3 and Phase 5 define their permission catalogues.
- [01-01]: EF Core HasQueryFilter for tenanted entities set in DbContext.OnModelCreating (not IEntityTypeConfiguration) to enable ITenantContext injection.
- [01-01]: Role HasQueryFilter combines tenant AND soft-delete in single expression — EF Core only allows one HasQueryFilter per entity.
- [01-01]: ITenanted lives in Organizations domain (not Chassis) — moved to SharedKernel if Scheduling needs it.
- [01-01]: TenantMiddleware called BEFORE UseAuthorization in pipeline so tenant context is available in authorization handlers.
- [01-02]: Guard.Against.NullOrWhiteSpace and Guard.Against.Default added to Chassis (not Ardalis) — consistent with project's custom Guard pattern using C# 13 extension blocks.
- [01-02]: RoleRepository.FindByIdAsync eagerly loads Permissions navigation — required for AssignPermission/RemovePermission operations without extra roundtrips.
- [01-02]: PermissionRepository.UpsertAsync materialises names to List before Except to avoid multiple enumeration.
- [01-02]: Permission validation via IPermissionRepository.UpsertAsync/GetByNamesAsync provides the registry validation mechanism for AssignPermissionsToRole (resolves research flag).
- [Phase 01-03]: ICommand<Unit>/ICommandHandler<TCommand, Unit> used for void commands — Mediator library only provides ICommand<T>
- [Phase 01-03]: GetRolePermissions fetches all permissions and filters in-memory — acceptable for v1 given small Permission catalogue size
- [Phase 01-03]: Domain aggregate namespaces added to GlobalUsings.cs for project-wide availability without per-file usings
- [Phase 01-04]: PermissionSeeder uses IHostedService with direct repository access (not HTTP) to avoid self-calling race condition on startup
- [Phase 01-04]: RegisterPermissionsEndpoint uses AllowAnonymous for service-to-service startup calls; TODO secured via mTLS in production
- [Phase 01-04]: gRPC client pattern: copy proto, wrap in IPersonaProfileService interface; AssignRoleCommand validates role (local DB) before profile (gRPC) to fail fast
- [Phase 02-01]: gRPC csharp_namespace set to Edvantix.Organizations.Grpc.Generated to avoid class name conflict with concrete implementation
- [Phase 02-01]: GetBySchoolAsync added to IRoleRepository using IgnoreQueryFilters() + explicit schoolId — gRPC paths lack ITenantContext ambient value
- [Phase 02-01]: GetUserPermissionGrantQuery (not CheckPermissionQuery) to comply with arch rule requiring Get/List/Visualize/Summarize prefix on IQuery types
- [Phase 02-03]: Proto-in-Chassis to avoid CS0436: permissions.proto lives in Chassis with GrpcServices=Both; Organizations removes its own Protobuf entry to prevent duplicate type conflicts in same csharp_namespace
- [Phase 02-03]: PermissionRequirementHandler singleton uses IHttpContextAccessor to bridge to scoped ITenantContext — canonical pattern for singleton-to-scoped access in ASP.NET Core
- [Phase 02-02]: Domain events placed in Infrastructure/EventServices/Events/ per plan spec; they pass arch tests because they contain only Guid fields with no infrastructure imports
- [Phase 02-02]: Mediator source generator MSG0005 requires all INotification types to have handlers — added domain event handlers in Domain/EventHandlers/ following Persona pattern
- [Phase 02-02]: InternalsVisibleTo added to Edvantix.Organizations.csproj for EventMapper unit test access — follows Blog/Scheduler pattern
- [Phase 02-02]: GetAllByRoleIdAsync uses IgnoreQueryFilters() for cross-tenant role enumeration in AssignPermissionsCommand cache invalidation
- [Phase 02-02]: Null UserId in UserPermissionsInvalidatedIntegrationEvent signals school-wide invalidation for RolePermissionsChangedEvent mapping
- [Phase 03-07]: Organization does NOT implement ITenanted — it IS the tenant root per D-17. No HasQueryFilter needed.
- [Phase 03-07]: Organization IDs use ValueGeneratedNever — IDs come from external provisioning, not DB auto-generation.
- [Phase 03-07]: GroupsPermissions uses groups.* namespace (not organizations.*) to distinguish group-management from RBAC-management permissions.
- [Phase 03-01]: PermissionSeeder uses IHttpClientFactory HTTP POST to Organizations with AddStandardResilienceHandler — Scheduling does not own Permission aggregate
- [Phase 03-01]: SchedulingDbContext shell has no DbSets — domain entities and HasQueryFilter deferred to Plan 02 when LessonSlot entity is created
- [Phase 03-02]: ITenanted promoted to Edvantix.SharedKernel.SeedWork (from Organizations.Domain.Abstractions) — Scheduling needs tenant isolation without Organizations dependency. Organizations keeps backward-compat via global using alias.
- [Phase 03-02]: LessonSlotRepository.HasConflictAsync uses IgnoreQueryFilters for cross-tenant teacher conflict check (D-04) — teacher cannot be double-booked even across schools
- [Phase 03-04]: OrganizationsGroupService uses HTTP GET /v1/groups/{id} as v1 fallback — gRPC GetGroupAsync not yet available; IOrganizationsGroupService interface is stable for Plan 03-09 swap
- [Phase 03-04]: EditLessonSlotCommandHandler calls ChangeTeacher/Reschedule only when values differ — avoids spurious domain events in Phase 4
- [Phase 03-08]: Organizations uses AddPermissionAuthorization self-call so GroupsPermissions policies enforce RBAC via gRPC without duplicating authorization logic
- [Phase 03-08]: IGroupRepository.Add is synchronous (void) — Group has no navigation properties at creation, simpler than IRoleRepository.AddAsync

### Pending Todos

None yet.

### Blockers/Concerns

- [Pre-Phase 1]: Multi-school user token model — v1 assumes one tenant_id per JWT claim. If v1.x allows school switching, Phase 1 schema must be forward-compatible. Flag for Phase 1 plan.

## Session Continuity

Last session: 2026-03-21T12:12:19.458Z
Stopped at: Completed 03-08-PLAN.md
Resume file: None
