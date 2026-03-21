---
phase: 04-scheduling-attendance-outbox
plan: "03"
subsystem: api
tags: [scheduling, attendance, mediator, endpoint, minimal-api]

requires:
  - phase: 04-01
    provides: AttendanceRecord aggregate, IAttendanceRecordRepository with GetBySlotAsync, SchedulingDbContext tenant filter for AttendanceRecord

provides:
  - GET /v1/slots/{slotId}/attendance endpoint returning list of AttendanceRecordDto
  - AttendanceRecordDto with string Status, Guid CorrelationId, DateTimeOffset MarkedAt
  - GetSlotAttendanceQuery / GetSlotAttendanceQueryHandler (read side of SCH-08)

affects:
  - 04-02
  - 05-payments

tech-stack:
  added: []
  patterns:
    - "IQuery<IReadOnlyList<T>> for collection-returning queries via Mediator"
    - "Tenant isolation for attendance reads applied automatically via HasQueryFilter from Plan 04-01"
    - "Status.ToString() as DTO projection — enum-to-string at the application boundary per D-07"

key-files:
  created:
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/GetSlotAttendance/AttendanceRecordDto.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/GetSlotAttendance/GetSlotAttendanceQuery.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/GetSlotAttendance/GetSlotAttendanceQueryHandler.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/GetSlotAttendance/GetSlotAttendanceEndpoint.cs
  modified: []

key-decisions:
  - "ViewSchedule permission reused for GET attendance endpoint (D-09) — no separate view-attendance permission needed, managers and teachers who can view the schedule can also view attendance for those slots"
  - "AttendanceRecordDto uses string Status (not enum) per D-07 response shape — API consumers receive human-readable values without needing enum numeric mapping"
  - "Query handler is stateless with single repository dependency — no permission-based filtering needed since endpoint authorization gate handles access control at the slot level"

patterns-established:
  - "GetSlotAttendance pattern: route param → query record → handler calls GetBySlotAsync → maps domain entity to DTO"

requirements-completed:
  - SCH-08

duration: 2min
completed: "2026-03-21"
---

# Phase 4 Plan 03: GetSlotAttendance Summary

**GET /v1/slots/{slotId}/attendance endpoint with IReadOnlyList<AttendanceRecordDto> response, string Status projection, and ViewSchedule authorization gate**

## Performance

- **Duration:** 2 min
- **Started:** 2026-03-21T18:42:46Z
- **Completed:** 2026-03-21T18:43:56Z
- **Tasks:** 1
- **Files modified:** 4

## Accomplishments

- GET /v1/slots/{slotId}/attendance endpoint returning all attendance records for a slot
- AttendanceRecordDto with string Status, Guid CorrelationId, DateTimeOffset MarkedAt per D-07
- Query handler maps AttendanceRecord domain entities to DTOs via Status.ToString()
- Endpoint protected by scheduling.view-schedule permission per D-09
- All 65 arch tests pass (query name starts with "Get", satisfies arch rule)

## Task Commits

Each task was committed atomically:

1. **Task 1: GetSlotAttendance query, handler, DTO, and endpoint** - `998b13d` (feat)

## Files Created/Modified

- `src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/GetSlotAttendance/AttendanceRecordDto.cs` - DTO record with StudentId, string Status, CorrelationId, MarkedAt
- `src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/GetSlotAttendance/GetSlotAttendanceQuery.cs` - IQuery<IReadOnlyList<AttendanceRecordDto>> with SlotId param
- `src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/GetSlotAttendance/GetSlotAttendanceQueryHandler.cs` - Fetches via GetBySlotAsync, maps to DTO
- `src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/GetSlotAttendance/GetSlotAttendanceEndpoint.cs` - Minimal API endpoint at /v1/slots/{slotId}/attendance

## Decisions Made

- Reused `scheduling.view-schedule` for GET attendance (D-09) — managers and teachers who can view the schedule implicitly have access to attendance records for those slots; no separate permission needed in v1
- Status exposed as string in DTO per D-07 response shape — avoids enum numeric leakage to API consumers
- No additional permission-based filtering inside the handler — the endpoint-level ViewSchedule gate is sufficient; data is already scoped to the tenant by the HasQueryFilter registered in Plan 04-01

## Deviations from Plan

None — plan executed exactly as written.

## Issues Encountered

None.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- Read side of SCH-08 (attendance viewing) is complete
- Plan 04-02 (MarkAttendance command, outbox) can proceed independently
- AttendanceRecordDto shape is stable and ready for use by any future Payments integration

## Self-Check: PASSED

- FOUND: AttendanceRecordDto.cs
- FOUND: GetSlotAttendanceQuery.cs
- FOUND: GetSlotAttendanceQueryHandler.cs
- FOUND: GetSlotAttendanceEndpoint.cs
- FOUND: commit 998b13d
