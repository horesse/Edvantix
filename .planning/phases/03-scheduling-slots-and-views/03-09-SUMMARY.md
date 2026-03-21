---
phase: 03-scheduling-slots-and-views
plan: 09
subsystem: api
tags: [grpc, efcore, organizations, scheduling, group-membership]

requires:
  - phase: 03-scheduling-slots-and-views
    plan: 08
    provides: Group aggregate, GroupsPermissions, Organizations Groups CRUD API

provides:
  - GroupMembership entity within Group aggregate (Organizations service)
  - Group.AddMember (idempotent per SCH-06) and Group.RemoveMember (no-op)
  - POST /v1/groups/{groupId}/members — add student to group with Persona gRPC validation
  - DELETE /v1/groups/{groupId}/members/{profileId} — remove student from group
  - GET /v1/groups/{groupId}/members — list group members
  - groups.proto with GetGroupsForStudent and GetGroup RPCs
  - GroupsGrpcService in Organizations implementing both RPCs
  - IOrganizationsGroupService.GetGroupsForStudentAsync in Scheduling (gRPC-based)
  - Scheduling gRPC client replacing HTTP fallback from 03-04

affects:
  - 03-05 (GetSchedule — student view needs GetGroupsForStudentAsync to resolve group memberships)

tech-stack:
  added: []
  patterns:
    - GroupMembership entity as child of Group aggregate — following RolePermission pattern
    - IgnoreQueryFilters on gRPC server paths — explicit schoolId/groupId filter applied instead of ambient ITenantContext
    - gRPC client via AddGrpcServiceReference — consistent with Persona client pattern

key-files:
  created:
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/GroupAggregate/GroupMembership.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/GroupMembershipConfiguration.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/GroupMembership/AddStudentToGroup/AddStudentToGroupCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/GroupMembership/AddStudentToGroup/AddStudentToGroupCommandHandler.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/GroupMembership/AddStudentToGroup/AddStudentToGroupCommandValidator.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/GroupMembership/AddStudentToGroup/AddStudentToGroupEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/GroupMembership/RemoveStudentFromGroup/RemoveStudentFromGroupCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/GroupMembership/RemoveStudentFromGroup/RemoveStudentFromGroupCommandHandler.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/GroupMembership/RemoveStudentFromGroup/RemoveStudentFromGroupEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/GroupMembership/GetGroupMembers/GetGroupMembersQuery.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/GroupMembership/GetGroupMembers/GetGroupMembersEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Grpc/Services/GroupsGrpcService.cs
    - src/BuildingBlocks/Edvantix.Chassis/Security/Protos/organizations/v1/groups.proto
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Protos/organizations/v1/groups.proto
  modified:
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/GroupAggregate/Group.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/GroupAggregate/IGroupRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/GroupRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/OrganizationsDbContext.cs
    - src/Services/Organizations/Edvantix.Organizations/Edvantix.Organizations.csproj
    - src/Services/Organizations/Edvantix.Organizations/Program.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Services/OrganizationsGroupService.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Extensions.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Edvantix.Scheduling.csproj
    - tests/Edvantix.Organizations.UnitTests/Domain/GroupAggregateTests.cs

key-decisions:
  - "GroupMembership uses WithMany(nameof(Group.Members)) not WithMany('_members') in EF config — EF auto-discovers backing field from property, explicit string causes CS0430 duplicate field mapping error"
  - "GroupsGrpcService uses IgnoreQueryFilters — gRPC context has no X-School-Id header (no ITenantContext). GetGroupsForStudent applies explicit schoolId; GetGroup filters by !IsDeleted only (safe since GroupId is not guessable)"
  - "IOrganizationsGroupService.GetGroupsForStudentAsync and GetGroupAsync already existed from Plan 03-08 — only the implementation was swapped from HTTP to gRPC"
  - "IOrganizationsGroupService registration changed from Singleton to Scoped — gRPC client is injected and may carry per-request metadata in future"
  - "AddStudentToGroup throws NotFoundException<GroupMembership> (not Profile) on missing profile — NotFoundException.For<T> only accepts one Guid parameter; using GroupMembership type as surrogate for profile-not-found case"

patterns-established:
  - "TDD for aggregate methods: test file first (RED — compile errors), domain implementation (GREEN — tests pass), no refactor needed"
  - "Group.Id must be set to Guid.CreateVersion7() in unit tests before calling methods that pass Id to child constructors — mirrors RoleAggregateTests pattern"

requirements-completed: [SCH-05, SCH-06, SCH-07]

duration: 10min
completed: 2026-03-21
---

# Phase 03 Plan 09: Group Membership and Groups gRPC Service Summary

**GroupMembership entity in Group aggregate, add/remove/list student endpoints, GetGroupsForStudent and GetGroup RPCs, and Scheduling gRPC client replacing HTTP fallback**

## Performance

- **Duration:** 10 min
- **Started:** 2026-03-21T12:12:19Z
- **Completed:** 2026-03-21T12:22:41Z
- **Tasks:** 2
- **Files modified:** 24

## Accomplishments

