# Project Research Summary

**Project:** Edvantix — Organizations, Scheduling, and Payments microservices
**Domain:** Online school management SaaS (multi-tenant, custom RBAC, lesson scheduling, package-based payment tracking)
**Researched:** 2026-03-18
**Confidence:** HIGH

## Executive Summary

Edvantix is adding three domain microservices to an existing .NET 10 / Aspire / Kafka / PostgreSQL stack. All three services are greenfield implementations that must conform to patterns already established in the `Persona` service and `Edvantix.Chassis` building block. No new infrastructure is required: the stack is fixed (EF Core 10, MassTransit 8.5.8, Mediator.SourceGenerator 3.0.1, Grpc.AspNetCore 2.76.0, HybridCacheService over Redis). The critical architectural decision is the authorization model: custom per-tenant RBAC implemented entirely in the Organizations service, with downstream services checking permissions via a cached HTTP call to Organizations. Keycloak remains authentication-only.

The recommended approach follows a strict build order enforced by runtime dependencies. Organizations must ship first because every write command in Scheduling and Payments depends on it for permission resolution. Scheduling ships second and introduces the `AttendanceRecordedIntegrationEvent` that Payments consumes asynchronously. Payments ships third and closes the end-to-end flow: attendance marks in Scheduling → lesson deducted from student package in Payments. All three services share the same vertical-slice feature layout (Domain/AggregatesModel, Features, Infrastructure, IntegrationEvents) copied from Persona.

The dominant risk category is data isolation. Tenant isolation must be established as an architectural invariant before a single entity is written — not retrofitted. The `ITenantContext` abstraction, EF Core named global query filters, and an architecture test enforcing their presence on all `ITenanted` entities must be the first deliverable of Phase 1. Permission cache staleness is the second risk: revoked permissions must be evicted within seconds via Kafka events, not minutes via TTL expiry. Any shortcut here (long TTL, no eviction events) creates an unacceptable security window for a school management context.

---

## Key Findings

### Recommended Stack

All three new services extend the existing stack without adding new infrastructure dependencies. EF Core global query filters handle multi-tenancy at the application layer (PostgreSQL RLS was explicitly rejected due to incompatibility with transaction-mode connection pooling). The MassTransit transactional outbox ensures attendance events and role-change events survive infrastructure interruptions. `HybridCacheService` (wrapping Redis) provides the two-level permission cache. The Mediator pipeline behavior pattern (same as `ValidationBehavior`) is the injection point for permission checks — no changes to the Chassis pipeline are needed.

**Core technologies:**
- `EF Core 10` (via `Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL` 13.1.2): database ORM per service — each service owns an isolated PostgreSQL database on the shared cluster
- `MassTransit.EntityFrameworkCore` 8.5.8: transactional outbox in Organizations and Scheduling; transactional inbox in Payments — guarantees event delivery and prevents duplicate processing
- `Mediator.SourceGenerator` 3.0.1: in-process CQRS dispatch with source-generated handlers — already wired with validation and activity pipelines in Chassis; all commands and queries go through it
- `Grpc.AspNetCore` 2.76.0: Organizations serves a `CheckPermission` gRPC RPC; Scheduling and Payments are gRPC clients — same `AddGrpcServiceReference<T>` pattern already used by Persona
- `Microsoft.Extensions.Caching.Hybrid` 10.4.0: tag-based permission cache (`HybridCacheService`) — cache key `perm:{userId}:{schoolId}:{permission}`, invalidated via Kafka `RoleAssignmentChangedEvent`

See `.planning/research/STACK.md` for full version table, installation snippets, and alternatives considered.

### Expected Features

All three services have clearly bounded MVP scopes. The Organizations service is the foundation — without it, no other service can enforce access control. The Scheduling service is the primary workflow surface for managers and teachers. The Payments service is a ledger that reacts to events from Scheduling; it has no direct user-initiated writes beyond package creation and manual paid/unpaid flags.

**Must have (table stakes):**
- Organizations: predefined system roles (Owner, Manager, Teacher, Student) with seed permissions; create/assign/revoke custom roles; assign permissions to roles from a fixed catalogue; tenant-scoped permission enforcement at every write endpoint
- Scheduling: create/edit/cancel lesson slots (group + teacher + time + duration); teacher double-booking conflict detection; manager calendar view; teacher filtered view; student view via group membership; mark attendance (Present/Absent/Excused)
- Payments: create lesson package for a student (N lessons); automatic lesson deduction on `AttendanceMarkedEvent`; student balance display (purchased / used / remaining); manual paid/unpaid flag per lesson; low-balance student list for manager

