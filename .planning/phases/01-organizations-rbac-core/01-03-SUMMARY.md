---
phase: 01-organizations-rbac-core
plan: 03
subsystem: api
tags: [mediator, cqrs, minimal-api, tunit, moq, role-management, permissions, tenant-context]

# Dependency graph
requires:
  - phase: 01-02
    provides: "Role/Permission/UserRoleAssignment aggregates, IRoleRepository, IPermissionRepository, IUserRoleAssignmentRepository, Guard validation"

provides:
  - "POST /v1/roles — CreateRole endpoint with ITenantContext scoping"
  - "GET /v1/roles — GetRoles endpoint returning tenant-scoped role list"
  - "GET /v1/roles/{id} — GetRoleById endpoint"
  - "PUT /v1/roles/{id} — UpdateRole endpoint for role name changes"
  - "DELETE /v1/roles/{id} — DeleteRole endpoint with 409 guard for active assignments"
  - "PUT /v1/roles/{id}/permissions — AssignPermissions endpoint with catalogue validation"
  - "GET /v1/roles/{id}/permissions — GetRolePermissions endpoint"
  - "9 handler unit tests covering create, delete (409 guard), and assign-permissions (valid, invalid, empty, not-found)"

affects:
  - "01-04 (UserRoleAssignment features build on same endpoint/handler pattern)"

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "ICommand<Unit>/ICommandHandler<TCommand, Unit> for void commands (Mediator library pattern)"
    - "IEndpoint<TResult, TRequest1, TRequest2> for endpoints that merge route + body params"
    - "Inline request body record (e.g., UpdateRoleRequest, AssignPermissionsRequest) co-located with endpoint"
    - "NotFoundException.For<T>(Guid) for typed 404 responses"
    - "GlobalExceptionHandler maps InvalidOperationException to 409 Conflict"

key-files:
  created:
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/CreateRole/CreateRoleCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/CreateRole/CreateRoleEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/CreateRole/CreateRoleValidator.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/GetRoles/GetRolesQuery.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/GetRoles/GetRolesEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/GetRoleById/GetRoleByIdQuery.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/GetRoleById/GetRoleByIdEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/UpdateRole/UpdateRoleCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/UpdateRole/UpdateRoleEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/UpdateRole/UpdateRoleValidator.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/DeleteRole/DeleteRoleCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/DeleteRole/DeleteRoleEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/AssignPermissions/AssignPermissionsCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/AssignPermissions/AssignPermissionsEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/AssignPermissions/AssignPermissionsValidator.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/GetRolePermissions/GetRolePermissionsQuery.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/GetRolePermissions/GetRolePermissionsEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/CreateRoleCommandHandlerTests.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/DeleteRoleCommandHandlerTests.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/AssignPermissionsCommandHandlerTests.cs
  modified:
    - src/Services/Organizations/Edvantix.Organizations/GlobalUsings.cs

key-decisions:
  - "ICommand<Unit>/ICommandHandler<TCommand, Unit> used for void commands — Mediator library only has ICommand<T>, no void variant"
  - "AddAsync mock sets Id in unit tests to simulate EF Core key generation that occurs before SaveChanges"
  - "GetRolePermissions fetches all catalogue permissions and filters in-memory — acceptable for v1 since Permission catalogue is small"
  - "Domain aggregate namespaces added to GlobalUsings.cs so all feature files can reference Role, Permission, IUserRoleAssignmentRepository without explicit usings"

patterns-established:
  - "Endpoint pattern: IEndpoint<TResult, TCommand, ISender> with HandleAsync merging route id + body into command"
  - "Inline request record co-located in endpoint file (e.g., UpdateRoleRequest, AssignPermissionsRequest)"
  - "Void commands: ICommand<Unit> + ICommandHandler<TCommand, Unit> + return default"
  - "NotFoundException.For<T>(id) used consistently for all 404 cases"

requirements-completed:
  - ORG-01
  - ORG-02
  - ORG-03

# Metrics
duration: 12min
completed: 2026-03-19
---

# Phase 01 Plan 03: Role Management API Summary

**Seven role management endpoints (CRUD + permission assignment) with Mediator command/query pattern, permission catalogue validation, 409 guard on delete, and 9 unit tests**

## Performance

- **Duration:** 12 min
- **Started:** 2026-03-18T21:51:16Z
- **Completed:** 2026-03-19T22:03:00Z
- **Tasks:** 2
- **Files modified:** 21

## Accomplishments

- Seven working API endpoints under `/v1/roles`: POST, GET (list), GET (by id), PUT, DELETE, PUT permissions, GET permissions
- DELETE /roles/{id} returns 409 Conflict when the role has active `UserRoleAssignment` records
- AssignPermissions validates all provided permission names against the global `Permission` catalogue before persisting
- 9 unit tests covering CreateRole handler (2), DeleteRole handler (3), and AssignPermissions handler (4)
- All 33 unit tests and 64 architecture tests pass

## Task Commits

Each task was committed atomically:

