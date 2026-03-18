# Pitfalls Research

**Domain:** Online school management — custom RBAC, scheduling, and lesson-package payment tracking in .NET 10 microservices
**Researched:** 2026-03-18
**Confidence:** HIGH (RBAC/multitenancy), MEDIUM (scheduling edge cases), HIGH (payment balance consistency)

---

## Critical Pitfalls

### Pitfall 1: Tenant Isolation Not Enforced at the Database Layer

**What goes wrong:**
Every query across Organizations, Scheduling, and Payments services silently returns cross-school data. A student from School A can read or mutate schedules, balances, and permissions belonging to School B. This is the single most catastrophic data breach in a SaaS product.

**Why it happens:**
The existing `DatabaseContext` base class in `Edvantix.Chassis` applies no EF Core global query filter for tenant scope. New services that inherit it receive zero isolation by default. Developers typically rely on manually including `WHERE school_id = @id` in application queries, which is error-prone — any query that forgets the clause leaks data. EF's `IgnoreQueryFilters()` and raw SQL calls bypass even filters that do exist.

**How to avoid:**
- Each new service's `DbContext` must override `OnModelCreating` and apply `HasQueryFilter(e => e.SchoolId == _currentTenant.SchoolId)` on every `ITenanted` entity.
- Introduce a `ITenantContext` abstraction in Chassis (scoped, populated from JWT claim) and inject it into each `DbContext` constructor.
- Add an architecture test (ArchUnit or NetArchTest) that fails if any `IEntity` implementing `ITenanted` is missing a query filter in its `EntityTypeConfiguration`.
- Never call `IgnoreQueryFilters()` outside of explicitly marked system/admin operations.

**Warning signs:**
- New service's `DbContext` extends `DatabaseContext` with no `HasQueryFilter` overrides.
- Integration tests pass when run with a single school but never test two schools accessing each other's data.
- No `ITenantContext` or equivalent appears in the service's DI registration.

**Phase to address:** Organizations service — Phase 1 (foundation). All subsequent services inherit the pattern. Define `ITenantContext` and the filter convention before writing any entity.

---

### Pitfall 2: Permission Checks That Are Not Tenant-Scoped

**What goes wrong:**
A user assigned the `Manager` role in School A can successfully invoke scheduling or payment endpoints for School B because the RBAC check asks "does user have permission `scheduling.create-slot`?" without also verifying "in the context of School B?" The system is logically correct but organizationally insecure.

**Why it happens:**
Role and permission lookup is written as: "fetch user's roles → check if any role has this permission." The tenant dimension is never added. When roles are modeled as global entities (no `SchoolId` column), this is structurally impossible to fix without a schema change.

**How to avoid:**
- The `Role` and `UserRoleAssignment` entities in Organizations service must carry `SchoolId` as a non-nullable column with a database constraint.
- Permission evaluation signature must be: `HasPermission(userId, schoolId, permissionName)` — never a two-argument form.
- The authorization middleware in each downstream service extracts both `userId` **and** `schoolId` from the JWT, then passes both when calling the Organizations permission API (or reading from cache).
- Write a test: create User A with Manager role in School 1, call a School 2 endpoint as User A — expect 403.

**Warning signs:**
- Permission check method signature has no tenant/school parameter.
- `SchoolId` absent from `Role` or `UserRoleAssignment` table definitions.
- JWT token claims have `sub` but no `school_id` or equivalent routing claim.

**Phase to address:** Organizations service — Phase 1. This is a schema-level decision; retrofitting `SchoolId` into RBAC tables after data exists is expensive.

---

### Pitfall 3: N+1 Permission Checks — Per-Request HTTP Calls to Organizations Service

**What goes wrong:**
Each HTTP request to Scheduling or Payments triggers a synchronous call to the Organizations service to resolve the user's permissions. Under modest load (20 concurrent users, each with 5 permissions to check), the Organizations service becomes a synchronous bottleneck. Latency multiplies and the entire platform degrades when Organizations is slow or restarted.

