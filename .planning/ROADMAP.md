# Roadmap: Edvantix

## Overview

Three new microservices are added to the existing .NET 10 / Aspire stack in strict dependency order. Phase 1 establishes multi-tenant RBAC in the Organizations service and the cross-cutting ITenantContext + EF Core query filter convention that all subsequent services inherit. Phase 2 adds the permission cache and role-change invalidation events, completing Organizations and unblocking Scheduling. Phase 3 builds the Scheduling service — groups, lesson slots, teacher/student views, conflict detection. Phase 4 adds attendance marking and the transactional outbox that produces AttendanceRecordedIntegrationEvent. Phase 5 closes the loop in the Payments service: append-only ledger, idempotent event consumer, and balance display.

## Phases

**Phase Numbering:**
- Integer phases (1, 2, 3): Planned milestone work
- Decimal phases (2.1, 2.2): Urgent insertions (marked with INSERTED)

Decimal phases appear between their surrounding integers in numeric order.

- [x] **Phase 1: Organizations — RBAC Core** - Custom roles, permissions, user-role assignments, and tenant isolation foundation for all services (completed 2026-03-18)
- [x] **Phase 2: Organizations — Permission Cache** - gRPC CheckPermission endpoint, HybridCache, and role-change invalidation events (completed 2026-03-21)
- [ ] **Phase 3: Scheduling — Slots and Views** - Groups, lesson slots with conflict detection, and manager/teacher/student calendar views
- [ ] **Phase 4: Scheduling — Attendance and Outbox** - Attendance marking with unique constraint and AttendanceRecordedIntegrationEvent via transactional outbox
- [ ] **Phase 5: Payments — Ledger and Balance** - Lesson package management, append-only ledger, idempotent event consumer, and balance display

## Phase Details

### Phase 1: Organizations — RBAC Core
**Goal**: School owners can create and manage custom roles, assign permissions to those roles, and assign roles to users — all tenant-isolated with an architecture-tested EF Core query filter convention that Scheduling and Payments will inherit
**Depends on**: Nothing (first phase)
**Requirements**: ORG-01, ORG-02, ORG-03, ORG-04, ORG-05, ORG-06, ORG-10
**Success Criteria** (what must be TRUE):
  1. School owner can create a named role, view all roles in their school, edit the name, and delete a role
  2. Owner can assign any combination of registered permission strings to a role, and the assignment is persisted and retrievable
  3. Services register their permission strings in the catalogue on startup, making them available for role assignment
  4. Owner can assign a role to a user within their school and revoke it; assignment and revocation are reflected immediately
  5. An architecture test fails the build if any ITenanted entity in any service has no EF Core global query filter registered for schoolId
**Plans**: 4 plans

Plans:
- [x] 01-01: ITenantContext abstraction, EF Core query filter convention, and architecture test
- [x] 01-02: Role and Permission domain model (aggregates, value objects, EF Core configuration)
- [x] 01-03: Manage roles — create, read, update, delete (commands, queries, API endpoints)
- [x] 01-04: User-role assignment — assign and revoke roles; permission string registration on startup

### Phase 2: Organizations — Permission Cache
**Goal**: Downstream services can resolve a user's permissions for a school in a single cached call, and the cache is evicted within seconds when a role or assignment changes
**Depends on**: Phase 1
**Requirements**: ORG-07, ORG-08, ORG-09
**Success Criteria** (what must be TRUE):
  1. Any service can call CheckPermission(userId, schoolId, permission) via gRPC and receive a correct bool response
  2. A permission check for a user/school combination that was already fetched does not issue a second database query within the cache TTL
  3. When an owner revokes a role from a user, a downstream service's cached permission for that user is invalidated and reflects the change on the next check (within the TTL fallback, maximum 60 seconds)
**Plans**: 3 plans

Plans:
- [x] 02-01-PLAN.md — gRPC CheckPermission server + GET /permissions endpoint + HybridCache wiring with Redis
- [x] 02-02-PLAN.md — Domain events on mutations + EventMapper + UserPermissionsInvalidated integration event + cache invalidation in command handlers
- [x] 02-03-PLAN.md — Chassis PermissionRequirement + PermissionRequirementHandler + AddPermissionAuthorization extension for downstream services

### Phase 3: Scheduling — Slots and Views
**Goal**: Managers can build and manage lesson schedules, teachers see their own sessions, and students see their group lessons — all enforcing permissions via Organizations. Groups and GroupMembership live in the Organizations service; Scheduling references GroupId as a plain Guid.
**Depends on**: Phase 2
**Requirements**: SCH-01, SCH-02, SCH-03, SCH-04, SCH-05, SCH-06, SCH-07, SCH-10
**Success Criteria** (what must be TRUE):
  1. Manager can create a lesson slot (group + date/time + teacher), and the system rejects creation if the teacher is already booked at that time
  2. Manager can edit a slot's time or teacher and can delete a slot; the change is immediately visible in the schedule view
  3. Manager sees a calendar view showing all groups' slots for their school; teacher sees only their own slots; student sees only slots for groups they belong to
  4. Manager can add a student to a group and remove a student from a group; group membership changes are reflected in the student's schedule view immediately
  5. All date/time values in the API and database are stored as DateTimeOffset; no DateTime.Now usage exists in the service
