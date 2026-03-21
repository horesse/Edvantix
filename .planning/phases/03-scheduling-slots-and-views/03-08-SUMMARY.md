---
phase: 03-scheduling-slots-and-views
plan: 08
subsystem: api
tags: [efcore, ddd, groups, organizations, cqrs, authorization, rbac]

# Dependency graph
requires:
  - phase: 01-organizations-rbac-core
    provides: Entity/IAggregateRoot/ISoftDelete/ITenanted base types, RoleRepository pattern, OrganizationsDbContext, IUnitOfWork
  - phase: 03-07
    provides: OrganizationAggregate, GroupsPermissions class, PermissionSeeder extended with GroupsPermissions.All
provides:
  - Group aggregate root in Organizations service (Name/SchoolId/MaxCapacity/Color + ISoftDelete + ITenanted)
  - IGroupRepository with FindByIdAsync/GetAllAsync/Add/Remove
  - GroupConfiguration: EF table 'groups', partial unique index (SchoolId, Name) where is_deleted=false
  - GroupRepository EF Core implementation
  - OrganizationsDbContext.Groups DbSet with combined tenant+soft-delete HasQueryFilter
  - POST /groups endpoint (CreateGroup, 201 Created)
  - PUT /groups/{id} endpoint (UpdateGroup, 204 NoContent)
  - DELETE /groups/{id} endpoint (DeleteGroup, soft-delete, 204 NoContent)
  - GET /groups endpoint (GetGroups, 200 Ok with GroupDto list)
  - GroupsPermissions authorization policies wired in Extensions.cs via self-call AddPermissionAuthorization
  - Edvantix.Organizations.UnitTests project with GroupAggregateTests (TDD)
affects:
  - 03-09-group-membership (adds Members collection to Group, uses IGroupRepository, ManageGroupMembership permission)

# Tech tracking
tech-stack:
  added: []
  patterns:
    - TDD for Group aggregate: RED (test project + failing tests) → GREEN (implementation)
    - Organizations self-permission-check: AddPermissionAuthorization points to own service name for group endpoints
    - GroupRepository follows RoleRepository pattern (no eager loading — no navigation properties in Group yet)

key-files:
  created:
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/GroupAggregate/Group.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/GroupAggregate/IGroupRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/GroupConfiguration.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/GroupRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/CreateGroup/CreateGroupCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/CreateGroup/CreateGroupCommandHandler.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/CreateGroup/CreateGroupCommandValidator.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/CreateGroup/CreateGroupEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/UpdateGroup/UpdateGroupCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/UpdateGroup/UpdateGroupCommandHandler.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/UpdateGroup/UpdateGroupCommandValidator.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/UpdateGroup/UpdateGroupEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/DeleteGroup/DeleteGroupCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/DeleteGroup/DeleteGroupCommandHandler.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/DeleteGroup/DeleteGroupEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/GetGroups/GetGroupsQuery.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Groups/GetGroups/GetGroupsEndpoint.cs
    - tests/Edvantix.Organizations.UnitTests/Domain/GroupAggregateTests.cs
    - tests/Edvantix.Organizations.UnitTests/Edvantix.Organizations.UnitTests.csproj
    - tests/Edvantix.Organizations.UnitTests/GlobalUsings.cs
  modified:
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/OrganizationsDbContext.cs
    - src/Services/Organizations/Edvantix.Organizations/GlobalUsings.cs
    - src/Services/Organizations/Edvantix.Organizations/Extensions/Extensions.cs

key-decisions:
  - "Organizations uses AddPermissionAuthorization pointing to itself (self-call) so GroupsPermissions policies enforce RBAC checks via the same gRPC handler used by downstream services."
  - "IGroupRepository.Add is synchronous (void) unlike IRoleRepository.AddAsync — Groups don't need async add since there are no navigation property loads required at creation time."
  - "GetGroupsEndpoint uses GroupsPermissions.CreateGroup for authorization (viewing groups requires the same access level as creating them) — follows the existing Role GET pattern."

patterns-established:
  - "Self-permission-check: Organizations service can guard its own endpoints with GroupsPermissions policies by wiring AddPermissionAuthorization to its own Aspire service name."
  - "IGroupRepository.Add void: No async needed on Add when no navigation properties need loading post-insert."

requirements-completed: [SCH-01]

# Metrics
duration: 8min
completed: 2026-03-21
---

# Phase 03 Plan 08: Group CRUD in Organizations Service Summary

**Group aggregate (Name/MaxCapacity/Color + ISoftDelete + ITenanted) with full CRUD API (POST/PUT/DELETE/GET /groups) protected by GroupsPermissions constants via self-referencing gRPC authorization in Organizations service**

## Performance

- **Duration:** 8 min
- **Started:** 2026-03-21T12:02:06Z
- **Completed:** 2026-03-21T12:10:48Z
- **Tasks:** 2 (Task 1 TDD: 3 commits; Task 2: 1 commit)
- **Files modified:** 23

