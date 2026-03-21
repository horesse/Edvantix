---
phase: 04-scheduling-attendance-outbox
plan: 01
subsystem: database
tags: [efcore, postgres, ddd, attendance, permissions]

requires:
  - phase: 03-scheduling-slots-and-views
    provides: SchedulingDbContext, LessonSlot aggregate, HasQueryFilter tenant pattern, SchedulingPermissions

provides:
  - AttendanceRecord aggregate root with CorrelationId (Guid v7) and UpdateStatus
  - AttendanceStatus enum (Present/Absent/Excused/Late)
  - IAttendanceRecordRepository with FindBySlotAndStudentAsync and GetBySlotAsync
  - AttendanceRecordRepository EF Core implementation
  - AttendanceRecordConfiguration with unique index (LessonSlotId, StudentId)
  - HasQueryFilter for AttendanceRecord in SchedulingDbContext
  - scheduling.mark-attendance permission constant and authorization policy
  - 6 passing aggregate unit tests; 2 Wave 0 handler stubs (skipped for Plan 02)

affects: [04-02, 04-03, 05-payments]

tech-stack:
  added: []
  patterns:
    - AttendanceRecord follows same Entity/IAggregateRoot/ITenanted pattern as LessonSlot
    - HasQueryFilter for new tenanted entity registered in SchedulingDbContext.OnModelCreating
    - CorrelationId generated with Guid.CreateVersion7() at aggregate construction, preserved on updates
    - Unique composite index (LessonSlotId, StudentId) enforces one-record-per-student-per-slot at DB level

key-files:
  created:
    - src/Services/Scheduling/Edvantix.Scheduling/Domain/AggregatesModel/AttendanceAggregate/AttendanceRecord.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Domain/AggregatesModel/AttendanceAggregate/AttendanceStatus.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Domain/AggregatesModel/AttendanceAggregate/IAttendanceRecordRepository.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/EntityConfigurations/AttendanceRecordConfiguration.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/Repositories/AttendanceRecordRepository.cs
    - tests/Edvantix.Scheduling.UnitTests/Domain/AttendanceRecordAggregateTests.cs
    - tests/Edvantix.Scheduling.UnitTests/Features/MarkAttendanceCommandHandlerTests.cs
  modified:
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/SchedulingDbContext.cs
    - src/BuildingBlocks/Edvantix.Constants/Permissions/SchedulingPermissions.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Extensions/Extensions.cs
    - tests/Edvantix.Scheduling.UnitTests/Features/GetScheduleQueryHandlerTests.cs

key-decisions:
  - "AttendanceRecord is a separate aggregate from LessonSlot (D-01) — own repository, own lifecycle, plain Guid cross-reference to LessonSlotId"
  - "CorrelationId uses Guid.CreateVersion7() at construction, never changed on UpdateStatus — preserves event chain identity per D-04"
  - "HasQueryFilter for AttendanceRecord registered in SchedulingDbContext.OnModelCreating (not entity config) to use injected ITenantContext — same pattern as LessonSlot"
  - "Domain events (AttendanceRecordedEvent) deferred to Plan 02 with TODO comments — avoids forward reference to non-existent type"
  - "[Rule 1 - Bug] GetScheduleQueryHandlerTests missing IMapper mock after handler signature changed in Phase 03 — fixed by adding Mock<IMapper<ScheduleSlotMappingContext, ScheduleSlotDto>> that delegates to real ScheduleSlotMapper"

patterns-established:
  - "New tenanted aggregates: add DbSet + HasQueryFilter in SchedulingDbContext.OnModelCreating after existing filters"
  - "New permissions: add const + XML doc + update All list in SchedulingPermissions, add AddPolicy chain in Extensions.cs"
  - "IAttendanceRecordRepository follows ILessonSlotRepository pattern with auto-discovery via Scrutor AddRepositories scan"

requirements-completed: [SCH-08]

duration: 10min
completed: 2026-03-21
---

# Phase 04 Plan 01: AttendanceRecord Aggregate and Repository Summary

**AttendanceRecord aggregate root with CorrelationId (Guid v7), unique (LessonSlotId, StudentId) index, HasQueryFilter tenant isolation, and scheduling.mark-attendance permission wired end-to-end**

## Performance

- **Duration:** ~10 min
- **Started:** 2026-03-21T18:29:00Z
- **Completed:** 2026-03-21T18:39:20Z
- **Tasks:** 2
- **Files modified:** 11 (7 created, 4 modified)