**Why it happens:**
The straightforward implementation calls `HttpClient.GetAsync("/permissions/{userId}/{schoolId}")` inside the authorization middleware for every incoming request. No caching layer is added because "we'll add it later."

**How to avoid:**
- Implement a two-level cache: in-memory `IMemoryCache` with a short TTL (30–60 seconds), backed by a distributed cache (Redis, already available via Aspire) with a longer TTL (5–10 minutes).
- Cache key must include both `userId` and `schoolId`: `permissions:{schoolId}:{userId}`.
- Publish a domain event from Organizations whenever a role assignment or permission changes; downstream service caches subscribe and evict the specific key immediately. This bounds the stale window to the messaging lag rather than the full TTL.
- As a fallback, accept that the cache may be stale for up to the TTL — document this as the accepted security tradeoff.

**Warning signs:**
- Organizations service CPU spikes proportionally to request load on Scheduling/Payments.
- No `IMemoryCache` or `IDistributedCache` usage in authorization middleware code.
- Load tests show p99 latency in Scheduling proportional to Organizations response time.

**Phase to address:** Organizations service — Phase 1 for basic caching; Phase 2 (Scheduling) for invalidation events.

---

### Pitfall 4: Stale Permission Cache After Role Revocation

**What goes wrong:**
A school owner removes a teacher's `Manager` role. For the next 5–10 minutes (or until cache TTL expires), that teacher can still create schedule slots, modify attendance, and access payment data because every service is reading from cache. The owner has no recourse until the TTL expires.

**Why it happens:**
Cache invalidation is treated as a performance optimization and added "later" — but the first version only sets entries with TTL and never proactively evicts them on permission change.

**How to avoid:**
- On every `RoleAssigned`, `RoleRevoked`, `PermissionGranted`, `PermissionRevoked` domain event in Organizations, publish an integration event to the message bus (e.g., MassTransit/RabbitMQ, present in the platform).
- Each downstream service subscribes to these events and immediately calls `cache.Remove(key)` for the affected `(schoolId, userId)` pair.
- For the period before event consumers are built, use an aggressive TTL (30 seconds max) rather than a long one. Do not defer this decision — a 10-minute stale window for a security revocation is unacceptable in a school context.

**Warning signs:**
- No integration event published on role/permission mutation.
- Cache TTL set to minutes without an invalidation event mechanism.
- Tests do not cover "revoke role, then assert permission denied within one second."

**Phase to address:** Organizations service — Phase 1 (events must be designed from day 1). Integration event consumption in Scheduling/Payments — Phase 2 and 3.

---

### Pitfall 5: EF Core Global Query Filter Bypassed by Raw SQL and `IgnoreQueryFilters`

**What goes wrong:**
The team correctly adds `HasQueryFilter` for tenant isolation, then writes one raw SQL query (e.g., a reporting query, or a migration seed) using `FromSqlRaw()`. That query silently bypasses the filter. A second developer later adds `IgnoreQueryFilters()` "just for this admin endpoint" — now two bypass paths exist, and only one is documented.

**Why it happens:**
EF Core's `FromSqlRaw`, `ExecuteSqlRaw`, raw ADO.NET calls, and `IgnoreQueryFilters()` all bypass query filters by design. Developers are often unaware of this when they only tested the standard LINQ path.

**How to avoid:**
- Ban `FromSqlRaw` and `ExecuteSqlRaw` in service code via a Roslyn analyzer rule or architecture test; use `FromSql` with interpolated parameters instead (EF8+).
- Require any use of `IgnoreQueryFilters()` to be explicitly tagged with a code comment referencing an approved use case (admin dashboard queries only), and add a PR checklist item.
- Add integration tests that create two schools, insert data for both, and assert that every repository method returns only the current tenant's data.

**Warning signs:**
- `FromSqlRaw` or `ExecuteSqlRaw` appears anywhere in a service repository.
- `IgnoreQueryFilters` appears without a comment explaining the tenant bypass rationale.
- Architecture tests do not exist or do not cover EF query patterns.