**Should have (competitive, defer to v1.x):**
- Recurring lesson slots (weekly pattern with end date)
- Package expiry dates with near-expiry warnings
- Attendance statistics per student (absence rate)
- Multiple packages with sequential consumption
- Audit log of role and permission changes

**Defer (v2+):**
- Payment gateway integration (Stripe, YooKassa) — requires compliance, webhook handling, and refund logic
- Real-time schedule notifications (SignalR/SSE infrastructure not in this milestone)
- Per-resource permissions (manage only Group A) — requires resource ownership model across all services
- Drag-and-drop calendar rescheduling — pure UX enhancement
- Subscription / recurring billing model

See `.planning/research/FEATURES.md` for the full prioritization matrix and competitor feature analysis.

### Architecture Approach

Three independent microservices behind the existing YARP API Gateway, each with its own PostgreSQL database, communicating through a combination of synchronous REST (Scheduling → Organizations for permission checks) and asynchronous Kafka events (Scheduling → Payments for attendance). The permission check pattern uses a Mediator `MessagePreProcessor` that reflects on each command type for a `PermissionKey` string constant, fetches the user's full permission set from Organizations (cached in Redis HybridCache), and throws `ForbiddenException` if the permission is absent. Tenant isolation is enforced by an `ITenantContext` scoped service (JWT claim `tenant_id`) injected into each `DbContext` and applied as EF Core 10 named global query filters on every tenant-scoped entity.

**Major components:**
1. **Organizations Service** — owns roles, permissions, and user-role assignments per tenant; serves `GET /permissions/{tenantId}/{profileId}` for downstream caching; publishes `RoleAssignedToUserEvent` and `PermissionRevokedEvent` to Kafka for cache invalidation
2. **Scheduling Service** — owns groups, time slots, teacher assignments, and attendance records; enforces permissions via Organizations; publishes `AttendanceRecordedIntegrationEvent` to Kafka via transactional outbox
3. **Payments Service** — owns lesson packages and the append-only balance ledger (`LessonTransaction`); consumes `AttendanceRecordedIntegrationEvent` with idempotent inbox; never calls Scheduling directly

See `.planning/research/ARCHITECTURE.md` for data flow diagrams, RBAC middleware code, and anti-patterns.

### Critical Pitfalls

1. **Tenant isolation not enforced at DB layer** — the `DatabaseContext` base class applies no global query filter by default; any new service entity missing `HasQueryFilter` silently leaks cross-school data. Prevent by defining `ITenantContext` and a filter convention before writing the first entity, and enforcing with an architecture test that fails if any `ITenanted` entity has no query filter registered.

2. **Permission cache stale after role revocation** — a long TTL (5–10 min) without eviction events means a revoked manager can act for minutes after removal. Prevent by publishing `RoleAssignedToUserEvent` / `PermissionRevokedEvent` from Organizations and having downstream services call `IHybridCache.RemoveByTagAsync` on receipt. Use maximum 60-second TTL as a temporary fallback before events are wired.

3. **N+1 synchronous permission checks (no caching)** — calling Organizations HTTP API on every request makes Organizations a hard bottleneck. Prevent by implementing the two-level HybridCache from the start, not as an afterthought.

4. **Double attendance marking / double lesson deduction** — user retries after timeout cause two `AttendanceMarked` events; Payments processes both and deducts twice. Prevent with a unique DB constraint on `(LessonSlotId, StudentId)` in attendance and MassTransit inbox idempotency in Payments.

5. **Scheduling times stored in local timezone** — `DateTime` without timezone awareness causes display bugs across timezones and breaks overlap detection. Prevent by using `DateTimeOffset` on all scheduling entities from the first migration; never use `DateTime.Now`.

See `.planning/research/PITFALLS.md` for the full pitfall catalog including recovery strategies.

---

## Implications for Roadmap

Based on combined research, the phase structure is **strictly constrained by runtime dependencies**. Organizations is a synchronous dependency of Scheduling; Scheduling produces the event that Payments consumes. This is not a preference — it is a hard build order.

### Phase 1: Organizations Service and RBAC Foundation

**Rationale:** Every command in Scheduling and Payments requires a working permission check endpoint. Building Organizations first unblocks all subsequent services. This phase also establishes the `ITenantContext` abstraction, global query filter convention, and architecture test that all subsequent phases inherit. These cross-cutting concerns cannot be retrofitted.