## Accomplishments

- AttendanceRecord aggregate with 6 properties (SchoolId, LessonSlotId, StudentId, Status, CorrelationId, MarkedAt), Guard.Against.Default validation on all Guid parameters, and UpdateStatus method preserving CorrelationId
- AttendanceRecordConfiguration with unique composite index on (LessonSlotId, StudentId) and `timestamp with time zone` for MarkedAt
- HasQueryFilter for AttendanceRecord in SchedulingDbContext prevents cross-tenant data leakage; arch tests (65) all green
- `scheduling.mark-attendance` permission added to SchedulingPermissions.All and authorization policy wired in Extensions.cs
- 6 aggregate unit tests passing, 2 Wave 0 handler stubs created with [Skip] for Plan 02

## Task Commits

Each task was committed atomically:

1. **Task 1: AttendanceRecord aggregate, EF config, repository, permission, and auth policy** - `3509141` (feat)
2. **Task 2: Wave 0 test stubs for MarkAttendanceCommandHandler** - `33bfd00` (test)

## Files Created/Modified

- `Domain/AggregatesModel/AttendanceAggregate/AttendanceRecord.cs` - Aggregate root with 6 properties and UpdateStatus
- `Domain/AggregatesModel/AttendanceAggregate/AttendanceStatus.cs` - Four-value enum per D-03
- `Domain/AggregatesModel/AttendanceAggregate/IAttendanceRecordRepository.cs` - Repository interface with FindBySlotAndStudentAsync and GetBySlotAsync
- `Infrastructure/EntityConfigurations/AttendanceRecordConfiguration.cs` - EF config with unique index
- `Infrastructure/Repositories/AttendanceRecordRepository.cs` - EF implementation, Scrutor auto-discovered
- `Infrastructure/SchedulingDbContext.cs` - Added DbSet<AttendanceRecord> and HasQueryFilter
- `Edvantix.Constants/Permissions/SchedulingPermissions.cs` - Added MarkAttendance const and updated All list
- `Extensions/Extensions.cs` - Added AddPolicy for MarkAttendance
- `tests/Domain/AttendanceRecordAggregateTests.cs` - 6 aggregate tests (all passing)
- `tests/Features/MarkAttendanceCommandHandlerTests.cs` - 2 Wave 0 stubs (skipped for Plan 02)
- `tests/Features/GetScheduleQueryHandlerTests.cs` - Fixed missing IMapper mock (pre-existing bug)

## Decisions Made

- AttendanceRecord domain events deferred to Plan 02 with `// TODO: Plan 02` comments — creating AttendanceRecordedEvent now would require forward-referencing a non-existent type
- CorrelationId uses `Guid.CreateVersion7()` for DB index locality (monotonically increasing within same millisecond window)
- Repository auto-discovered by existing Scrutor `AddRepositories` scan — no manual DI registration needed

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Fixed GetScheduleQueryHandlerTests missing IMapper mock**

- **Found during:** Task 1 (aggregate implementation — ran existing tests to verify baseline)
- **Issue:** `GetScheduleQueryHandler` constructor was extended with `IMapper<ScheduleSlotMappingContext, ScheduleSlotDto>` in Phase 03 Plan 05 but the test `CreateHandler` method was not updated, causing CS7036 compile error
- **Fix:** Added `Mock<IMapper<ScheduleSlotMappingContext, ScheduleSlotDto>> _mapperMock` that delegates to real `ScheduleSlotMapper` for accurate assertions
- **Files modified:** `tests/Edvantix.Scheduling.UnitTests/Features/GetScheduleQueryHandlerTests.cs`
- **Verification:** All 17 existing tests continue to pass after fix
- **Committed in:** `3509141` (Task 1 commit)

---

**Total deviations:** 1 auto-fixed (Rule 1 - Bug)
**Impact on plan:** Bug fix was blocking test suite compilation; no scope creep.

## Issues Encountered

None — plan executed cleanly after auto-fixing the pre-existing test compilation error.

## Next Phase Readiness

- AttendanceRecord aggregate is production-ready for Plan 02 command/query handlers
- Domain event registration points are marked with `// TODO: Plan 02` comments
- Wave 0 handler stubs serve as acceptance criteria specifications for Plan 02
- HasQueryFilter in place — SchedulingTenantIsolationTests arch tests remain green

---

*Phase: 04-scheduling-attendance-outbox*
*Completed: 2026-03-21*