**Phase to address:** All service phases — establish the rule in Phase 1 and enforce it via architecture tests before Phase 2 begins.

---

### Pitfall 6: Scheduling — Times Stored in Local Timezone Instead of UTC

**What goes wrong:**
A manager in Moscow creates a lesson slot for "10:00 AM." The server is running in UTC+0 (Azure/container default). The stored value is `2026-04-01T07:00:00Z`. A teacher in Yekaterinburg (UTC+5) sees the slot displayed as "12:00 PM" — wrong. Worse, overlap detection logic compares raw `DateTime` values without normalization, allowing two slots that actually overlap to appear non-conflicting.

**Why it happens:**
`DateTime.Now` (local time) is used instead of `DateTime.UtcNow`. Entities store `DateTime` without `DateTimeKind`, and EF Core maps this as unspecified. No timezone information is stored alongside the time value.

**How to avoid:**
- All `DateTime` columns must be `DateTimeOffset` (not `DateTime`) — EF Core maps this to `timestamp with time zone` in PostgreSQL, which stores UTC explicitly.
- Store the school's IANA timezone name (e.g., `"Europe/Moscow"`) on the `School` entity. This is the "source of truth" for display conversion.
- Never convert to local time in the backend. Provide raw `DateTimeOffset` in UTC to the frontend; the frontend handles display using the school timezone.
- In C#, use `TimeZoneInfo.FindSystemTimeZoneById` with IANA names via `TimeZoneInfo.TryConvertIanaIdToWindowsId` or the `TimeZoneConverter` NuGet package for cross-platform correctness.
- Overlap detection must compare `DateTimeOffset` values, never raw `DateTime`.

**Warning signs:**
- Entity properties typed as `DateTime` instead of `DateTimeOffset`.
- `DateTime.Now` usage anywhere in scheduling domain code.
- No `TimeZoneId` or `TimeZone` property on `School` or `LessonSlot` entity.
- Tests only run with system timezone = UTC, masking bugs.

**Phase to address:** Scheduling service — Phase 2. Decision must be made before the first migration is created; changing column types after data exists requires a migration with data conversion.

---

### Pitfall 7: Double Attendance Marking — Lesson Balance Deducted Twice

**What goes wrong:**
A teacher marks a student present, the request times out before receiving a response, and the teacher clicks "Mark Attendance" again. Two `AttendanceMarked` events are published. The Payments service processes both, deducting two lessons from the student's package. The student's balance is now incorrect and the error is hard to detect without an audit trail.

**Why it happens:**
The attendance command handler is not idempotent. There is no deduplication check. The event consumer in Payments processes every received event regardless of whether it has seen it before.

**How to avoid:**
- The `MarkAttendance` command must check whether attendance for `(lessonSlotId, studentId)` already exists before inserting. Use a unique database constraint on `(LessonSlotId, StudentId)` in the attendance table — let the database enforce the invariant.
- The Payments service event consumer must record processed event IDs (outbox or inbox pattern) and skip duplicates. Use MassTransit's built-in idempotency support or a processed-events table.
- Expose a `GET /attendance/{slotId}/{studentId}` endpoint so the frontend can check current state before a retry rather than blindly resubmitting.

**Warning signs:**
- No unique constraint on the attendance table for `(LessonSlotId, StudentId)`.
- Payments event consumer has no deduplication logic.
- Manual UI re-test shows balance decrements by 2 on double-click.

**Phase to address:** Scheduling service Phase 2 (attendance entity), Payments service Phase 3 (event consumer idempotency).

---

### Pitfall 8: Lesson Balance Inconsistency — Payments Service Misses Events

**What goes wrong:**
The Payments service computes a student's remaining balance by counting consumed lessons from `AttendanceMarked` events. If the message bus goes down, or the Payments consumer crashes mid-processing, the event is lost (or processed but the DB write fails). The balance is now permanently incorrect relative to actual attendance.

