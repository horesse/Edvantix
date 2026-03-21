---
phase: 03-scheduling-slots-and-views
verified: 2026-03-21T00:00:00Z
status: passed
score: 28/28 must-haves verified
re_verification: false
---

# Phase 03: Scheduling Slots and Views Verification Report

**Phase Goal:** Deliver the complete scheduling feature — lesson slot CRUD with teacher conflict detection, permission-filtered schedule views (teacher/student/manager), and group management (create/edit/delete groups + membership) fully wired into the Organizations service so students can be enrolled and the Scheduling service can validate group membership via gRPC.
**Verified:** 2026-03-21
**Status:** passed
**Re-verification:** No — initial verification

## Goal Achievement

### Observable Truths

| # | Truth | Status | Evidence |
|---|-------|--------|----------|
| 1 | Scheduling service compiles and is visible as a resource in Aspire dashboard | VERIFIED | `dotnet build Edvantix.Scheduling.csproj` — 0 errors, 0 warnings; `AppHost.cs:89` has `AddProject<Edvantix_Scheduling>` wired with all dependencies |
| 2 | Scheduling permissions are seeded to Organizations via HTTP on startup | VERIFIED | `PermissionSeeder.cs:27` POSTs to `/v1/permissions/register` using `IHttpClientFactory` named client |
| 3 | All 5 scheduling permission constants defined (4 original + ViewOwnSchedule added in Plan 05) | VERIFIED | `SchedulingPermissions.cs` has CreateLessonSlot, EditLessonSlot, DeleteLessonSlot, ViewSchedule, ViewOwnSchedule; `All` list contains all 5 |
| 4 | SchedulingDbContext has exactly ONE HasQueryFilter for LessonSlot (tenant-only, no soft-delete) | VERIFIED | `SchedulingDbContext.cs:34` — single `HasQueryFilter(s => s.SchoolId == tenantContext.SchoolId)` |
| 5 | LessonSlot domain aggregate exists with ITenanted, DateTimeOffset times, GroupId as plain Guid | VERIFIED | `LessonSlot.cs` — `Entity, IAggregateRoot, ITenanted`; `GroupId` is plain Guid with comment "no navigation property"; both time props are `DateTimeOffset` |
| 6 | LessonSlotRepository.HasConflictAsync uses IgnoreQueryFilters for global teacher conflict check | VERIFIED | `LessonSlotRepository.cs` — `context.LessonSlots.IgnoreQueryFilters().AnyAsync(...)` with strict overlap predicate |
| 7 | Architecture tests pass confirming Scheduling tenant isolation | VERIFIED | 65/65 arch tests pass including `SchedulingTenantIsolationTests.cs` (1 ITenanted entity: LessonSlot) |
| 8 | Unit tests for LessonSlot constructor, Reschedule, ChangeTeacher pass | VERIFIED | 11/11 unit tests pass in `Edvantix.Scheduling.UnitTests` |
| 9 | Manager can create a lesson slot with GroupId validation via Organizations gRPC and teacher conflict check | VERIFIED | `CreateLessonSlotCommandHandler.cs` — calls `groupService.GroupExistsAsync`, then `slotRepository.HasConflictAsync(null)`, then creates slot |
| 10 | Manager can edit a slot's time or teacher with self-exclusion on conflict check | VERIFIED | `EditLessonSlotCommandHandler.cs` — calls `HasConflictAsync(command.Id)` with slot ID as excludedSlotId |
| 11 | Manager can delete a slot (hard delete) | VERIFIED | `DeleteLessonSlotCommand.cs` contains co-located `DeleteLessonSlotCommandHandler` that calls `slotRepository.Remove(slot)` |
| 12 | All date/time values in commands and responses use DateTimeOffset | VERIFIED | No `DateTime.Now` or `DateTime.UtcNow` found in Scheduling service code; all command/entity props use `DateTimeOffset` |
| 13 | Single GET /schedule endpoint returns different data per caller permission level | VERIFIED | `GetScheduleQueryHandler.cs` — two gRPC permission checks (manager via `CreateLessonSlot`, teacher via `ViewOwnSchedule`), three distinct query branches |
| 14 | Teacher sees only their own slots (detected by permission, not data-driven) | VERIFIED | Handler line 104-107: filters `s.TeacherId == profileId` only when `isTeacher == true` (from permission check) |
| 15 | Student sees only slots for groups they belong to via Organizations gRPC | VERIFIED | Handler line 112-125: calls `groupService.GetGroupsForStudentAsync(schoolId, profileId, ct)` then filters `groupIds.Contains(s.GroupId)` |
| 16 | Organization aggregate exists in Organizations service (tenant root, NOT ITenanted) | VERIFIED | `Organization.cs` — `Entity, IAggregateRoot` only; no ITenanted; `OrganizationsDbContext.cs:22` has `DbSet<Organization>` |
| 17 | GroupsPermissions.All has 4 items in groups.* namespace | VERIFIED | `GroupsPermissions.cs` — CreateGroup, UpdateGroup, DeleteGroup, ManageGroupMembership; `All` list returns 4 items |
| 18 | Organizations PermissionSeeder seeds GroupsPermissions.All alongside OrganizationsPermissions.All | VERIFIED | `PermissionSeeder.cs:28` — `UpsertAsync(GroupsPermissions.All, ...)` called after OrganizationsPermissions |
| 19 | Group aggregate exists with Name/SchoolId/MaxCapacity/Color/ISoftDelete/ITenanted | VERIFIED | `Group.cs` — `Entity, IAggregateRoot, ISoftDelete, ITenanted`; all 4 properties present |
| 20 | Group EF configuration has partial unique index on (SchoolId, Name) where is_deleted = false | VERIFIED | `GroupConfiguration.cs:28-30` — `HasIndex(...SchoolId, Name...).IsUnique().HasFilter("is_deleted = false")` |
| 21 | OrganizationsDbContext has Groups DbSet with combined tenant + soft-delete HasQueryFilter | VERIFIED | `OrganizationsDbContext.cs:28,65` — `DbSet<Group>` and `HasQueryFilter(g => g.SchoolId == tenantContext.SchoolId && !g.IsDeleted)` |
| 22 | Manager can create, update, delete, and list groups via Organizations API with GroupsPermissions guards | VERIFIED | All four feature folders exist; `CreateGroupEndpoint.cs:30` `.RequireAuthorization(GroupsPermissions.CreateGroup)` confirmed |
| 23 | GroupMembership entity exists within Group aggregate with ITenanted | VERIFIED | `GroupMembership.cs` — `Entity, ITenanted`; `OrganizationsDbContext.cs:31` has `DbSet<GroupMembership>` |
| 24 | Group.AddMember is idempotent (no-op on duplicate per SCH-06) | VERIFIED | `Group.cs:83-86` — `if (_members.Any(m => m.ProfileId == profileId)) return;` |
| 25 | Student profile validated via Persona gRPC before adding to group | VERIFIED | `AddStudentToGroupCommandHandler.cs:25-33` — `personaProfileService.ValidateProfileExistsAsync` throws NotFoundException if false |
| 26 | Manager can add/remove students from groups, endpoints protected by groups.manage-group-membership | VERIFIED | Both endpoints verified: `AddStudentToGroupEndpoint.cs:30` and `RemoveStudentFromGroupEndpoint.cs:28` require `GroupsPermissions.ManageGroupMembership` |
| 27 | groups.proto defines GetGroupsForStudent and GetGroup RPCs; Organizations implements both | VERIFIED | `groups.proto` exists in chassis protos; `GroupsGrpcService.cs` implements both RPCs with `IgnoreQueryFilters` and explicit schoolId/IsDeleted filtering |
| 28 | Scheduling can call Organizations gRPC to resolve student group memberships | VERIFIED | `OrganizationsGroupService.cs` — uses `GroupsGrpcService.GroupsGrpcServiceClient`; `Grpc/Extensions.cs:37,47` registers client and `IOrganizationsGroupService` |