- GroupMembership entity within Group aggregate (following RolePermission pattern), with idempotent AddMember and no-op RemoveMember
- POST/DELETE/GET /v1/groups/{groupId}/members endpoints protected by groups.manage-group-membership; AddStudent validates via Persona gRPC before modifying aggregate
- groups.proto (GetGroupsForStudent + GetGroup RPCs), GroupsGrpcService in Organizations, and Scheduling gRPC client replacing the HTTP fallback from Plan 03-04
- All 65 arch tests pass; 15 Organization unit tests pass (5 new TDD tests for GroupMembership behaviors)

## Task Commits

1. **Task 1: GroupMembership entity + add/remove/list student endpoints (TDD)** - `4f135f5` (feat)
2. **Task 2: groups.proto, GroupsGrpcService, Scheduling gRPC client** - `1c23116` (feat)

## Files Created/Modified

- `GroupMembership.cs` — Entity within Group aggregate, ITenanted, EF private constructor
- `GroupMembershipConfiguration.cs` — EF config: table group_memberships, unique (GroupId, ProfileId), cascade FK
- `Group.cs` — Added _members backing field, Members collection, AddMember (idempotent), RemoveMember (no-op)
- `IGroupRepository.cs` — Added FindByIdWithMembersAsync
- `GroupRepository.cs` — Implemented FindByIdWithMembersAsync with .Include(g => g.Members)
- `OrganizationsDbContext.cs` — Added DbSet<GroupMembership>, tenant HasQueryFilter
- `AddStudentToGroup*` — Command, Handler (Persona gRPC validation + idempotent add), Validator, Endpoint (POST)
- `RemoveStudentFromGroup*` — Command, Handler (no-op remove), Endpoint (DELETE)
- `GetGroupMembers*` — Query+Handler (list members), Endpoint (GET)
- `groups.proto` — In Chassis and Scheduling (edition=2023, GetGroupsForStudent + GetGroup)
- `GroupsGrpcService.cs` — Organizations gRPC server (IgnoreQueryFilters on both RPCs)
- `OrganizationsGroupService.cs` — Replaced HTTP with gRPC implementation
- `Grpc/Extensions.cs` (Scheduling) — Added AddGrpcServiceReference for GroupsGrpcServiceClient

## Decisions Made

- `WithMany(nameof(Group.Members))` in EF config instead of `"_members"` — prevents duplicate field mapping error (EF auto-discovers backing field)
- `IgnoreQueryFilters` on gRPC server methods — gRPC lacks X-School-Id header, explicit schoolId filter provides tenant scoping for GetGroupsForStudent
- IOrganizationsGroupService interface unchanged — it already had GetGroupsForStudentAsync from Plan 03-08; only implementation swapped
- Registration changed from Singleton to Scoped for gRPC-based IOrganizationsGroupService

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] EF Core field mapping conflict with _members backing field**
- **Found during:** Task 1 (arch test run)
- **Issue:** GroupMembershipConfiguration used `WithMany("_members")`, but EF Core auto-discovers the backing field from the `Members` property. Specifying it again caused `InvalidOperationException: member '_members' cannot use field '_members' because it is already used by 'Group.Members'`
- **Fix:** Changed `WithMany("_members")` to `WithMany(nameof(Group.Members))` — EF resolves navigation via the property and auto-discovers the backing field without conflict
- **Files modified:** `GroupMembershipConfiguration.cs`
- **Verification:** 65 arch tests pass
- **Committed in:** `4f135f5` (Task 1 commit)

**2. [Rule 1 - Bug] Unit tests required Group.Id assignment before AddMember**
- **Found during:** Task 1 TDD GREEN phase
- **Issue:** `Group.Id` is `Guid.Empty` after public constructor — EF SQL default only fires on INSERT. `AddMember` passes `Id` to `GroupMembership` constructor which calls `Guard.Against.Default(groupId)`, causing ArgumentException in tests
- **Fix:** Added `group.Id = Guid.CreateVersion7()` before membership operations in tests, mirroring `RoleAggregateTests` pattern
- **Files modified:** `GroupAggregateTests.cs`
- **Verification:** 15 unit tests pass
- **Committed in:** `4f135f5` (Task 1 commit)

---

**Total deviations:** 2 auto-fixed (Rule 1 — bugs in EF config and test setup)
**Impact on plan:** Both fixes necessary for correctness. No scope creep.

## Issues Encountered

None — both issues caught by test/arch-test feedback loop and fixed within the same task.

## User Setup Required

None — no external service configuration required.

## Next Phase Readiness

- SCH-06 (add student to group) and SCH-07 (remove student from group) are fully delivered in Organizations
- Scheduling now has `GetGroupsForStudentAsync` via gRPC — Plan 03-05 (student schedule view) can call it to filter lesson slots by the student's groups
- Groups gRPC client in Scheduling fully replaces the HTTP fallback; the named "organizations" HttpClient remains for PermissionSeeder only
- A database migration is required to create the `group_memberships` table before running in production

---
*Phase: 03-scheduling-slots-and-views*
*Completed: 2026-03-21*

## Self-Check: PASSED

- GroupMembership.cs: FOUND
- GroupsGrpcService.cs: FOUND
- groups.proto (Chassis): FOUND
- groups.proto (Scheduling): FOUND
- SUMMARY.md: FOUND
- Commit 4f135f5: FOUND
- Commit 1c23116: FOUND