**Why it happens:**
Relying on message delivery guarantees without an outbox/inbox pattern means any infrastructure interruption creates permanent data divergence between what Scheduling knows and what Payments knows.

**How to avoid:**
- Use the transactional outbox pattern in Scheduling: write the attendance record and the outbox event in the same database transaction. MassTransit supports this with EF Core outbox integration.
- In Payments, use the inbox pattern: record the event ID before processing. If the consumer crashes after DB write but before acknowledging, the re-delivered message is identified as a duplicate and skipped.
- Provide an admin reconciliation endpoint that can recompute a student's consumed lesson count from the canonical Scheduling data on demand.

**Warning signs:**
- Attendance write and event publish are in separate transactions (publish happens after `SaveChangesAsync`).
- No outbox table in Scheduling service schema.
- Balance mismatch discovered during manual testing by simply killing the Payments service mid-operation.

**Phase to address:** Payments service — Phase 3. Design the outbox in Scheduling (Phase 2) before Payments consumes its events.

---

### Pitfall 9: Custom RBAC — Role Explosion from Unconstrained Role Creation

**What goes wrong:**
School owners can create any number of roles with any name. After 6 months, School A has 47 roles including `"Math Teacher (can reschedule but not delete)"`, `"Math Teacher (cannot reschedule)"`, `"Substitute Math Teacher"`, etc. Permission checks become a full table scan across hundreds of role-permission assignments per user. The system remains functionally correct but operationally unmaintainable.

**Why it happens:**
No guardrails on role count or permission assignment count per school. The RBAC model does not provide a set of suggested default roles — owners start from scratch every time and create fine-grained roles for every edge case.

**How to avoid:**
- Seed each school with 3–4 default roles (`Owner`, `Manager`, `Teacher`, `Student`) that cover 95% of use cases.
- Apply a soft limit on custom role count per school (e.g., 20 roles max) — enforced as a domain rule in the `AddRole` command, not a database constraint.
- The permission model should remain flat (permission = CQRS command identifier) but roles should be additive, not substitutive — most schools will use defaults without modification.
- For performance: pre-compute and cache the effective permission set per `(userId, schoolId)` rather than joining role-permission tables on every request.

**Warning signs:**
- No default role seeding on school creation.
- `GetUserPermissions` query does multiple `JOIN`s with no caching layer.
- Schools with more than 15 custom roles appear in the first month of operation.

**Phase to address:** Organizations service — Phase 1 (default role seeding must be part of school creation).

---

### Pitfall 10: Event Ordering — Payments Processes `AttendanceMarked` Before `PackagePurchased`

**What goes wrong:**
A student's package is created and they attend a lesson within the same minute. Due to message bus ordering, `AttendanceMarked` is consumed before `PackagePurchased` arrives. The Payments service tries to deduct from a non-existent package and fails — either silently dropping the event or logging an error with no retry. The student's balance is never initialized correctly.

**Why it happens:**
Message buses do not guarantee ordering across different event types published by different services (or even the same service under concurrent load). The Payments service assumes packages always exist when attendance arrives.

**How to avoid:**
- The Payments service should not fail silently when a package does not exist. Instead, it should either:
  - Use a saga/process manager to correlate `PackagePurchased` + `AttendanceMarked` before computing balance (complex but correct).
  - Or more pragmatically: treat the Payments balance as a projection that can be rebuilt from Scheduling's attendance log. The `PackagePurchased` event sets the initial balance; subsequent attendance events decrement it. If an `AttendanceMarked` arrives with no matching package, park it in a retry queue with exponential backoff.
- Use a `CorrelationId` on all events for the same student+school to route to the same consumer partition (if using Kafka or partitioned consumers), preserving order within a student's event stream.

**Warning signs:**
- Payments consumer throws `NullReferenceException` or `KeyNotFoundException` on missing package during integration tests.
- No dead-letter or retry mechanism for unresolvable events.
- Tests do not simulate out-of-order event delivery.