**Score:** 28/28 truths verified

### Required Artifacts

| Artifact | Status | Notes |
|----------|--------|-------|
| `src/BuildingBlocks/Edvantix.Constants/Permissions/SchedulingPermissions.cs` | VERIFIED | 5 constants (4 original + ViewOwnSchedule), All list correct |
| `src/BuildingBlocks/Edvantix.Constants/Permissions/GroupsPermissions.cs` | VERIFIED | 4 constants in groups.* namespace |
| `src/Services/Scheduling/Edvantix.Scheduling/Edvantix.Scheduling.csproj` | VERIFIED | Builds successfully |
| `src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/SchedulingDbContext.cs` | VERIFIED | Single HasQueryFilter for LessonSlot |
| `src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/Seeding/PermissionSeeder.cs` | VERIFIED | Uses IHttpClientFactory, POSTs to /v1/permissions/register |
| `src/Services/Scheduling/Edvantix.Scheduling/Domain/AggregatesModel/LessonSlotAggregate/LessonSlot.cs` | VERIFIED | ITenanted, IAggregateRoot, DateTimeOffset, GroupId plain Guid |
| `src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/Repositories/LessonSlotRepository.cs` | VERIFIED | HasConflictAsync uses IgnoreQueryFilters, strict overlap predicate |
| `src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/CreateLessonSlot/CreateLessonSlotCommandHandler.cs` | VERIFIED | GroupExistsAsync + HasConflictAsync(null) + slot creation |
| `src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/EditLessonSlot/EditLessonSlotCommandHandler.cs` | VERIFIED | HasConflictAsync(command.Id) self-exclusion |
| `src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/DeleteLessonSlot/DeleteLessonSlotCommand.cs` | VERIFIED | Command and handler co-located; hard delete via Remove() |
| `src/Services/Scheduling/Edvantix.Scheduling/Features/Schedule/GetSchedule/GetScheduleQueryHandler.cs` | VERIFIED | Three permission branches, GetGroupsForStudentAsync for student path |
| `src/Services/Scheduling/Edvantix.Scheduling/Grpc/Services/IOrganizationsGroupService.cs` | VERIFIED | GroupExistsAsync, GetGroupsForStudentAsync, GetGroupAsync |
| `src/Services/Scheduling/Edvantix.Scheduling/Grpc/Services/OrganizationsGroupService.cs` | VERIFIED | Uses GroupsGrpcServiceClient, all three methods implemented |
| `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/OrganizationAggregate/Organization.cs` | VERIFIED | IAggregateRoot only, no ITenanted |
| `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/GroupAggregate/Group.cs` | VERIFIED | All interfaces, AddMember/RemoveMember idempotent |
| `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/GroupAggregate/GroupMembership.cs` | VERIFIED | Entity, ITenanted, DateTimeOffset AddedAt |
| `src/Services/Organizations/Edvantix.Organizations/Grpc/Services/GroupsGrpcService.cs` | VERIFIED | Both RPCs implemented with IgnoreQueryFilters |
| `src/BuildingBlocks/Edvantix.Chassis/Security/Protos/organizations/v1/groups.proto` | VERIFIED | GetGroupsForStudent and GetGroup RPCs defined |
| `src/Services/Scheduling/Edvantix.Scheduling/Grpc/Protos/organizations/v1/groups.proto` | VERIFIED | Client-side copy present |
| `tests/Edvantix.ArchTests/Domain/SchedulingTenantIsolationTests.cs` | VERIFIED | Passes as part of all 65 arch tests |
| `tests/Edvantix.Scheduling.UnitTests/Domain/LessonSlotAggregateTests.cs` | VERIFIED | 11/11 tests pass |
| `tests/Edvantix.Scheduling.UnitTests/Features/GetScheduleQueryHandlerTests.cs` | VERIFIED | All 4 branches covered (manager, teacher, student, date range) |

