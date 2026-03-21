---
phase: 03-scheduling-slots-and-views
plan: 02
subsystem: database
tags: [efcore, domain-model, tunit, archunit, tenant-isolation, scheduling]

requires:
  - phase: 03-01
    provides: SchedulingDbContext shell, PermissionSeeder, service infrastructure
  - phase: 01-01
    provides: ITenanted interface pattern, HasQueryFilter convention, SharedKernel Entity base class

provides:
  - LessonSlot domain aggregate (GroupId as plain Guid, DateTimeOffset StartTime/EndTime)
  - ILessonSlotRepository with HasConflictAsync using IgnoreQueryFilters for global conflict check
  - LessonSlotConfiguration EF config (lesson_slots table, timestamp with time zone columns)
  - LessonSlotRepository with cross-tenant conflict detection
  - SchedulingDbContext updated with LessonSlots DbSet and single HasQueryFilter for tenant isolation
  - ITenanted promoted to SharedKernel.SeedWork (no longer Organizations-only)
  - Edvantix.Scheduling.UnitTests project with 7 passing LessonSlot aggregate tests
  - SchedulingTenantIsolationTests arch test confirming 1 ITenanted entity has HasQueryFilter

affects:
  - 03-03: CreateLessonSlot command handler depends on LessonSlot aggregate and ILessonSlotRepository
  - 03-04: GetScheduleQuery depends on SchedulingDbContext.LessonSlots with tenant filter
  - 03-05: LessonSlot update/delete endpoints depend on LessonSlotRepository.Remove
  - future Scheduling plans that add ITenanted entities must add HasQueryFilter

tech-stack:
  added: []
  patterns:
    - LessonSlot aggregate with plain Guid cross-service reference (no navigation property, D-15)
    - IgnoreQueryFilters for global cross-tenant conflict check (D-04)
    - DateTimeOffset for all time properties with timestamp with time zone EF mapping (SCH-10)
    - ITenanted promoted to SharedKernel so multiple services can use without Organizations dependency
    - global using alias in Organizations/Domain/Abstractions/ITenanted.cs for backward compat

key-files:
  created:
    - src/BuildingBlocks/Edvantix.SharedKernel/SeedWork/ITenanted.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Domain/AggregatesModel/LessonSlotAggregate/LessonSlot.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Domain/AggregatesModel/LessonSlotAggregate/ILessonSlotRepository.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/EntityConfigurations/LessonSlotConfiguration.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/Repositories/LessonSlotRepository.cs
    - tests/Edvantix.ArchTests/Domain/SchedulingTenantIsolationTests.cs
    - tests/Edvantix.Scheduling.UnitTests/Edvantix.Scheduling.UnitTests.csproj
    - tests/Edvantix.Scheduling.UnitTests/GlobalUsings.cs
    - tests/Edvantix.Scheduling.UnitTests/Domain/LessonSlotAggregateTests.cs
  modified:
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/SchedulingDbContext.cs
    - src/Services/Scheduling/Edvantix.Scheduling/GlobalUsings.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/Abstractions/ITenanted.cs
    - src/Services/Organizations/Edvantix.Organizations/GlobalUsings.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/GlobalUsings.cs
    - tests/Edvantix.ArchTests/Edvantix.ArchTests.csproj
    - tests/Edvantix.ArchTests/Abstractions/BaseTest.cs
    - tests/Edvantix.ArchTests/Abstractions/ArchUnitBaseTest.cs
    - tests/Edvantix.ArchTests/Domain/TenantIsolationTests.cs
    - Edvantix.slnx

key-decisions:
  - "ITenanted promoted to Edvantix.SharedKernel.SeedWork (from Organizations.Domain.Abstractions) — Scheduling needs tenant isolation without taking Organizations assembly dependency. Organizations.Domain.Abstractions.ITenanted.cs replaced with global using alias for backward compat."
  - "LessonSlotRepository.HasConflictAsync uses IgnoreQueryFilters for cross-tenant teacher conflict check (D-04) — a teacher cannot be double-booked even across schools"
  - "LessonSlot uses DateTimeOffset for StartTime/EndTime mapped to timestamp with time zone in PostgreSQL (SCH-10 compliance)"
  - "GroupId is plain Guid with no navigation property — Group aggregate lives in Organizations service (D-15)"
  - "No ISoftDelete on LessonSlot — lesson slots are hard-deleted, single HasQueryFilter for tenant-only"

patterns-established:
  - "Cross-service reference: plain Guid field with no navigation property, documented with D-xx reference in comment"
  - "DateTimeOffset + HasColumnType(timestamp with time zone) for all time properties in PostgreSQL"
  - "IgnoreQueryFilters in repository for cross-tenant queries — applies to conflict checks and gRPC paths"
  - "ITenanted in SharedKernel, not per-service — enables multi-service tenant isolation with shared arch test pattern"

requirements-completed: [SCH-01, SCH-10]

duration: 13min
completed: 2026-03-21
---

# Phase 3 Plan 02: LessonSlot Domain Aggregate Summary

**LessonSlot EF aggregate with DateTimeOffset times, cross-tenant conflict detection via IgnoreQueryFilters, ITenanted promoted to SharedKernel, and 65 passing arch tests including Scheduling tenant isolation**

## Performance

- **Duration:** 13 min
- **Started:** 2026-03-21T11:45:16Z
- **Completed:** 2026-03-21T11:58:00Z
- **Tasks:** 2
- **Files modified:** 19