**Plans**: 7 plans

Plans:
- [x] 03-01-PLAN.md — Scheduling service bootstrap: project, constants (4 scheduling permissions), PermissionSeeder (HTTP), Aspire wiring, Keycloak client (Wave 1)
- [x] 03-02-PLAN.md — LessonSlot domain model + EF Core + repos + arch tests + unit test project (Wave 2)
- [x] 03-04-PLAN.md — LessonSlot create/edit/delete with global teacher conflict detection + Organizations gRPC group validation (Wave 3)
- [x] 03-05-PLAN.md — GET /schedule with permission-filtered manager/teacher/student views, student groups via Organizations gRPC (Wave 4)
- [x] 03-07-PLAN.md — Organization entity (Id only) + GroupsPermissions class in Organizations service (Wave 1)
- [x] 03-08-PLAN.md — Group aggregate + CRUD in Organizations service, protected by groups.* permissions (Wave 3)
- [x] 03-09-PLAN.md — GroupMembership in Organizations + GetGroupsForStudent gRPC + Scheduling gRPC client (Wave 4)

### Phase 4: Scheduling — Attendance and Outbox
**Goal**: Attendance is recorded for each student on each lesson slot and the AttendanceRecordedIntegrationEvent is reliably published to Kafka, providing Payments with a durable event stream to consume
**Depends on**: Phase 3
**Requirements**: SCH-08, SCH-09
**Success Criteria** (what must be TRUE):
  1. Manager can mark each student on a slot as Present, Absent, or Excused; attempting to mark the same student on the same slot twice updates the existing record rather than creating a duplicate
  2. After attendance is recorded, an AttendanceRecordedIntegrationEvent appears on the `attendance-recorded-integration-event` Kafka topic via the transactional outbox, even if the application restarts between the DB commit and the send
  3. The event payload includes StudentId, LessonSlotId, SchoolId, Timestamp (DateTimeOffset), and a CorrelationId suitable for use as an idempotency key in Payments
**Plans**: 3 plans

Plans:
- [x] 04-01-PLAN.md — AttendanceRecord aggregate, EF config with unique index, HasQueryFilter, permissions, Wave 0 test stubs (Wave 1)
- [ ] 04-02-PLAN.md — Domain event pipeline (EventMapper + outbox wiring), MarkAttendance PUT endpoint, handler tests (Wave 2)
- [x] 04-03-PLAN.md — GetSlotAttendance GET endpoint with AttendanceRecordDto (Wave 2)

### Phase 5: Payments — Ledger and Balance
**Goal**: Students have a lesson balance that managers can top up, decrements automatically when attendance is confirmed by Scheduling, and is visible to both student and manager with per-slot payment status
**Depends on**: Phase 4
**Requirements**: PAY-01, PAY-02, PAY-03, PAY-04, PAY-05, PAY-06
**Success Criteria** (what must be TRUE):
  1. Manager can manually add lessons (credits) to a student's balance within the school; the addition appears in the balance immediately
  2. When an AttendanceRecordedIntegrationEvent is received, exactly one lesson is debited from the student's balance; processing the same event twice does not debit twice
  3. Student and manager can view the student's balance showing total purchased, total used, and remaining lessons
  4. On a specific lesson slot, the payment status for each student is visible (paid / unpaid / in debt) derived from the ledger state
  5. Manager can manually apply a credit or debit adjustment to a student's balance; all transactions are preserved as immutable append-only ledger entries
**Plans**: TBD

Plans:
- [ ] 05-01: LessonPackage aggregate and LessonTransaction ledger (append-only); EF Core configuration and tenant isolation
- [ ] 05-02: Manual credit and adjustment commands; balance query (purchased / used / remaining)
- [ ] 05-03: AttendanceRecordedIntegrationEvent consumer with MassTransit inbox idempotency; per-slot payment status query

## Progress

**Execution Order:**
Phases execute in numeric order: 1 -> 2 -> 3 -> 4 -> 5

| Phase | Plans Complete | Status | Completed |
|-------|----------------|--------|-----------|
| 1. Organizations — RBAC Core | 4/4 | Complete   | 2026-03-18 |
| 2. Organizations — Permission Cache | 3/3 | Complete   | 2026-03-21 |
| 3. Scheduling — Slots and Views | 7/9 | In Progress|  |
| 4. Scheduling — Attendance and Outbox | 2/3 | In Progress|  |
| 5. Payments — Ledger and Balance | 0/3 | Not started | - |