**Phase to address:** Payments service — Phase 3. Design the event schema (including correlation IDs) in Scheduling Phase 2 before Payments consumes them.

---

## Technical Debt Patterns

| Shortcut | Immediate Benefit | Long-term Cost | When Acceptable |
|----------|-------------------|----------------|-----------------|
| Skip `ITenantContext` in first service, add `WHERE school_id = @id` manually everywhere | Faster first feature | Every new query is a potential isolation bug; no architecture test can catch it | Never — set the abstraction before writing the first repository |
| Use `DateTime` instead of `DateTimeOffset` for lesson slots | Simpler entity model | Cross-timezone bugs; painful migration with data conversion required | Never for scheduling data |
| No inbox/outbox — publish events after `SaveChanges` | Fewer moving parts in v1 | Permanent balance inconsistencies under any infrastructure interruption | Only if message delivery is never required (this project requires it) |
| Cache permissions with 10-minute TTL, no eviction events | Simpler Organizations service | Revoked permissions remain valid for 10 minutes — unacceptable security gap | Maximum 60-second TTL is acceptable as a temporary measure before events are wired |
| Hardcode system roles, no custom RBAC in v1 | Eliminates entire Organizations service complexity | Owners cannot manage their school — core product requirement violated | Never — this is a stated v1 requirement |

---

## Integration Gotchas

| Integration | Common Mistake | Correct Approach |
|-------------|----------------|------------------|
| Keycloak JWT → Organizations RBAC | Treating Keycloak roles as the source of truth for in-app permissions | Keycloak provides AuthN only (`sub`, `email`). All role/permission data lives in Organizations service. Never read Keycloak roles in Scheduling or Payments. |
| Organizations → Scheduling (permission check) | Calling Organizations HTTP API on every request with no circuit breaker | Use `IHttpClientFactory` with Polly circuit breaker + retry; add memory cache with eviction events. |
| Scheduling → Payments (attendance event) | Publishing integration event outside the database transaction | Use MassTransit EF Core outbox — event is written to DB in same transaction as attendance record. |
| Frontend timezone → Scheduling API | Sending local `DateTime` strings to the API | Frontend must send `DateTimeOffset` (ISO 8601 with offset); API must store as UTC `DateTimeOffset`. |
| EF Core `Include()` with global query filters | `Include()` on a tenant-filtered entity can silently drop parent records due to INNER JOIN with filter | Configure navigations as optional (LEFT JOIN) or explicitly test Include paths across tenant boundaries. |

---

## Performance Traps

| Trap | Symptoms | Prevention | When It Breaks |
|------|----------|------------|----------------|
| Per-request synchronous permission resolution (Organizations HTTP call) | Organizations service CPU scales with total request volume; Scheduling/Payments p99 latency spikes on Organizations restarts | In-memory + distributed cache keyed on `(schoolId, userId)` with eviction events | At ~50 concurrent users with no caching |
| Loading all lesson slots for a school without pagination | Scheduling list endpoints time out for schools with 12+ months of history | Always paginate slot queries; add composite index on `(SchoolId, StartTime)` | At ~500 slots per school (typical after 6 months) |
| `GetUserPermissions` joining roles + permissions on every request | Slow permission resolution; multiple DB round-trips per request | Pre-join and cache the effective permission set as a `HashSet<string>` per `(schoolId, userId)` | At ~10 permissions per role, ~3 roles per user, ~100 concurrent requests |
| Balance recomputed from all attendance records every time | Payments balance query full-scans the attendance table | Maintain a running balance counter on `StudentPackage`; update atomically on each attendance event | At ~100 lessons per student |

---

## Security Mistakes