**Delivers:** Custom RBAC store; `GET /permissions/{tenantId}/{profileId}` endpoint; predefined system roles seeded on school creation; `RoleAssignedToUserEvent` / `PermissionRevokedEvent` Kafka integration events; `ITenantContext` convention and architecture test in the test suite.

**Addresses (from FEATURES.md):** All Organizations table-stakes features; permission enforcement middleware (used by later phases).

**Avoids (from PITFALLS.md):** Tenant isolation not enforced (Pitfall 1); permission check not tenant-scoped (Pitfall 2); N+1 permission checks (Pitfall 3 — cache built here); stale cache after revocation (Pitfall 4 — eviction events published here); role explosion (Pitfall 9 — default role seeding + soft cap).

**Research flag:** Standard patterns — Persona service is the reference implementation; no additional research needed.

---

### Phase 2: Scheduling Service

**Rationale:** Scheduling can be built once Organizations exposes the permission query endpoint. This phase introduces the first cross-service Kafka event (`AttendanceRecordedIntegrationEvent`), which Payments will consume in Phase 3. The outbox must be designed here — Payments depends on the event contract.

**Delivers:** Groups, lesson slots, conflict detection, attendance marking (Present/Absent/Excused), manager/teacher/student calendar views, `AttendanceRecordedIntegrationEvent` published via transactional outbox.

**Uses (from STACK.md):** EF Core global query filters (tenant isolation); `Grpc.Tools` (gRPC client to Organizations); `MassTransit.EntityFrameworkCore` outbox; `Microsoft.Extensions.Caching.Hybrid` for permission cache invalidation.

**Implements (from ARCHITECTURE.md):** `PermissionBehavior` Mediator pre-processor wired to Organizations; `SlotAggregate` with overlap detection; `AttendanceRecord` with unique constraint; Kafka topic `edvantix.scheduling.attendance-recorded`.

**Avoids (from PITFALLS.md):** Timezone storage as DateTime (Pitfall 6 — use DateTimeOffset from first migration); double attendance marking (Pitfall 7 — unique constraint on attendance); missing outbox (Pitfall 8 — outbox designed here for Payments to rely on).

**Research flag:** `DateTimeOffset` and IANA timezone handling are standard .NET patterns — no additional research needed. Conflict detection algorithm (teacher overlap) is a straightforward DB query — no research needed.

---

### Phase 3: Payments Service

**Rationale:** Payments is the terminal consumer of the Scheduling event and the simplest service in terms of API surface. It can be built and tested independently by publishing mock `AttendanceRecordedIntegrationEvent` messages. The append-only ledger pattern eliminates mutable balance concurrency concerns.

**Delivers:** Lesson package management; append-only `LessonTransaction` ledger; automatic lesson deduction via idempotent `AttendanceRecordedIntegrationEvent` consumer; student balance display; manual paid/unpaid flag; low-balance student list for manager.

**Uses (from STACK.md):** `MassTransit.EntityFrameworkCore` inbox (idempotent consumer); EF Core global query filter; `Microsoft.Extensions.Caching.Hybrid` for permission cache.

**Implements (from ARCHITECTURE.md):** `LessonPackageAggregate` with `LessonLedgerEntry` (append-only); `AttendanceRecordedIntegrationEventHandler` with inbox deduplication; balance computed as `SUM(credits) - SUM(debits)` — no mutable counter.

**Avoids (from PITFALLS.md):** Double lesson deduction (Pitfall 7 — inbox idempotency); event ordering issue where attendance arrives before package exists (Pitfall 10 — retry queue with exponential backoff for unresolvable events); balance inconsistency from lost events (Pitfall 8 — inbox pattern).

**Research flag:** Standard patterns — MassTransit inbox is documented and already present in the stack. No additional research needed.

---

### Phase Ordering Rationale

- **Organizations before Scheduling:** Scheduling's `PermissionBehavior` makes a synchronous HTTP call to Organizations on every write command. Organizations must be deployable (even with stub data) before Scheduling can be developed with real permission checks wired in.
- **Scheduling before Payments:** The `AttendanceRecordedIntegrationEvent` contract is defined in Scheduling. Payments must consume this event without modification. The event schema (including `CorrelationId` for partition-keyed ordering) must be finalized in Phase 2 before Phase 3 begins.
- **Tenant isolation and architecture test in Phase 1:** These cross-cutting concerns cannot be added after the fact. Establishing the filter convention and architecture test in Phase 1 means Phases 2 and 3 inherit correct isolation automatically — no per-phase audit required.
- **Outbox in Phase 2, inbox in Phase 3:** The transactional outbox in Scheduling and inbox in Payments must be designed as a pair. Designing them in sequential phases ensures they are compatible before Payments goes to production.