### Key Link Verification

| From | To | Via | Status | Notes |
|------|----|-----|--------|-------|
| `AppHost.cs` | `Edvantix_Scheduling` project | `AddProject<Edvantix_Scheduling>` | WIRED | AppHost.cs:89 |
| `PermissionSeeder.cs` | Organizations POST /v1/permissions/register | IHttpClientFactory | WIRED | PermissionSeeder.cs:27 |
| `CreateLessonSlotCommandHandler` | `IOrganizationsGroupService.GroupExistsAsync` | gRPC/HTTP call | WIRED | Handler.cs:31 |
| `CreateLessonSlotCommandHandler` | `ILessonSlotRepository.HasConflictAsync` | null excludedSlotId | WIRED | Handler.cs:39-44 |
| `EditLessonSlotCommandHandler` | `ILessonSlotRepository.HasConflictAsync` | command.Id excludedSlotId | WIRED | Handler.cs:34-39 |
| `CreateLessonSlotEndpoint` | `SchedulingPermissions.CreateLessonSlot` | RequireAuthorization | WIRED | Endpoint.cs:32 |
| `GetScheduleQueryHandler` | Organizations gRPC CheckPermission | gRPC client calls | WIRED | Handler.cs:59,76 |
| `GetScheduleQueryHandler` | `IOrganizationsGroupService.GetGroupsForStudentAsync` | student branch | WIRED | Handler.cs:112 |
| `GetScheduleQueryHandler` | `SchedulingDbContext.LessonSlots` | LINQ Where queries | WIRED | Handler.cs:91 |
| `GetScheduleEndpoint` | `SchedulingPermissions.ViewSchedule` | RequireAuthorization | WIRED | Endpoint.cs:39 |
| `CreateGroupEndpoint` | `GroupsPermissions.CreateGroup` | RequireAuthorization | WIRED | Endpoint.cs:30 |
| `CreateGroupCommandHandler` | `ITenantContext.SchoolId` | tenant-scoped creation | WIRED | Handler uses tenantContext |
| `OrganizationsDbContext` | Group entity HasQueryFilter | tenant + soft-delete | WIRED | DbContext.cs:65 |
| `AddStudentToGroupCommandHandler` | `IPersonaProfileService` | Persona gRPC validation | WIRED | Handler.cs:25 |
| `AddStudentToGroupCommandHandler` | `Group.AddMember` | aggregate method | WIRED | Handler.cs:40 |
| `GroupsGrpcService.GetGroupsForStudent` | `OrganizationsDbContext.GroupMemberships` | LINQ query | WIRED | GrpcService.cs:35-40 |
| `OrganizationsGroupService` | `GroupsGrpcService.GroupsGrpcServiceClient` | gRPC client | WIRED | Extensions.cs:37 |

