---
phase: 01-organizations-rbac-core
plan: 02
subsystem: domain
tags: [ddd, efcore, repository-pattern, guard-clauses, tunit, domain-aggregates]

# Dependency graph
requires:
  - phase: 01-01
    provides: "Role/Permission/UserRoleAssignment stubs, OrganizationsDbContext, ITenanted, RoleConfiguration"

provides:
  - "Role aggregate with full business logic: AssignPermission, RemovePermission, SetPermissions, UpdateName, Delete"
  - "Permission aggregate with name guard validation"
  - "UserRoleAssignment aggregate with all-Guid guard validation"
  - "IRoleRepository, IPermissionRepository, IUserRoleAssignmentRepository interfaces"
  - "RoleRepository, PermissionRepository, UserRoleAssignmentRepository EF Core implementations"
  - "24 passing domain unit tests covering all aggregate behaviors and invariants"
  - "Guard.Against.NullOrWhiteSpace and Guard.Against.Default extension methods"

affects:
  - "01-03 (application services build on these aggregates and repository interfaces)"
  - "01-04 (API endpoints compose with repositories)"

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Guard.Against extension methods pattern for domain validation (NullOrWhiteSpace, Default)"
    - "TDD Red-Green cycle: write failing tests first, then implement to pass"
    - "Repository pattern: sealed class + primary constructor + IUnitOfWork delegation"
    - "Backing field access mode for EF Core navigation on aggregate collections"
    - "Idempotent permission upsert via Except on existing names"

key-files:
  created:
    - src/BuildingBlocks/Edvantix.Chassis/Utilities/Guards/GuardAgainstStringExtensions.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/RoleAggregate/IRoleRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/PermissionAggregate/IPermissionRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/UserRoleAssignmentAggregate/IUserRoleAssignmentRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/RoleRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/PermissionRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/UserRoleAssignmentRepository.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Domain/RoleAggregateTests.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Domain/PermissionAggregateTests.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Domain/UserRoleAssignmentAggregateTests.cs
  modified:
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/RoleAggregate/Role.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/PermissionAggregate/Permission.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/UserRoleAssignmentAggregate/UserRoleAssignment.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/RoleConfiguration.cs
    - src/Services/Organizations/Edvantix.Organizations/GlobalUsings.cs

key-decisions:
  - "Guard.Against.NullOrWhiteSpace and Guard.Against.Default added to Chassis (not Ardalis) — consistent with project's custom Guard pattern"
  - "RoleRepository eagerly loads Permissions navigation on FindByIdAsync — needed for AssignPermission/RemovePermission operations without extra round trips"
  - "PermissionRepository.UpsertAsync materialises names list before Except to avoid multiple-enumeration warning"
  - "RoleConfiguration.Permissions backing field access mode set in IEntityTypeConfiguration (not DbContext) — applies to static EF model config only, no ITenantContext needed"

patterns-established:
  - "Repository: sealed + primary constructor + IUnitOfWork => context delegation"
  - "Domain validation: Guard.Against.NullOrWhiteSpace for strings, Guard.Against.Default for Guids"
  - "Test naming: GivenCondition_WhenAction_ThenExpectedResult (matching DDD guidelines)"

requirements-completed:
  - ORG-01
  - ORG-02
  - ORG-03

# Metrics
duration: 5min
completed: 2026-03-18
---

# Phase 01 Plan 02: Organizations Domain Model Summary

**Role/Permission/UserRoleAssignment aggregates with guard-clause validation, repository interfaces, EF Core implementations, and 24 passing domain unit tests**

## Performance

- **Duration:** 5 min
- **Started:** 2026-03-18T21:38:58Z
- **Completed:** 2026-03-18T21:43:58Z
- **Tasks:** 2
- **Files modified:** 15

## Accomplishments

- Full Role aggregate with `AssignPermission`, `RemovePermission`, `SetPermissions`, `UpdateName`, `Delete` — all with Guard validation and idempotent permission operations
- Permission and UserRoleAssignment aggregates enhanced with `Guard.Against` validation on all inputs
- Three repository interfaces (`IRoleRepository`, `IPermissionRepository`, `IUserRoleAssignmentRepository`) following the Persona `IProfileRepository` pattern
- Three sealed EF Core repository implementations with eager loading and idempotent upsert logic
- 24 domain unit tests covering creation, updates, deletion, permission assignment, guard-clause rejection (TDD Red-Green)