| Mistake | Risk | Prevention |
|---------|------|------------|
| RBAC check does not include `schoolId` — only checks `userId + permissionName` | Privilege escalation across schools: School A manager manages School B data | Always pass `schoolId` extracted from route/JWT to every permission check; write a cross-school integration test |
| `IgnoreQueryFilters()` on a tenant-scoped entity in a non-admin endpoint | Cross-tenant data read | Architecture test: assert no `IgnoreQueryFilters()` call exists outside `AdminDbContext` or explicitly tagged admin query handlers |
| Permission names stored as free-text strings with no registry | Typos create phantom permissions that are never matched, or collisions between services | Define permission names as constants in a shared `Permissions` static class; validate on role creation that the permission name exists in the registry |
| Student can call `MarkAttendance` endpoint directly | Student marks themselves present, consuming no lesson balance | `MarkAttendance` requires `scheduling.mark-attendance` permission which is only assignable to Teacher/Manager roles |
| No audit log for balance mutations | Impossible to investigate disputed balances | Every `PackageDeducted` and `PackageCredited` event must be persisted in an immutable ledger table in Payments, not just reflected in the balance counter |

---

## UX Pitfalls

| Pitfall | User Impact | Better Approach |
|---------|-------------|-----------------|
| Slot times displayed in UTC on the frontend | Teachers see "10:00 UTC" instead of their local time, causing missed lessons | Store school IANA timezone in Organizations; pass it in API responses; frontend converts using `Intl.DateTimeFormat` |
| Balance shows "consumed / purchased" as raw counts with no context | Manager cannot tell if a student is running low on lessons | Show "X lessons remaining (out of Y)" with a visual indicator when remaining < 3 |
| Attendance marking UI does not reflect optimistic state | Teacher marks student present, sees no immediate feedback, clicks again — double deduction | Disable the mark button immediately on click; show loading state; restore on error |
| Permission denied errors show generic 403 with no explanation | Manager does not know which permission they are missing | Return `{ "required_permission": "scheduling.create-slot" }` in the 403 response body |

---

## "Looks Done But Isn't" Checklist

- [ ] **Tenant isolation:** Query returns correct data for one school — verify it also correctly excludes data from a second school in the same database.
- [ ] **Permission revocation:** Role removed from user — verify the user is denied within the cache TTL period (requires eviction event wiring, not just TTL expiry).
- [ ] **Attendance idempotency:** Attendance marked once — verify marking again returns the existing record, not a second deduction.
- [ ] **Balance initialization:** Student package created — verify balance displays correctly before any attendance is marked.
- [ ] **Timezone display:** Slot created by user in UTC+3 — verify a user in UTC+0 sees the correct adjusted local time.
- [ ] **Overlap prevention:** Two slots created for the same teacher at the same time — verify the second is rejected, not silently accepted.
- [ ] **Outbox delivery:** Payments service killed mid-processing — verify balance is correct after service restart (no permanent divergence).
- [ ] **Cross-service auth:** User with permissions in School A calls School B endpoints — verify 403, not 200.

---

## Recovery Strategies

| Pitfall | Recovery Cost | Recovery Steps |
|---------|---------------|----------------|
| Tenant isolation not enforced (data leak discovered in production) | HIGH | Immediate: disable affected endpoints; audit all queries for missing tenant filter; apply filters and deploy; notify affected schools per GDPR/data protection law |
| Wrong timezone stored for existing slots | MEDIUM | Write a migration that reads each slot's school timezone, converts stored value to UTC `DateTimeOffset`, and re-saves; coordinate with school admins on displayed time changes |
| Balance inconsistency from lost attendance events | MEDIUM | Build a reconciliation job: for each student, recount attendance from Scheduling, compare to Payments balance, emit corrective `PackageCredited`/`PackageDebited` events |
| Permission cache stale after revocation | LOW | Restart the affected service (flushes in-memory cache) as an emergency measure; long-term fix requires eviction events |
| Double attendance deductions discovered | MEDIUM | Identify affected `(lessonSlotId, studentId)` pairs with count > 1 in attendance table; issue compensating `PackageCredited` events for each duplicate; add unique constraint |

---

## Pitfall-to-Phase Mapping