## Accomplishments

- LessonSlot domain aggregate with GroupId as plain Guid (D-15: Groups in Organizations service), DateTimeOffset StartTime/EndTime (SCH-10), Reschedule and ChangeTeacher methods with Guard validation
- LessonSlotRepository with HasConflictAsync using `IgnoreQueryFilters()` for global cross-tenant teacher conflict detection (D-04)
- ITenanted interface promoted from Organizations to SharedKernel so Scheduling can implement tenant isolation without an Organizations dependency; backward-compat alias in Organizations
- 7 passing LessonSlot unit tests and 65 passing arch tests (including new SchedulingTenantIsolationTests confirming 1 ITenanted entity has HasQueryFilter)

## Task Commits

Each task was committed atomically:

1. **Task 1: Create LessonSlot domain aggregate, EF config, repository, and update SchedulingDbContext** - `cd286ec` (feat)
2. **Task 2: Create architecture tests, unit test project, and LessonSlot domain tests** - `6c3d243` (feat)

**Plan metadata:** (see final metadata commit)

## Files Created/Modified

- `src/BuildingBlocks/Edvantix.SharedKernel/SeedWork/ITenanted.cs` - ITenanted promoted from Organizations to SharedKernel
- `src/Services/Scheduling/Edvantix.Scheduling/Domain/AggregatesModel/LessonSlotAggregate/LessonSlot.cs` - LessonSlot aggregate root
- `src/Services/Scheduling/Edvantix.Scheduling/Domain/AggregatesModel/LessonSlotAggregate/ILessonSlotRepository.cs` - Repository interface with HasConflictAsync
- `src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/EntityConfigurations/LessonSlotConfiguration.cs` - EF config with lesson_slots table
- `src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/Repositories/LessonSlotRepository.cs` - Repository with IgnoreQueryFilters conflict check
- `src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/SchedulingDbContext.cs` - Added LessonSlots DbSet and HasQueryFilter
- `src/Services/Scheduling/Edvantix.Scheduling/GlobalUsings.cs` - Added LessonSlot aggregate namespace
- `src/Services/Organizations/Edvantix.Organizations/Domain/Abstractions/ITenanted.cs` - Replaced with global using alias to SharedKernel
- `tests/Edvantix.ArchTests/Domain/SchedulingTenantIsolationTests.cs` - Arch test for Scheduling tenant isolation
- `tests/Edvantix.Scheduling.UnitTests/Domain/LessonSlotAggregateTests.cs` - 7 unit tests for LessonSlot behavior

## Decisions Made

- **ITenanted to SharedKernel:** Needed by Scheduling without Organizations dependency. Organizations keeps backward-compat via `global using ITenanted = Edvantix.SharedKernel.SeedWork.ITenanted;`. All arch tests updated to use SharedKernel version.
- **HasConflictAsync uses IgnoreQueryFilters:** Teacher cannot be double-booked across schools. Cross-tenant conflict check is a correctness requirement (D-04).
- **DateTimeOffset + timestamp with time zone:** SCH-10 compliance — no DateTime.Now/UtcNow anywhere in Scheduling service.
- **No ISoftDelete on LessonSlot:** Slots are hard-deleted. Single HasQueryFilter (tenant-only) satisfies the arch test.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Updated Organizations ITenanted.cs and GlobalUsings for ambiguity resolution**
- **Found during:** Task 2 (ArchTests build)
- **Issue:** Adding ITenanted to SharedKernel created CS0104 ambiguity in Organizations project (both namespaces in scope)
- **Fix:** Replaced Organizations `ITenanted.cs` with a project-wide `global using` alias pointing to SharedKernel version; removed `using Edvantix.Organizations.Domain.Abstractions` from Organizations GlobalUsings and UnitTests GlobalUsings; updated TenantIsolationTests to import `Edvantix.SharedKernel.SeedWork` instead
- **Files modified:** Organizations/Domain/Abstractions/ITenanted.cs, Organizations/GlobalUsings.cs, Organizations.UnitTests/GlobalUsings.cs, ArchTests/Domain/TenantIsolationTests.cs
- **Verification:** All 65 arch tests pass, Organizations builds with 0 warnings
- **Committed in:** 6c3d243 (Task 2 commit)

---

**Total deviations:** 1 auto-fixed (Rule 1 — bug/ambiguity)
**Impact on plan:** Fix required for compile correctness. ITenanted migration was always the intent (STATE.md decision: "moved to SharedKernel if Scheduling needs it"). No scope creep.

## Issues Encountered

- TUnit uses `--treenode-filter` not `--filter`. The plan's verification command `dotnet test --filter "SchedulingTenantIsolation"` would produce "zero tests run" with TUnit. Tests were verified via full test suite run (65/65 passing). This is a known TUnit CLI difference.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- LessonSlot domain model complete and compiling
- SchedulingDbContext has LessonSlots DbSet with tenant filter — ready for EF migrations
- ILessonSlotRepository ready for command handler injection (Plan 03)
- All architecture tests passing — any future ITenanted entity in Scheduling will be caught by SchedulingTenantIsolationTests
- No stubs — all code is wired and passing tests

## Self-Check: PASSED

- All key files found on disk
- Both commits (cd286ec, 6c3d243) verified in git log
- 65/65 arch tests passing
- 7/7 unit tests passing
- 0 build warnings/errors

---
*Phase: 03-scheduling-slots-and-views*
*Completed: 2026-03-21*