### Research Flags

Phases likely needing deeper research during planning:
- **None identified** — all three services follow established patterns from the existing Persona service and Chassis building blocks. Research confidence is HIGH across all areas.

Phases with standard patterns (skip research-phase):
- **Phase 1 (Organizations):** Custom RBAC with EF Core is well-documented; Persona serves as the reference for project structure and outbox wiring.
- **Phase 2 (Scheduling):** Conflict detection and `DateTimeOffset` storage are standard .NET/EF Core patterns.
- **Phase 3 (Payments):** Append-only ledger and MassTransit inbox are documented patterns with existing project examples.

---

## Confidence Assessment

| Area | Confidence | Notes |
|------|------------|-------|
| Stack | HIGH | All versions verified against `Directory.Packages.props` and `Versions.props`; Persona service is a working reference implementation |
| Features | HIGH (RBAC, Scheduling), MEDIUM (Payments) | RBAC and scheduling patterns verified against competitor analysis and SaaS RBAC literature; payment tracking patterns are simpler but rely on the event-driven design holding correctly |
| Architecture | HIGH | Patterns derived directly from the existing Chassis and Persona codebase; data flow diagrams match the existing Aspire AppHost wiring |
| Pitfalls | HIGH (tenant isolation, permission cache, balance consistency), MEDIUM (scheduling edge cases) | Tenant isolation and RBAC pitfalls are well-documented in official sources; scheduling edge cases (timezone, overlap) are standard but require careful first-migration discipline |

**Overall confidence:** HIGH

### Gaps to Address

- **Group membership ownership decision:** FEATURES.md flags that groups could live in Organizations (org structure) or Scheduling (scheduling construct). Research recommends Scheduling as the natural owner since groups are scheduling constructs, but this requires explicit confirmation before schema design. Misplacing groups in Phase 1 would require a migration to move them in Phase 2.
- **Permission string registry:** ARCHITECTURE.md notes that permission names are defined as `const` in each command class with no type-system enforcement of valid strings. The PITFALLS.md security section recommends a seed registry and validation at role creation time. The mechanism for validating permission strings during `AssignPermissionsToRole` needs to be designed in Phase 1 before Scheduling and Payments define their permission catalogues.
- **Multi-school user token model:** ARCHITECTURE.md notes that in v1 a user belongs to exactly one tenant. If this changes in v1.x (user switches between schools), the `tenant_id` JWT claim approach requires re-authentication or token refresh. This needs to be flagged in Phase 1 schema design so the data model is forward-compatible.

---

## Sources

### Primary (HIGH confidence)
- `E:/projects/Edvantix/Directory.Packages.props` — all package version pins
- `E:/projects/Edvantix/Versions.props` — Aspire 13.1.2, MassTransit 8.5.8, Mediator 3.0.1, Grpc 2.76.0
- `E:/projects/Edvantix/src/Services/Persona/` — reference service structure (gRPC, outbox, HybridCache)
- `E:/projects/Edvantix/src/BuildingBlocks/Edvantix.Chassis/` — EventBus, Caching, CQRS pipelines
- `E:/projects/Edvantix/src/Aspire/Edvantix.ServiceDefaults/` — `AddGrpcServiceReference<T>` pattern
- [EF Core Global Query Filters — Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/querying/filters)
- [ASP.NET Core HybridCache — Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-10.0)

### Secondary (MEDIUM confidence)
- [MassTransit EF Core Outbox docs](https://masstransit.io/documentation/patterns/saga/entity-framework) — outbox + inbox configuration
- [Multi-tenant Applications with EF Core — Milan Jovanovic](https://www.milanjovanovic.tech/blog/multi-tenant-applications-with-ef-core)
- [How to Design Multi-Tenant RBAC for SaaS — WorkOS](https://workos.com/blog/how-to-design-multi-tenant-rbac-saas)
- [Idempotent Consumer Pattern — microservices.io](https://microservices.io/patterns/communication-style/idempotent-consumer.html)
- [Time Zone Uncertainty — Martin Fowler](https://martinfowler.com/bliki/TimeZoneUncertainty.html)
- [TutorCruncher](https://tutorcruncher.com), [Teachworks](https://teachworks.com), [DreamClass](https://www.dreamclass.io) — competitor feature analysis

### Tertiary (LOW confidence)
- [ASP.NET Core custom authorization — Microsoft Learn (ASP.NET Core 8)](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-8.0) — pattern applies to .NET 10 but docs not yet updated

---

*Research completed: 2026-03-18*
*Ready for roadmap: yes*
