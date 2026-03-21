---
phase: 03-scheduling-slots-and-views
plan: "04"
subsystem: api
tags: [scheduling, lesson-slots, cqrs, grpc, http-client, conflict-detection, fluent-validation]

requires:
  - phase: 03-02
    provides: "LessonSlot aggregate, ILessonSlotRepository with HasConflictAsync, SchedulingDbContext with tenant query filter"
  - phase: 03-01
    provides: "SchedulingPermissions constants, PermissionSeeder, Organizations HttpClient registration"

provides:
  - "POST /v1/lesson-slots — creates a slot after validating group existence and checking teacher conflict"
  - "PUT /v1/lesson-slots/{id} — edits a slot with self-exclusion conflict check (D-06)"
  - "DELETE /v1/lesson-slots/{id} — hard-deletes a slot"
  - "IOrganizationsGroupService — HTTP-based cross-service group validation (D-15)"

affects:
  - 03-05
  - 03-06
  - 03-09

tech-stack:
  added: []
  patterns:
    - "IOrganizationsGroupService: interface+HTTP-impl pattern for cross-service validation (v1 HTTP fallback, gRPC in Plan 03-09)"
    - "HasConflictAsync with excludedSlotId=null for Create, excludedSlotId=command.Id for Edit (D-06 self-exclusion)"
    - "ValidationException with ValidationFailure list for domain-level conflict errors mapped to 422"
    - "NotFoundException.For<T>() for aggregate lookup failures mapped to 404"
    - "EditLessonSlotRequest record as request body — Id comes from route, body carries mutable fields"

key-files:
  created:
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Services/IOrganizationsGroupService.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Services/OrganizationsGroupService.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/CreateLessonSlot/CreateLessonSlotCommand.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/CreateLessonSlot/CreateLessonSlotCommandHandler.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/CreateLessonSlot/CreateLessonSlotCommandValidator.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/CreateLessonSlot/CreateLessonSlotEndpoint.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/EditLessonSlot/EditLessonSlotCommand.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/EditLessonSlot/EditLessonSlotCommandHandler.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/EditLessonSlot/EditLessonSlotCommandValidator.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/EditLessonSlot/EditLessonSlotEndpoint.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/DeleteLessonSlot/DeleteLessonSlotCommand.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/LessonSlots/DeleteLessonSlot/DeleteLessonSlotEndpoint.cs
  modified:
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Extensions.cs

key-decisions:
  - "OrganizationsGroupService uses HTTP GET /v1/groups/{id} as v1 fallback — Organizations gRPC does not yet expose GetGroupAsync; Plan 03-09 will add the gRPC method and swap the implementation behind IOrganizationsGroupService"
  - "IOrganizationsGroupService registered as singleton using the named 'organizations' HttpClient (already configured with StandardResilienceHandler in Extensions/Extensions.cs)"
  - "Conflict errors thrown as ValidationException(IEnumerable<ValidationFailure>) so ValidationExceptionHandler maps them to HTTP 422 with per-field error structure"
  - "EditLessonSlotCommandHandler conditionally calls ChangeTeacher/Reschedule only when values differ — avoids spurious domain events in future phases"
  - "DeleteLessonSlotCommand + handler colocated in one file following the Organizations DeleteRole pattern for small commands with no validator"

patterns-established:
  - "Cross-service validation pattern: IXxxService interface + HTTP/gRPC impl, registered as singleton, injected into command handler"
  - "Self-exclusion conflict check: pass command.Id as excludedSlotId when editing, null when creating"
  - "PUT endpoint uses request body record for mutable fields; Id comes from route parameter"

requirements-completed: [SCH-01, SCH-02, SCH-10]

duration: 2min
completed: 2026-03-21
---

# Phase 03 Plan 04: LessonSlot CED Commands Summary

**POST/PUT/DELETE /v1/lesson-slots with cross-service group validation (HTTP fallback), global teacher conflict detection, and self-exclusion on edit — protected by scheduling permissions**

## Performance

- **Duration:** 2 min
- **Started:** 2026-03-21T12:02:17Z
- **Completed:** 2026-03-21T12:04:37Z
- **Tasks:** 2
- **Files modified:** 13 (12 created, 1 modified)

## Accomplishments

