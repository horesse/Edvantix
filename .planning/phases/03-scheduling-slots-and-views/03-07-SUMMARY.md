---
phase: 03-scheduling-slots-and-views
plan: 07
subsystem: database
tags: [efcore, ddd, permissions, organization, groups]

# Dependency graph
requires:
  - phase: 01-organizations-rbac-core
    provides: Entity/IAggregateRoot base types, PermissionSeeder pattern, OrganizationsDbContext
  - phase: 02-organizations-permission-cache
    provides: GroupsPermissions namespace established for cache invalidation scope
provides:
  - Organization aggregate root (tenant root entity, Id only, not ITenanted)
  - OrganizationConfiguration mapping to 'organizations' table
  - OrganizationsDbContext.Organizations DbSet
  - GroupsPermissions class with 4 constants in groups.* namespace
  - PermissionSeeder extended to seed GroupsPermissions.All
affects:
  - 03-08-groups-crud (uses GroupsPermissions for authorization, Organization as FK target)
  - 03-09-group-membership (uses ManageGroupMembership permission)

# Tech tracking
tech-stack:
  added: []
  patterns:
    - Organization as tenant root (not ITenanted) — it IS the tenant, not scoped to another tenant
    - ValueGeneratedNever for externally-assigned Organization IDs (provisioned from outside)
    - Grouped permission namespace (groups.*) separate from service namespace (organizations.*)

key-files:
  created:
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/OrganizationAggregate/Organization.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/OrganizationConfiguration.cs
    - src/BuildingBlocks/Edvantix.Constants/Permissions/GroupsPermissions.cs
  modified:
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/OrganizationsDbContext.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/Seeding/PermissionSeeder.cs

key-decisions:
  - "Organization does NOT implement ITenanted — it IS the tenant root. No HasQueryFilter needed."
  - "Organization IDs use ValueGeneratedNever — IDs come from external provisioning, not DB auto-generation."
  - "GroupsPermissions uses groups.* namespace (not organizations.*) to distinguish group-management from RBAC-management permissions."

patterns-established:
  - "Tenant root pattern: aggregate that IS the tenant does not implement ITenanted and has no HasQueryFilter."
  - "Permission namespace separation: each feature domain gets its own prefix (organizations.*, groups.*, scheduling.*)."

requirements-completed: [SCH-01, SCH-06]

# Metrics
duration: 2min
completed: 2026-03-21
---

# Phase 03 Plan 07: Organization Aggregate and GroupsPermissions Summary

**Organization aggregate root (tenant root, Id only, no ITenanted) + GroupsPermissions with 4 groups.* constants seeded via PermissionSeeder**

## Performance

- **Duration:** 2 min
- **Started:** 2026-03-21T11:30:28Z
- **Completed:** 2026-03-21T11:32:22Z
- **Tasks:** 2
- **Files modified:** 5

## Accomplishments

- Created Organization aggregate as tenant root entity (Entity + IAggregateRoot only; no ITenanted, no ISoftDelete in v1)
- Created OrganizationConfiguration mapping to `organizations` table with ValueGeneratedNever on the PK
- Added DbSet<Organization> to OrganizationsDbContext with no HasQueryFilter (Organization is not tenant-scoped)
- Created GroupsPermissions class with 4 constants: groups.create-group, groups.update-group, groups.delete-group, groups.manage-group-membership
- Extended PermissionSeeder to seed both OrganizationsPermissions.All and GroupsPermissions.All on startup
- All 64 architecture tests continue to pass

## Task Commits

Each task was committed atomically:

1. **Task 1: Create Organization aggregate, EF configuration, and update OrganizationsDbContext** - `2f52cec` (feat)
2. **Task 2: Create GroupsPermissions class and extend PermissionSeeder** - `1686b61` (feat)

## Files Created/Modified

- `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/OrganizationAggregate/Organization.cs` - Organization aggregate root, Entity + IAggregateRoot, no ITenanted
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/OrganizationConfiguration.cs` - EF config mapping to 'organizations' table, ValueGeneratedNever
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/OrganizationsDbContext.cs` - Added DbSet<Organization> and using import
- `src/BuildingBlocks/Edvantix.Constants/Permissions/GroupsPermissions.cs` - 4 constants in groups.* namespace
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/Seeding/PermissionSeeder.cs` - Seeds GroupsPermissions.All alongside OrganizationsPermissions.All

## Decisions Made

- Organization does NOT implement ITenanted per decision D-17: Organization IS the tenant root. Applying a tenant query filter would be circular (filtering Organization by its own ID doesn't make sense).
- Used ValueGeneratedNever on Organization.Id because organizations are provisioned externally (e.g., via admin API or Keycloak realm creation), not auto-generated at DB insert time.
- GroupsPermissions uses the `groups.*` namespace (not `organizations.*`) to distinguish group-management operations from RBAC-level operations. Plans 03-08 and 03-09 will use these permissions for Group CRUD and membership management authorization.

## Deviations from Plan

None - plan executed exactly as written.

## Issues Encountered

None.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- Organization aggregate and DbSet ready for use as FK target in Group entity (Plan 03-08)
- GroupsPermissions constants ready for use in Group CRUD endpoint authorization (Plan 03-08)
- ManageGroupMembership permission ready for membership management endpoint authorization (Plan 03-09)
- EF migration for `organizations` table should be added in Plan 03-08 alongside Group entity migration

---
*Phase: 03-scheduling-slots-and-views*
*Completed: 2026-03-21*