1. **Task 1: Role CRUD features with unit tests** - `2e8f928` (feat)
2. **Task 2: AssignPermissions and GetRolePermissions features** - `1f02f3e` (feat)

## Files Created/Modified

- `Features/Roles/CreateRole/CreateRoleCommand.cs` - Command + handler; injects ITenantContext.SchoolId
- `Features/Roles/CreateRole/CreateRoleEndpoint.cs` - POST /roles, returns 201 with location header to GetRoleById
- `Features/Roles/CreateRole/CreateRoleValidator.cs` - NotEmpty, MaximumLength(150) for Name
- `Features/Roles/GetRoles/GetRolesQuery.cs` - Query + handler; projects to RoleListItem (id, name)
- `Features/Roles/GetRoles/GetRolesEndpoint.cs` - GET /roles
- `Features/Roles/GetRoleById/GetRoleByIdQuery.cs` - Query + handler; throws NotFoundException.For<Role>
- `Features/Roles/GetRoleById/GetRoleByIdEndpoint.cs` - GET /roles/{id:guid}
- `Features/Roles/UpdateRole/UpdateRoleCommand.cs` - Command + handler; calls role.UpdateName
- `Features/Roles/UpdateRole/UpdateRoleEndpoint.cs` - PUT /roles/{id:guid}; co-located UpdateRoleRequest record
- `Features/Roles/UpdateRole/UpdateRoleValidator.cs` - NotEmpty for both Id and Name
- `Features/Roles/DeleteRole/DeleteRoleCommand.cs` - Command + handler; ExistsByRoleIdAsync check then role.Delete()
- `Features/Roles/DeleteRole/DeleteRoleEndpoint.cs` - DELETE /roles/{id:guid}; Produces 409
- `Features/Roles/AssignPermissions/AssignPermissionsCommand.cs` - Command + handler; GetByNamesAsync validation + SetPermissions
- `Features/Roles/AssignPermissions/AssignPermissionsEndpoint.cs` - PUT /roles/{id:guid}/permissions; co-located AssignPermissionsRequest
- `Features/Roles/AssignPermissions/AssignPermissionsValidator.cs` - NotEmpty for RoleId
- `Features/Roles/GetRolePermissions/GetRolePermissionsQuery.cs` - Query + handler; in-memory filter from GetAllAsync
- `Features/Roles/GetRolePermissions/GetRolePermissionsEndpoint.cs` - GET /roles/{id:guid}/permissions
- `UnitTests/Features/CreateRoleCommandHandlerTests.cs` - 2 tests for tenant scoping and Id return
- `UnitTests/Features/DeleteRoleCommandHandlerTests.cs` - 3 tests: soft-delete, 409 guard, not-found
- `UnitTests/Features/AssignPermissionsCommandHandlerTests.cs` - 4 tests: valid, invalid names, not-found, empty list
- `Edvantix.Organizations/GlobalUsings.cs` - Added 3 domain aggregate namespaces

## Decisions Made

- `ICommand<Unit>` / `ICommandHandler<TCommand, Unit>` used for void commands — the Mediator source generator only provides `ICommand<T>` (no non-generic variant), so Unit is used as the return type with `return default`
- Mock `AddAsync` assigns a `Guid.CreateVersion7()` to the role's `Id` to simulate EF Core key generation that would occur before `SaveChanges` in production — without this the returned Id would be empty in unit tests
- `GetRolePermissions` fetches all permissions via `GetAllAsync` and filters in-memory. Acceptable for v1 because the Permission catalogue is a small, bounded set (tens of entries). A `GetByIdsAsync` method can be added to `IPermissionRepository` if this becomes a bottleneck.
- Domain aggregate namespaces added to `GlobalUsings.cs` so they're available project-wide without per-file usings

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Unit test for `GivenValidCommand_WhenHandling_ThenReturnsNewRoleId` failed because `role.Id` was empty**

- **Found during:** Task 1 (TDD GREEN phase)
- **Issue:** `Role.Id` (inherited from `Entity`) defaults to `Guid.Empty` — EF Core would normally generate/assign the key before `SaveChanges`. The `AddAsync` mock returned the role unchanged, so the handler returned `Guid.Empty`.
- **Fix:** Updated the `AddAsync` mock lambda to call `r.Id = Guid.CreateVersion7()` before returning, simulating EF Core's key generation behaviour in the real repository.
- **Files modified:** `CreateRoleCommandHandlerTests.cs`
- **Verification:** All 29 unit tests pass after fix
- **Committed in:** 2e8f928 (Task 1 commit)

---

**Total deviations:** 1 auto-fixed (Rule 1 - bug in test mock setup)
**Impact on plan:** Mock fix required for test correctness. No production code changed. No scope creep.

## Issues Encountered

None.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- All seven role management endpoints implemented and verified
- Pattern established: IEndpoint + ICommand<Unit>/ICommandHandler for PUT/DELETE endpoints, inline request records
- Plan 04 (UserRoleAssignment features) can build on the same pattern with the `IUserRoleAssignmentRepository` interface

---
*Phase: 01-organizations-rbac-core*
*Completed: 2026-03-19*