### Requirements Coverage

| Requirement | Plans | Description | Status | Evidence |
|-------------|-------|-------------|--------|----------|
| SCH-01 | 03-02, 03-04, 03-07, 03-08 | Manager can create lesson slot (group + date/time + teacher) with teacher conflict detection | SATISFIED | CreateLessonSlotCommandHandler: GroupExistsAsync + HasConflictAsync + LessonSlot creation |
| SCH-02 | 03-04 | Manager can edit and delete lesson slots | SATISFIED | EditLessonSlotCommandHandler (PUT) + DeleteLessonSlotCommandHandler (DELETE) |
| SCH-03 | 03-05 | Manager sees schedule for all groups in their school | SATISFIED | GetScheduleQueryHandler manager branch: no extra filter, returns all tenant slots |
| SCH-04 | 03-05 | Teacher sees only their own schedule | SATISFIED | GetScheduleQueryHandler teacher branch: filters `s.TeacherId == profileId` |
| SCH-05 | 03-05, 03-09 | Student sees only lessons for groups they belong to | SATISFIED | GetScheduleQueryHandler student branch: GetGroupsForStudentAsync + GroupId filter |
| SCH-06 | 03-09 | Manager can add student to a group | SATISFIED | AddStudentToGroupEndpoint (POST) + Group.AddMember idempotent |
| SCH-07 | 03-09 | Manager can remove student from a group | SATISFIED | RemoveStudentFromGroupEndpoint (DELETE) + Group.RemoveMember |
| SCH-10 | 03-02, 03-04, 03-05 | All dates/times stored as DateTimeOffset | SATISFIED | No DateTime.Now/UtcNow in Scheduling service; all entity/command/DTO props use DateTimeOffset |