## Accomplishments

- Created Group aggregate with Name (max 150), SchoolId (ITenanted), MaxCapacity, Color (max 50), ISoftDelete — follows Role pattern exactly
- GroupConfiguration with partial unique index on `(SchoolId, Name)` where `is_deleted = false`
- OrganizationsDbContext extended: Groups DbSet + combined tenant+soft-delete `HasQueryFilter`
- Full CRUD: POST /groups (201), PUT /groups/{id} (204), DELETE /groups/{id} (soft-delete 204), GET /groups (200 with GroupDto)
- All 4 endpoints protected by `GroupsPermissions.*` constants — authorization via `AddPermissionAuthorization` self-call
- Created `Edvantix.Organizations.UnitTests` project with 10 TDD tests covering Group constructor, Update, Delete behaviors
- 65 architecture tests continue to pass

## Task Commits

Each task was committed atomically:

1. **Task 1 TDD RED: Add failing tests for Group aggregate** - `28afbe5` (test)
2. **Task 1 TDD GREEN: Implement Group aggregate, repository, EF config, DbContext update** - `5bd9ffc` (feat)
3. **Task 2: Implement Group CRUD commands, endpoints, and authorization policies** - `c9a8982` (feat)

## Files Created/Modified

- `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/GroupAggregate/Group.cs` - Group aggregate: Name/SchoolId/MaxCapacity/Color + ISoftDelete + ITenanted
- `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/GroupAggregate/IGroupRepository.cs` - Repository interface: FindByIdAsync, GetAllAsync, Add, Remove
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/GroupConfiguration.cs` - EF config: table 'groups', partial unique index (SchoolId, Name)
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/GroupRepository.cs` - EF Core implementation of IGroupRepository
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/OrganizationsDbContext.cs` - Groups DbSet + combined HasQueryFilter
- `src/Services/Organizations/Edvantix.Organizations/GlobalUsings.cs` - Added GroupAggregate namespace
- `src/Services/Organizations/Edvantix.Organizations/Features/Groups/CreateGroup/` - Command, handler, validator, endpoint
- `src/Services/Organizations/Edvantix.Organizations/Features/Groups/UpdateGroup/` - Command, handler, validator, endpoint
- `src/Services/Organizations/Edvantix.Organizations/Features/Groups/DeleteGroup/` - Command, handler, endpoint
- `src/Services/Organizations/Edvantix.Organizations/Features/Groups/GetGroups/` - Query (with GroupDto), handler, endpoint
- `src/Services/Organizations/Edvantix.Organizations/Extensions/Extensions.cs` - Added GroupsPermissions policies + AddPermissionAuthorization self-call
- `tests/Edvantix.Organizations.UnitTests/` - New test project: 10 unit tests for Group aggregate (TDD)

## Decisions Made

- Organizations uses `AddPermissionAuthorization($"https+http://{Services.Organizations}")` to self-call its own gRPC permissions endpoint. This ensures group management endpoints are protected by the same RBAC system as other services, without duplicating the authorization logic. The self-call is feasible via Aspire service discovery.
- `IGroupRepository.Add` is synchronous (no `async Task<Group>` like `IRoleRepository.AddAsync`) because Group has no navigation properties to eagerly load at creation time — the simpler void Add pattern is sufficient.
- `GetGroupsEndpoint` uses `GroupsPermissions.CreateGroup` for authorization guard (same as create permission) following the Role GET pattern where listing is guarded by the same level as the creating permission.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Created Edvantix.Organizations.UnitTests project**
- **Found during:** Task 1 (TDD RED phase)
- **Issue:** No unit test project existed for Organizations service, required for TDD execution
- **Fix:** Created new test project following Edvantix.Scheduling.UnitTests pattern (TUnit via Directory.Build.props, no explicit TUnit reference needed)
- **Files modified:** tests/Edvantix.Organizations.UnitTests/ (new directory + project)
- **Verification:** Project builds and 10 tests pass
- **Committed in:** 28afbe5 (TDD RED commit)

---

**Total deviations:** 1 auto-fixed (1 blocking)
**Impact on plan:** Required for TDD execution — the plan calls for TDD on Task 1 but no test project existed. No scope creep.

## Issues Encountered

- `dotnet build --no-restore` failed for new test project on first attempt due to missing transitive assembly references (SharedKernel, Chassis). Full `dotnet build` (with restore) succeeded. The issue was that artifacts from the transitive project chain weren't cached locally without the restore step.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- Group aggregate and IGroupRepository ready for GroupMembership navigation property in Plan 03-09
- ManageGroupMembership permission policy already registered in Extensions.cs (added in this plan)
- EF migration for `groups` table should be added in Plan 03-09 alongside GroupMembership table migration
- GroupsPermissions.All are seeded into the DB by PermissionSeeder (from Plan 03-07)

---
*Phase: 03-scheduling-slots-and-views*
*Completed: 2026-03-21*
