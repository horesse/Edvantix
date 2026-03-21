---
phase: 03-scheduling-slots-and-views
plan: "05"
subsystem: api
tags: [scheduling, schedule, query, grpc, permission-filtering, cqrs, tdd, unit-tests]

requires:
  - phase: 03-04
    provides: "LessonSlot CED commands, IOrganizationsGroupService (GroupExistsAsync), OrganizationsGroupService HTTP impl"
  - phase: 03-09
    provides: "GroupMembership in Organizations — GetGroupsForStudentAsync endpoint exists behind HTTP fallback"

provides:
  - "GET /v1/schedule?dateFrom=X&dateTo=Y — returns permission-filtered ScheduleSlotDto list"
  - "ScheduleSlotDto with nullable TeacherId/TeacherName/StudentCount for permission-based data shaping"
  - "GetScheduleQueryHandler: two gRPC permission checks (manager=CreateLessonSlot proxy, teacher=ViewOwnSchedule) then EF query"
  - "scheduling.view-own-schedule permission constant for permission-based teacher detection"
  - "IOrganizationsGroupService.GetGroupsForStudentAsync and GetGroupAsync methods"

affects:
  - 03-06

tech-stack:
  added: []
  patterns:
    - "Permission-based role detection in query handler: two sequential gRPC CheckPermission calls (manager proxy then teacher marker)"
    - "Student group resolution: GetGroupsForStudentAsync via IOrganizationsGroupService then Contains filter on EF query"
    - "N+1 avoidance for group info: collect distinct GroupIds → Task.WhenAll → dictionary lookup"
    - "TDD cycle: failing tests committed first (RED), implementation added (GREEN)"
    - "GrpcTestHelpers.CreateUnaryCall<T> for Moq-compatible AsyncUnaryCall test doubles"

key-files:
  created:
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Schedule/GetSchedule/ScheduleSlotDto.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Schedule/GetSchedule/GetScheduleQuery.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Schedule/GetSchedule/GetScheduleQueryHandler.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Schedule/GetSchedule/GetScheduleEndpoint.cs
    - tests/Edvantix.Scheduling.UnitTests/Features/GetScheduleQueryHandlerTests.cs
    - tests/Edvantix.Scheduling.UnitTests/Features/GrpcTestHelpers.cs
  modified:
    - src/BuildingBlocks/Edvantix.Constants/Permissions/SchedulingPermissions.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Extensions/Extensions.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Services/IOrganizationsGroupService.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Services/OrganizationsGroupService.cs
    - tests/Edvantix.Scheduling.UnitTests/Edvantix.Scheduling.UnitTests.csproj

key-decisions:
  - "ViewOwnSchedule permission used as teacher marker (not data-driven slot query) — a teacher with zero slots in the queried range would be misidentified as student if detection were data-driven"
  - "Manager proxy check uses CreateLessonSlot permission — managers already hold this and no separate is-manager permission exists; consistent with D-08"
  - "StudentCount set to 0 (not null) for manager/teacher in v1 — actual attendance count deferred to Phase 4 when attendance data exists"
  - "TeacherName is TeacherId.ToString() in v1 — Persona gRPC name resolution deferred per plan spec"
  - "GetGroupAsync called in parallel via Task.WhenAll on distinct GroupIds to avoid N+1"
  - "GrpcTestHelpers.CreateUnaryCall helper added to test project — AsyncUnaryCall constructor requires 5 args including status/headers/trailers delegates"

patterns-established:
  - "Permission-branching pattern in query handlers: isManager → isTeacher → isStudent waterfall"
  - "Test helper for gRPC client mocking via AsyncUnaryCall constructor without running gRPC server"

requirements-completed: [SCH-03, SCH-04, SCH-05]

duration: 8min
completed: 2026-03-21
---

# Phase 03 Plan 05: Permission-Filtered Schedule Query Summary

**GET /schedule endpoint with manager/teacher/student permission branches, Organizations gRPC group membership resolution, and unit tests covering all three roles**

## Performance

- **Duration:** 8 min
- **Started:** 2026-03-21T12:12:19Z
- **Completed:** 2026-03-21T12:20:21Z
- **Tasks:** 2
- **Files modified:** 10 (6 created, 4 modified)

## Accomplishments