## Task Commits

Each task was committed atomically:

1. **Task 1: Domain aggregates with guard clauses and unit tests** - `f8d3429` (feat)
2. **Task 2: Repository interfaces and implementations** - `78509d0` (feat)

## Files Created/Modified

- `src/BuildingBlocks/Edvantix.Chassis/Utilities/Guards/GuardAgainstStringExtensions.cs` - NullOrWhiteSpace and Default guard extensions
- `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/RoleAggregate/Role.cs` - Full domain aggregate with permission management
- `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/PermissionAggregate/Permission.cs` - Guard-validated Permission aggregate
- `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/UserRoleAssignmentAggregate/UserRoleAssignment.cs` - Guard-validated UserRoleAssignment
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/RoleConfiguration.cs` - Permissions backing field access mode
- `src/Services/Organizations/Edvantix.Organizations/GlobalUsings.cs` - Added Edvantix.Chassis.Utilities.Guards
- `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/RoleAggregate/IRoleRepository.cs` - Role repository interface
- `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/PermissionAggregate/IPermissionRepository.cs` - Permission repository interface
- `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/UserRoleAssignmentAggregate/IUserRoleAssignmentRepository.cs` - UserRoleAssignment repository interface
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/RoleRepository.cs` - EF Core Role repository (eager loads Permissions)
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/PermissionRepository.cs` - EF Core Permission repository with UpsertAsync
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/UserRoleAssignmentRepository.cs` - EF Core UserRoleAssignment repository
- `src/Services/Organizations/Edvantix.Organizations.UnitTests/Domain/RoleAggregateTests.cs` - 11 Role aggregate tests
- `src/Services/Organizations/Edvantix.Organizations.UnitTests/Domain/PermissionAggregateTests.cs` - 3 Permission aggregate tests
- `src/Services/Organizations/Edvantix.Organizations.UnitTests/Domain/UserRoleAssignmentAggregateTests.cs` - 4 UserRoleAssignment aggregate tests

## Decisions Made

- Guard.Against extension methods added to Chassis (custom, not Ardalis) to maintain project consistency — `NullOrWhiteSpace` delegates to `ArgumentException.ThrowIfNullOrWhiteSpace`, `Default` uses `IEquatable<T>` comparison
- `RoleRepository.FindByIdAsync` eagerly includes `Permissions` navigation — required for permission assignment/removal operations without extra database roundtrips in the application layer
- `PermissionRepository.UpsertAsync` materialises the `names` parameter to a List before the two async queries to avoid multiple-enumeration

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 2 - Missing Critical] Added Guard.Against.NullOrWhiteSpace and Guard.Against.Default extension methods**
- **Found during:** Task 1 (domain aggregate implementation)
- **Issue:** Plan specified `Guard.Against.NullOrWhiteSpace` and `Guard.Against.Default` but these methods did not exist in the project's custom Guard class (only `NotFound` and `NotAuthenticated` existed)
- **Fix:** Created `GuardAgainstStringExtensions.cs` in `Edvantix.Chassis/Utilities/Guards/` with both extension methods using C# 13 extension block syntax consistent with existing guard files
- **Files modified:** `src/BuildingBlocks/Edvantix.Chassis/Utilities/Guards/GuardAgainstStringExtensions.cs` (created), `src/Services/Organizations/Edvantix.Organizations/GlobalUsings.cs`
- **Verification:** Build passes 0 errors, 24 unit tests pass, arch tests pass
- **Committed in:** f8d3429 (Task 1 commit)

---

**Total deviations:** 1 auto-fixed (Rule 2 - missing critical functionality)
**Impact on plan:** Required for the plan's Guard.Against pattern to work. No scope creep — the guard methods are minimal infrastructure additions with no behavioral changes to existing code.

## Issues Encountered

None.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- Domain layer complete: Role, Permission, UserRoleAssignment aggregates with full business logic
- Repository interfaces defined: Plans 03 and 04 can depend on `IRoleRepository`, `IPermissionRepository`, `IUserRoleAssignmentRepository`
- Repository implementations ready: DI registration needed in Plans 03/04 (not done here — per plan scope)
- Architecture tests still pass: 64/64

---
*Phase: 01-organizations-rbac-core*
*Completed: 2026-03-18*