| Pitfall | Prevention Phase | Verification |
|---------|------------------|--------------|
| Tenant isolation not enforced at DB layer | Phase 1 — Organizations (establish `ITenantContext` + filter convention) | Architecture test fails if `ITenanted` entity has no query filter |
| Permission check not tenant-scoped | Phase 1 — Organizations (schema includes `SchoolId` on Role) | Cross-school integration test: School A manager cannot access School B endpoints |
| N+1 permission checks | Phase 1 — Organizations (build cache layer before other services consume it) | Load test: 50 concurrent requests produce 0 calls to Organizations service (all cached) |
| Stale permission cache after revocation | Phase 1 — Organizations (publish domain events on role change) | Test: revoke role, assert 403 within 5 seconds on downstream service |
| EF query filter bypassed by raw SQL | Phase 1–3 — All services (architecture test from day 1) | Architecture test present in test suite; passes on every CI run |
| Scheduling timezone storage | Phase 2 — Scheduling (DateTimeOffset before first migration) | Test with two users in different UTC offsets; assert displayed times are correct for each |
| Double attendance / lesson deduction | Phase 2 — Scheduling (unique constraint); Phase 3 — Payments (idempotent consumer) | Double-submit integration test returns existing record, not two records |
| Payments misses events (no outbox) | Phase 2 — Scheduling (outbox design); Phase 3 — Payments (inbox pattern) | Kill Payments mid-test; restart; assert balance is correct |
| Event ordering (attendance before package) | Phase 3 — Payments (retry queue for unresolvable events) | Integration test: publish `AttendanceMarked` before `PackagePurchased`; assert balance eventually correct |
| Role explosion | Phase 1 — Organizations (default role seeding + soft cap) | Test: school created → verify 4 default roles exist; verify `AddRole` command rejects the 21st role |

---

## Sources

- [Multi-tenant Applications with EF Core — Milan Jovanovic](https://www.milanjovanovic.tech/blog/multi-tenant-applications-with-ef-core)
- [EF Core Global Query Filters — Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/querying/filters)
- [Multi-tenancy — EF Core Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/miscellaneous/multitenancy)
- [Authorization in a microservices world — Alexander Lolis](https://www.alexanderlolis.com/authorization-in-a-microservices-world)
- [Best Practices for Microservice Authorization — Oso](https://www.osohq.com/post/microservices-authorization-patterns)
- [How to Design Multi-Tenant RBAC for SaaS — WorkOS](https://workos.com/blog/how-to-design-multi-tenant-rbac-saas)
- [Four RBAC Limitations and How to Fix Them — Axiomatics](https://axiomatics.com/news/press-releases/four-role-based-access-control-rbac-limitations-and-how-to-fix-them)
- [Idempotent Consumer Pattern — microservices.io](https://microservices.io/patterns/communication-style/idempotent-consumer.html)
- [Idempotency and ordering in event-driven systems — CockroachLabs](https://www.cockroachlabs.com/blog/idempotency-and-ordering-in-event-driven-systems/)
- [Time Zone Uncertainty — Martin Fowler](https://martinfowler.com/bliki/TimeZoneUncertainty.html)
- [How to Solve Race Conditions in a Booking System — HackerNoon](https://hackernoon.com/how-to-solve-race-conditions-in-a-booking-system)
- [Mastering Multi-Tenancy in .NET EF Core — Jordan Rowles, Medium, Jan 2026](https://jordansrowles.medium.com/mastering-multi-tenancy-in-net-ef-core-part-1-foundations-5ecaab4ffc28)
- [How a Cache Problem Sparked Event-Driven Innovation — Qantas Engineering Blog](https://medium.com/qantas-engineering-blog/how-a-simple-cache-problem-snowballed-into-event-driven-innovation-23b6d27c1abd)
- [Race Conditions in APIs — APIsec](https://www.apisec.ai/blog/race-condition-vulnerabilities-in-apis)

---

*Pitfalls research for: Edvantix online school management — Organizations (RBAC), Scheduling, Payments microservices*
*Researched: 2026-03-18*