- Created `IOrganizationsGroupService` with HTTP-based `GroupExistsAsync` using the existing `"organizations"` named client; registered in `Grpc/Extensions.cs` as singleton alongside `PersonaProfileService`
- Implemented CreateLessonSlot (POST /lesson-slots): validates group via Organizations HTTP, checks global teacher conflict with `excludedSlotId=null`, creates `LessonSlot` aggregate
- Implemented EditLessonSlot (PUT /lesson-slots/{id}): self-exclusion conflict check passes `command.Id`, conditionally calls `ChangeTeacher`/`Reschedule` only when values differ
- Implemented DeleteLessonSlot (DELETE /lesson-slots/{id}): hard-delete via `ILessonSlotRepository.Remove`; all 65 architecture tests pass

## Task Commits

Each task was committed atomically:

1. **Task 1: Create Organizations gRPC group service client and CreateLessonSlot command** - `002cfeb` (feat)
2. **Task 2: EditLessonSlot and DeleteLessonSlot commands with self-exclusion conflict check** - `d95f65e` (feat)

**Plan metadata:** (docs commit follows)

## Files Created/Modified

- `Grpc/Services/IOrganizationsGroupService.cs` — abstraction for cross-service group validation (D-15)
- `Grpc/Services/OrganizationsGroupService.cs` — HTTP GET /v1/groups/{id} implementation; marked `[ExcludeFromCodeCoverage]`
- `Grpc/Extensions.cs` — added `IOrganizationsGroupService` singleton registration
- `Features/LessonSlots/CreateLessonSlot/CreateLessonSlotCommand.cs` — GroupId, TeacherId, StartTime, EndTime (DateTimeOffset)
- `Features/LessonSlots/CreateLessonSlot/CreateLessonSlotCommandHandler.cs` — group validation → conflict check (null) → create
- `Features/LessonSlots/CreateLessonSlot/CreateLessonSlotCommandValidator.cs` — non-empty GUIDs, EndTime > StartTime
- `Features/LessonSlots/CreateLessonSlot/CreateLessonSlotEndpoint.cs` — POST /lesson-slots, `RequireAuthorization(SchedulingPermissions.CreateLessonSlot)`
- `Features/LessonSlots/EditLessonSlot/EditLessonSlotCommand.cs` — Id, TeacherId, StartTime, EndTime (DateTimeOffset)
- `Features/LessonSlots/EditLessonSlot/EditLessonSlotCommandHandler.cs` — find slot → conflict check (command.Id) → conditional domain method calls
- `Features/LessonSlots/EditLessonSlot/EditLessonSlotCommandValidator.cs` — non-empty GUIDs, EndTime > StartTime
- `Features/LessonSlots/EditLessonSlot/EditLessonSlotEndpoint.cs` — PUT /lesson-slots/{id}, `EditLessonSlotRequest` body record
- `Features/LessonSlots/DeleteLessonSlot/DeleteLessonSlotCommand.cs` — command + handler colocated (small command pattern)
- `Features/LessonSlots/DeleteLessonSlot/DeleteLessonSlotEndpoint.cs` — DELETE /lesson-slots/{id}, `RequireAuthorization(SchedulingPermissions.DeleteLessonSlot)`

## Decisions Made

- `OrganizationsGroupService` uses HTTP (not gRPC) in v1 because Organizations does not yet expose `GetGroupAsync` via gRPC. Interface is stable — Plan 03-09 will swap the implementation.
- `IOrganizationsGroupService` registered as singleton (same as `IPersonaProfileService`) — `IHttpClientFactory` is thread-safe.
- Teacher conflict errors use `ValidationException(IEnumerable<ValidationFailure>)` (FluentValidation) so `ValidationExceptionHandler` maps them to HTTP 422 with structured field-level errors.
- `EditLessonSlotCommandHandler` conditionally calls `slot.ChangeTeacher` / `slot.Reschedule` only when values actually differ — avoids triggering unnecessary domain events in Phase 4.
- `DeleteLessonSlotCommand` and its handler colocated in one file (following Organizations `DeleteRoleCommand` pattern) — no validator needed for a single-ID delete.

## Deviations from Plan

None — plan executed exactly as written.

## Issues Encountered

None.

## User Setup Required

None — no external service configuration required.

## Next Phase Readiness

- POST/PUT/DELETE /lesson-slots endpoints are functional and protected; Plan 03-05/03-06 can build GET endpoints on top
- `IOrganizationsGroupService` interface is stable; Plan 03-09 will provide gRPC implementation without touching any feature code
- All 65 architecture tests pass with the new feature structure

---
*Phase: 03-scheduling-slots-and-views*
*Completed: 2026-03-21*