**Notes on SCH-08 and SCH-09:** These requirements are explicitly marked `[ ]` (pending) in REQUIREMENTS.md and are not claimed by any Phase 03 plan. They are out of scope for this phase.

### Anti-Patterns Found

| File | Pattern | Severity | Impact |
|------|---------|----------|--------|
| `GetScheduleQueryHandler.cs:167-183` | `TeacherName: s.TeacherId.ToString()` and `StudentCount: 0` | Info | Documented v1 placeholder — plan explicitly states "TeacherName: v1 return TeacherId.ToString() as a placeholder" and "v1 placeholder for student count". Not a stub — this is explicitly deferred behavior. |
| `GetScheduleQueryHandler.cs:147` | `GroupInfoDto(pair.First.ToString(), "#808080")` fallback | Info | Placeholder only when Organizations gRPC returns null for a group. Real data is fetched; fallback is defensive. |

No blockers or warnings found. All "placeholder" comments in the code explicitly reference the v1 scope documented in the plans.

### Structural Note

`DeleteLessonSlotCommandHandler` is co-located in `DeleteLessonSlotCommand.cs` rather than in a separate file as listed in the Plan 04 `files_modified`. The implementation is complete and wired correctly — the handler finds the slot, calls `slotRepository.Remove(slot)`, and saves. This is a file organization deviation, not a functional gap.

`GroupInfoDto` in `IOrganizationsGroupService.cs` is defined as `(string Name, string Color)` rather than the plan spec's `(Guid Id, string Name, string Color, int MaxCapacity)`. The narrowed record still satisfies all usages in `GetScheduleQueryHandler` — `Id` and `MaxCapacity` are never accessed in the DTO mapping. This is a reasonable simplification.

### Human Verification Required

#### 1. Aspire Dashboard Visibility

**Test:** Run `just run`, open the Aspire dashboard.
**Expected:** `scheduling` service appears as a resource with status Running. No error state.
**Why human:** Requires a live Aspire process to verify dashboard visibility.

#### 2. Keycloak Scheduling Client

**Test:** Open Keycloak admin, check the `edvantix` realm for a client matching `${CLIENT_SCHEDULING_ID}`.
**Expected:** Scheduling client exists with service account enabled and `scheduling_*` scopes configured.
**Why human:** Keycloak realm JSON presence can be checked programmatically, but actual realm import success and client activation requires a running Keycloak instance.

#### 3. Permission Seeder End-to-End

**Test:** Start the application, query Organizations API for the permissions list.
**Expected:** All 5 scheduling permissions (`scheduling.create-lesson-slot`, etc.) and all 4 groups permissions (`groups.create-group`, etc.) appear in the Organizations permission catalogue.
**Why human:** Requires both services running and an authenticated HTTP call.

#### 4. gRPC Cross-Service Call (Scheduling → Organizations)

**Test:** As a student, call `GET /schedule?dateFrom=X&dateTo=Y` with a valid token.
**Expected:** Request succeeds; response contains only slots for groups the student is a member of.
**Why human:** Requires live gRPC channel between two running services and a student with group membership.

---

## Build and Test Summary

| Check | Result |
|-------|--------|
| `dotnet build Edvantix.Scheduling.csproj` | PASS — 0 errors, 0 warnings |
| `dotnet build Edvantix.Organizations.csproj` | PASS — 0 errors, 0 warnings |
| `dotnet build Edvantix.AppHost.csproj` | PASS — 0 errors, 0 warnings |
| `dotnet test Edvantix.Scheduling.UnitTests.csproj` | PASS — 11/11 |
| `dotnet test Edvantix.ArchTests.csproj` | PASS — 65/65 |

---

_Verified: 2026-03-21_
_Verifier: Claude (gsd-verifier)_