- Added `scheduling.view-own-schedule` permission constant to `SchedulingPermissions` (now 5 total) and registered authorization policy
- Extended `IOrganizationsGroupService` with `GetGroupsForStudentAsync` and `GetGroupAsync`; added `GroupInfoDto` record; updated HTTP implementation
- Implemented `GetScheduleQueryHandler` with two sequential gRPC calls: manager check (`CreateLessonSlot` proxy) then teacher check (`ViewOwnSchedule`); student path resolves groups via `GetGroupsForStudentAsync`
- Created `ScheduleSlotDto` with nullable fields for permission-based data shaping per D-09
- Implemented `GET /schedule` endpoint with `RequireAuthorization(ViewSchedule)` baseline gate
- Added `GrpcTestHelpers` for creating `AsyncUnaryCall` test doubles
- All 4 `GetScheduleQueryHandlerTests` pass; all 65 architecture tests pass

## Task Commits

1. **Task 1: Add ViewOwnSchedule permission and extend IOrganizationsGroupService** - `dcc0711` (feat)
2. **Task 2 RED: Failing tests for GetScheduleQueryHandler** - `8e8865d` (test)
3. **Task 2 GREEN: Implement GET /schedule endpoint** - `45220d0` (feat)

## Files Created/Modified

- `SchedulingPermissions.cs` — added `ViewOwnSchedule` constant, updated `All` to 5 items
- `Extensions/Extensions.cs` — added `ViewOwnSchedule` authorization policy
- `Grpc/Services/IOrganizationsGroupService.cs` — added `GetGroupsForStudentAsync`, `GetGroupAsync`, `GroupInfoDto`
- `Grpc/Services/OrganizationsGroupService.cs` — implemented new interface methods with HTTP fallback
- `Features/Schedule/GetSchedule/ScheduleSlotDto.cs` — record with nullable TeacherId/TeacherName/StudentCount
- `Features/Schedule/GetSchedule/GetScheduleQuery.cs` — IQuery<List<ScheduleSlotDto>> with DateFrom/DateTo
- `Features/Schedule/GetSchedule/GetScheduleQueryHandler.cs` — two-gRPC-call permission waterfall, EF query, group info parallel resolution
- `Features/Schedule/GetSchedule/GetScheduleEndpoint.cs` — GET /schedule, ViewSchedule gate, MapToApiVersion(V1)
- `tests/Features/GetScheduleQueryHandlerTests.cs` — 4 tests covering manager, teacher, student, date range
- `tests/Features/GrpcTestHelpers.cs` — AsyncUnaryCall factory for Moq setup

## Decisions Made

- `ViewOwnSchedule` permission chosen over data-driven teacher detection: a teacher with no slots in the queried range would otherwise fall through to the student branch.
- Manager proxy: `CreateLessonSlot` permission used to identify managers — they already hold this permission and no dedicated is-manager permission exists.
- `StudentCount = 0` for manager/teacher in v1: actual attendance count requires Phase 4 attendance data; using 0 avoids null in fields that are semantically populated for these roles.
- `TeacherName = TeacherId.ToString()` in v1: Persona gRPC name lookup deferred per plan spec.
- `Task.WhenAll` on distinct GroupIds to avoid N+1 when resolving group display info.

## Deviations from Plan

None — plan executed exactly as written.

## Known Stubs

- `TeacherName` returns `TeacherId.ToString()` — full name resolution via Persona gRPC is a v1 placeholder. Noted in plan spec.
- `StudentCount` returns `0` for manager/teacher views — actual student count per group requires attendance data from Phase 4. Noted in plan spec.
- `OrganizationsGroupService.GetGroupsForStudentAsync` calls `GET /v1/groups/student/{profileId}?schoolId={schoolId}` — this HTTP endpoint must exist in the Organizations service. Plan 03-09 will replace with a gRPC call.

## Self-Check: PASSED

Files exist:
- `src/Services/Scheduling/Edvantix.Scheduling/Features/Schedule/GetSchedule/GetScheduleQueryHandler.cs` - FOUND
- `src/Services/Scheduling/Edvantix.Scheduling/Features/Schedule/GetSchedule/GetScheduleEndpoint.cs` - FOUND
- `tests/Edvantix.Scheduling.UnitTests/Features/GetScheduleQueryHandlerTests.cs` - FOUND

Commits verified:
- `dcc0711` - FOUND (feat: ViewOwnSchedule permission)
- `8e8865d` - FOUND (test: failing tests)
- `45220d0` - FOUND (feat: implementation)

All 11 unit tests pass, all 65 arch tests pass.

---
*Phase: 03-scheduling-slots-and-views*
*Completed: 2026-03-21*
