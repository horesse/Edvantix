---
phase: 04-scheduling-attendance-outbox
plan: "02"
subsystem: scheduling
tags: [domain-events, outbox, kafka, attendance, upsert, cqrs]
dependency_graph:
  requires: ["04-01"]
  provides: ["AttendanceRecordedIntegrationEvent", "MarkAttendance PUT endpoint"]
  affects: ["Phase 05 Payments — must subscribe to attendance-recorded-integration-event"]
tech_stack:
  added: []
  patterns: ["domain-event → EventMapper → outbox", "upsert via EF change tracking"]
key_files:
  created:
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/EventServices/Events/AttendanceRecordedEvent.cs
    - src/Services/Scheduling/Edvantix.Scheduling/IntegrationEvents/AttendanceRecordedIntegrationEvent.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/EventServices/EventMapper.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Domain/EventHandlers/AttendanceRecordedDomainEventHandler.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/MarkAttendance/MarkAttendanceCommand.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/MarkAttendance/MarkAttendanceCommandHandler.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/MarkAttendance/MarkAttendanceCommandValidator.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Features/Attendance/MarkAttendance/MarkAttendanceEndpoint.cs
    - tests/Edvantix.Scheduling.UnitTests/Infrastructure/EventMapperTests.cs
  modified:
    - src/Services/Scheduling/Edvantix.Scheduling/Domain/AggregatesModel/AttendanceAggregate/AttendanceRecord.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Extensions/Extensions.cs
    - tests/Edvantix.Scheduling.UnitTests/Features/MarkAttendanceCommandHandlerTests.cs
decisions:
  - "AttendanceRecordedEvent.Status is string (not enum) per D-12 — decouples integration event from internal enum"
  - "AttendanceRecordedIntegrationEvent uses namespace Edvantix.Contracts in Scheduling assembly — ensures DiscoverMessageTypes finds it during RegisterKafkaProducers scan"
  - "Kafka topic is attendance-recorded-integration-event — auto-derived by KebabCaseEndpointNameFormatter per D-11"
  - "Upsert uses EF change tracking (not ExecuteUpdate) so EventDispatchInterceptor captures domain events"
  - "MarkAttendanceEndpoint implements plain IEndpoint (not generic IEndpoint<T,T1...>) — 5 handler params exceed interface max of 4"
metrics:
  duration_minutes: 12
  completed_date: "2026-03-21"
  tasks_completed: 2
  files_changed: 12
---

# Phase 04 Plan 02: Domain Event Pipeline and MarkAttendance Command Summary

**One-liner:** Domain event pipeline (AttendanceRecordedEvent → EventMapper → AttendanceRecordedIntegrationEvent → MassTransit outbox → Kafka `attendance-recorded-integration-event`) wired with upsert PUT endpoint.

## What Was Built

### Task 1: Domain event pipeline

The complete pipeline from aggregate to Kafka outbox was wired:

- `AttendanceRecordedEvent` — domain event with Status as string (D-12), placed in `Infrastructure/EventServices/Events/` to pass arch tests (no infrastructure imports in domain folder)
- `AttendanceRecordedIntegrationEvent` — integration event in `namespace Edvantix.Contracts` within Scheduling assembly (same pattern as Organizations). Kafka topic: `attendance-recorded-integration-event` per D-11
- `EventMapper` — internal sealed class mapping the single domain event via switch expression
- `AttendanceRecordedDomainEventHandler` — mediator handler delegating to `IEventDispatcher`
- `AttendanceRecord.cs` updated — `RegisterDomainEvent` calls added in both constructor and `UpdateStatus`
- `Extensions.cs` updated — `AddScoped<IEventMapper, EventMapper>()` and `AddEventDispatcher()` registered
- 4 EventMapper unit tests covering all fields, string status, correlationId preservation, unknown event exception

### Task 2: MarkAttendance command and PUT endpoint

- `MarkAttendanceCommand` — record implementing `ICommand<Unit>` with SlotId, StudentId, Status
- `MarkAttendanceCommandHandler` — upsert logic using `FindBySlotAndStudentAsync` + `Add`/`UpdateStatus` (no `ExecuteUpdate`)
- `MarkAttendanceCommandValidator` — validates non-empty GUIDs and valid enum value
- `MarkAttendanceEndpoint` — `PUT /v1/slots/{slotId}/attendance/{studentId}` returning 200 OK, protected by `MarkAttendance` permission
- Handler tests rewritten (stubs removed): new record test verifies `Add` called once, existing record test verifies `UpdateStatus` called and `Add` not called

## Decisions Made

- `AttendanceRecordedEvent.Status` is string (not enum) per D-12 to decouple the integration event contract from Scheduling's internal `AttendanceStatus` enum. The aggregate converts via `Status.ToString()`.
- Kafka topic `attendance-recorded-integration-event` confirmed per D-11. Phase 5 Payments MUST subscribe to this exact topic name.
- Upsert uses EF change tracking (not `ExecuteUpdate`/`ExecuteUpdateAsync`) so the `EventDispatchInterceptor` can capture domain events during `SaveEntitiesAsync`.
- `MarkAttendanceEndpoint` implements plain `IEndpoint` instead of the generic variant because the handler lambda requires 5 parameters (slotId, studentId, request, sender, ct) which exceeds the interface's 4-param maximum.

## Test Results

- `dotnet test tests/Edvantix.Scheduling.UnitTests`: 23/23 passed (0 skipped — Plan 01 stubs implemented)
- `dotnet test tests/Edvantix.ArchTests`: 65/65 passed

## Deviations from Plan

None — plan executed exactly as written, with one minor implementation choice: `MarkAttendanceEndpoint` uses plain `IEndpoint` (not the typed variant) because the generic `IEndpoint<TResult, T1, T2, T3>` maximum is 3 request args and the endpoint needs slotId + studentId + request + ISender = 4. This is consistent with the Delete endpoint pattern in the same service.

## Known Stubs

None — all functionality is fully wired. The integration event will reach the outbox via the existing MassTransit `EventDispatchInterceptor` pipeline provisioned in Phase 3.

## Self-Check: PASSED
