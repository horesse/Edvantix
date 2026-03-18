# Architecture Research

**Domain:** Online school management — Organizations/RBAC, Scheduling, Payments microservices
**Researched:** 2026-03-18
**Confidence:** HIGH

## Standard Architecture

### System Overview

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          Next.js 16 Frontend                            │
│  (manager / teacher / student views — school-scoped JWT in every call)  │
└────────────────────────────────┬────────────────────────────────────────┘
                                 │ HTTPS — includes tenantId claim
                                 ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                         API Gateway (YARP)                              │
│              Routes by path prefix, forwards JWT unchanged              │
└──────┬──────────────────────┬──────────────────────┬───────────────────┘
       │                      │                      │
       ▼                      ▼                      ▼
┌─────────────┐    ┌─────────────────┐    ┌──────────────────┐
│ Organizations│    │   Scheduling    │    │    Payments      │
│   Service   │    │    Service      │    │    Service       │
│             │    │                 │    │                  │
│ RBAC store  │    │ Slots + Groups  │    │ Lesson packages  │
│ Roles/Perms │    │ Attendance log  │    │ Balance ledger   │
│             │◄───│ Calls Orgs for  │    │ Consumes Kafka   │
│             │    │ permission check│    │ attendance events│
└──────┬──────┘    └────────┬────────┘    └──────────────────┘
       │                   │
       │ Kafka             │ Kafka
       │ TenantRoleAssigned│ AttendanceRecorded
       ▼                   ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                     Kafka (MassTransit CloudEvents)                     │
└─────────────────────────────────────────────────────────────────────────┘
       │
       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                     Shared PostgreSQL cluster                           │
│  organizationsdb │ schedulingdb │ paymentsdb  (three separate databases)│
└─────────────────────────────────────────────────────────────────────────┘
```

### Component Responsibilities

| Component | Responsibility | Implementation |
|-----------|----------------|----------------|
| Organizations Service | Owns roles, permissions, user-role assignments per tenant. Answers "does user X have permission Y in school Z?" | REST + gRPC, own PostgreSQL DB |
| Scheduling Service | Owns groups, time slots, teacher assignments, attendance records. Calls Organizations to authorize write operations. | REST, own PostgreSQL DB |
| Payments Service | Owns lesson packages, balance ledger per student per tenant. Consumes attendance events to deduct lessons. | REST, own PostgreSQL DB |
| API Gateway | Path-based routing. Does NOT check permissions — only validates JWT signature via Keycloak JWKS. | YARP (existing) |
| Keycloak | Issues JWT on login. Stores `profile_id` and `tenant_id` as custom attributes. No AuthZ. | Existing |
| Kafka | Carries integration events between services. CloudEvents format (existing serialization infrastructure). | Existing |

---

## Recommended Project Structure

Each new service mirrors the Persona service layout exactly. No new patterns are introduced.

```
src/Services/
├── Organizations/
│   └── Edvantix.Organizations/
│       ├── Domain/
│       │   ├── AggregatesModel/
│       │   │   ├── RoleAggregate/           # Role (AR), Permission (value object)
│       │   │   │   ├── Role.cs
│       │   │   │   ├── Permission.cs        # record — wraps string identifier
│       │   │   │   └── IRoleRepository.cs
│       │   │   └── MembershipAggregate/     # UserTenantRole (AR)
│       │   │       ├── UserTenantRole.cs    # userId + tenantId + roleId
│       │   │       └── IMembershipRepository.cs
│       │   └── Events/
│       │       └── TenantRoleAssignedDomainEvent.cs
│       ├── Features/
│       │   ├── Roles/
│       │   │   ├── CreateRole/
│       │   │   ├── AssignPermission/
│       │   │   └── ListRoles/
│       │   ├── Memberships/
│       │   │   ├── AssignRole/
│       │   │   └── RevokeRole/
│       │   └── Permissions/
│       │       └── CheckPermission/         # query — used by other services
│       ├── Infrastructure/
│       │   └── EntityConfigurations/
│       └── IntegrationEvents/
│           └── TenantRoleAssignedIntegrationEvent.cs
│
├── Scheduling/
│   └── Edvantix.Scheduling/
│       ├── Domain/
│       │   ├── AggregatesModel/
│       │   │   ├── GroupAggregate/          # Group (AR), Student membership
│       │   │   │   ├── Group.cs
│       │   │   │   └── IGroupRepository.cs
│       │   │   └── SlotAggregate/           # TimeSlot (AR), AttendanceRecord
│       │   │       ├── TimeSlot.cs          # tenantId + groupId + teacherId + datetime
│       │   │       ├── AttendanceRecord.cs  # slotId + studentId + status (present/absent)
│       │   │       └── ISlotRepository.cs
│       │   └── Events/
│       │       └── AttendanceRecordedDomainEvent.cs
│       ├── Features/
│       │   ├── Groups/
│       │   ├── Slots/
│       │   │   ├── CreateSlot/
│       │   │   ├── ListSlots/               # filtered by role: all (manager), own (teacher/student)
│       │   │   └── RecordAttendance/
│       │   └── Attendance/
│       ├── Infrastructure/
│       │   ├── Authorization/
│       │   │   └── PermissionAuthorizationBehavior.cs  # calls Organizations
│       │   └── EntityConfigurations/
│       └── IntegrationEvents/
│           └── AttendanceRecordedIntegrationEvent.cs   # published after slot attendance saved
│
├── Payments/
│   └── Edvantix.Payments/
│       ├── Domain/
│       │   ├── AggregatesModel/
│       │   │   └── LessonPackageAggregate/  # LessonPackage (AR), LessonLedgerEntry
│       │   │       ├── LessonPackage.cs     # tenantId + studentId + total + used
│       │   │       ├── LessonLedgerEntry.cs # packageId + slotId + deductedAt + status
│       │   │       └── ILessonPackageRepository.cs
│       │   └── Events/
│       │       └── LessonDeductedDomainEvent.cs
│       ├── Features/
│       │   ├── Packages/
│       │   │   ├── CreatePackage/           # manager: manual entry
│       │   │   ├── GetBalance/              # student: own balance
│       │   │   └── ListPackages/            # manager: all students
│       │   └── Ledger/
│       │       └── GetLessonStatus/         # oплачен / не оплачен per slot
│       ├── Infrastructure/
│       │   ├── IntegrationEventHandlers/
│       │   │   └── AttendanceRecordedIntegrationEventHandler.cs
│       │   └── EntityConfigurations/
│       └── IntegrationEvents/               # consumed, not produced in v1
```

### Structure Rationale

- **`Domain/AggregatesModel/`**: Matches Persona pattern exactly. Each aggregate root lives in its own folder with a co-located repository interface.
- **`Features/`**: Vertical slice per use case. Command + Handler + Endpoint + Validator in one folder.
- **`Infrastructure/Authorization/`**: Permission pipeline behavior lives here, not in the domain layer, because it talks to an external service (Organizations).
- **`IntegrationEvents/`**: Events produced by this service that cross service boundaries. Consumed events are handled inside `Infrastructure/IntegrationEventHandlers/`.

---

## Architectural Patterns

### Pattern 1: Permission = Command Identifier as String Constant

**What:** Each write command declares a `public const string PermissionKey` equal to a namespaced string like `scheduling.create-slot`. A MediatR/Mediator pipeline behavior looks up this constant on the command type via reflection or interface, then calls Organizations to verify that the current user (profileId + tenantId from JWT) holds that permission.

**When to use:** Every command that mutates state in Scheduling or Payments. Queries use role-based filtering (manager sees all, teacher/student sees own) but do not call Organizations — they filter in the query handler using the role claim that Keycloak already embedded in the JWT.

**Trade-offs:** Synchronous HTTP call on every command adds latency. Mitigated by caching permission sets per user per tenant using the existing `IHybridCache` (Redis-backed). Cache is invalidated when Organizations publishes a `TenantRoleAssignedIntegrationEvent`.

**Example:**

```csharp
// In Scheduling service
public sealed class CreateSlotCommand : ICommand<Guid>
{
    // Convention: "{service}.{action}" — matches the string stored in Organizations DB
    public const string PermissionKey = "scheduling.create-slot";

    public required Guid TenantId { get; init; }
    public required Guid GroupId { get; init; }
    public required Guid TeacherId { get; init; }
    public required DateTimeOffset StartsAt { get; init; }
    public required int DurationMinutes { get; init; }
}

// Pipeline behavior — registered once in Scheduling service DI
internal sealed class PermissionBehavior<TMessage, TResponse>(
    IOrganizationsClient organizationsClient,
    IHybridCache cache,
    IHttpContextAccessor httpContextAccessor
) : MessagePreProcessor<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : notnull
{
    protected override async ValueTask Handle(TMessage message, CancellationToken ct)
    {
        // Retrieve PermissionKey constant via reflection from message type
        var field = typeof(TMessage).GetField("PermissionKey",
            BindingFlags.Public | BindingFlags.Static);
        if (field is null) return; // Queries have no PermissionKey — skip

        var permissionKey = (string)field.GetValue(null)!;
        var profileId = httpContextAccessor.HttpContext!.User.GetProfileId();
        var tenantId = httpContextAccessor.HttpContext!.User.GetTenantId();

        var cacheKey = $"perms:{tenantId}:{profileId}";
        var permissions = await cache.GetOrCreateAsync(
            cacheKey,
            _ => organizationsClient.GetPermissionsAsync(profileId, tenantId, ct),
            tags: [$"tenant:{tenantId}", $"user:{profileId}"]
        );

        if (!permissions.Contains(permissionKey))
            throw new ForbiddenException(permissionKey);
    }
}
```

### Pattern 2: Tenant Isolation via EF Core Named Global Query Filter

**What:** Every aggregate root entity that is tenant-scoped carries a `TenantId` column. The `DbContext` for each service registers a named global query filter (EF Core 10 feature) per entity type, automatically appending `WHERE tenant_id = @currentTenantId` to all queries. Tenant ID is resolved from the HTTP request via `IHttpContextAccessor` through a scoped `ITenantContext` service.

**When to use:** All three new services — Organizations, Scheduling, Payments. Every aggregate root is tenant-scoped.

**Trade-offs:** Named filters in EF Core 10 allow both a tenant filter and a soft-delete filter on the same entity without one overwriting the other (previous versions had this limitation). `IgnoreQueryFilters()` must never be used in production code paths — only in admin/migration tooling.

**Example:**

```csharp
// Scoped service, populated from JWT claim by middleware
public interface ITenantContext
{
    Guid TenantId { get; }
}

// In SchedulingDbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Named filter — does not conflict with soft-delete filter
    modelBuilder.Entity<TimeSlot>()
        .HasQueryFilter("TenantIsolation",
            s => s.TenantId == _tenantContext.TenantId);

    modelBuilder.Entity<Group>()
        .HasQueryFilter("TenantIsolation",
            g => g.TenantId == _tenantContext.TenantId);
}
```

### Pattern 3: Attendance-to-Payment via Integration Event (Kafka)

**What:** When a manager records attendance for a slot, the Scheduling service saves `AttendanceRecord` entities and then publishes an `AttendanceRecordedIntegrationEvent` to Kafka (via the existing `IEventDispatcher`). The Payments service consumes this event and deducts one lesson from the student's active `LessonPackage` by adding a `LessonLedgerEntry`.

**When to use:** This is the primary cross-service data flow for the payment deduction. It is the only integration point between Scheduling and Payments.

**Trade-offs:** Eventual consistency — the balance update happens after the attendance write, not atomically. For v1 (manual management, no real-time financial settlement), this is acceptable. The ledger entry stores the `slotId` for traceability.

**Example:**

```csharp
// Published by Scheduling after SaveChangesAsync
public sealed record AttendanceRecordedIntegrationEvent : IntegrationEvent
{
    public required Guid TenantId { get; init; }
    public required Guid SlotId { get; init; }
    public required Guid StudentId { get; init; }  // profileId
    public required DateTimeOffset OccurredAt { get; init; }
    // Only published for Present status — absent students are not charged
}

// Consumed by Payments
internal sealed class AttendanceRecordedIntegrationEventHandler(
    ILessonPackageRepository packages,
    ITenantContext tenantContext
) : IConsumer<AttendanceRecordedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<AttendanceRecordedIntegrationEvent> context)
    {
        var ev = context.Message;
        var activePackage = await packages.GetActivePackageAsync(
            ev.TenantId, ev.StudentId, context.CancellationToken);

        if (activePackage is null)
        {
            // No package — record as unpaid, do not throw
            return;
        }

        activePackage.DeductLesson(ev.SlotId, ev.OccurredAt);
        await packages.UnitOfWork.SaveEntitiesAsync(context.CancellationToken);
    }
}
```

---

## Data Flow

### Request Flow: Manager Creates a Slot

```
Next.js (manager)
    │  POST /scheduling/slots  {tenantId, groupId, teacherId, startsAt}
    │  Authorization: Bearer <JWT with profile_id + tenant_id claims>
    ▼
API Gateway (YARP)
    │  Validates JWT signature (Keycloak JWKS). Forwards as-is.
    ▼
Scheduling Service — Minimal API endpoint
    │  Extracts profileId + tenantId from HttpContext.User
    ▼
Mediator pipeline — PermissionBehavior
    │  Checks cache for perms:{tenantId}:{profileId}
    │  CACHE MISS → HTTP GET Organizations /permissions/{tenantId}/{profileId}
    │  Organizations returns ["scheduling.create-slot", "scheduling.list-slots", ...]
    │  Cache SET with tags [tenant:{tenantId}, user:{profileId}]
    │  Verifies "scheduling.create-slot" is present
    ▼
Mediator pipeline — ValidationBehavior (existing)
    ▼
CreateSlotCommandHandler
    │  new TimeSlot(tenantId, groupId, teacherId, startsAt, ...)
    │  slotRepo.AddAsync(slot)
    │  slotRepo.UnitOfWork.SaveEntitiesAsync()  ← EF Core interceptor dispatches domain events
    ▼
EventDispatchInterceptor (existing in Chassis)
    │  Publishes SlotCreatedDomainEvent (internal only, not to Kafka)
    ▼
Response: 201 Created { slotId }
```

### Request Flow: Manager Records Attendance

```
Next.js (manager)
    │  POST /scheduling/slots/{slotId}/attendance  [{studentId, status}, ...]
    ▼
API Gateway → Scheduling Service
    ▼
PermissionBehavior: checks "scheduling.record-attendance"
    ▼
RecordAttendanceCommandHandler
    │  foreach present student: slot.RecordAttendance(studentId, Present)
    │  slotRepo.UnitOfWork.SaveEntitiesAsync()
    │  foreach present student:
    │      eventDispatcher.Publish(AttendanceRecordedIntegrationEvent { tenantId, slotId, studentId })
    ▼
Kafka topic: edvantix.scheduling.attendance-recorded
    ▼
Payments Service consumer
    │  Finds active LessonPackage for (tenantId, studentId)
    │  package.DeductLesson(slotId, occurredAt)
    │  SaveEntitiesAsync()
    ▼
Student balance updated (eventually consistent, ~seconds)
```

### Request Flow: Student Views Balance

```
Next.js (student)
    │  GET /payments/balance   (no body — tenantId + profileId from JWT)
    ▼
API Gateway → Payments Service
    │  No PermissionBehavior for queries — queries use JWT claims directly
    ▼
GetBalanceQueryHandler
    │  profileId = user.GetProfileId(), tenantId = user.GetTenantId()
    │  EF Core global filter auto-applies WHERE tenant_id = @tenantId
    │  SELECT total_lessons, used_lessons FROM lesson_packages
    │      WHERE student_id = @profileId AND tenant_id = @tenantId
    ▼
Response: { totalLessons: 20, usedLessons: 7, remainingLessons: 13 }
```

### Key Data Flows

1. **Permission cache invalidation:** Organizations publishes `TenantRoleAssignedIntegrationEvent` when a role is assigned or revoked. Scheduling and Payments both consume this and call `IHybridCache.RemoveByTagAsync($"user:{profileId}")`, forcing fresh permission fetch on the next command.

2. **Cross-service user references:** Services store `profileId` (from Persona/Keycloak) as a plain `Guid` foreign key. They never call Persona at runtime — display names are resolved by the frontend from a separate Persona API call, or embedded in JWT claims.

3. **Tenant context propagation:** `tenantId` travels as a JWT claim (`tenant_id`) set by Keycloak on login. The `ITenantContext` middleware extracts it and makes it available to EF Core global filters and CQRS handlers. It is never trusted from request body or URL path.

---

## Build Order (Service Dependencies)

Build in this order. Each step can only start when the previous is deployable:

**Step 1 — Organizations Service** (no runtime dependency on Scheduling or Payments)
- Foundation for all permission checks.
- Until Organizations exists, the permission behavior in other services cannot be wired up.
- Deliverables: roles CRUD, permissions CRUD, user-role assignment, `CheckPermission` query endpoint.

**Step 2 — Scheduling Service** (depends on Organizations for permission checks)
- Can be built once Organizations exposes a permission query endpoint.
- Groups, time slots, teacher assignment, attendance recording.
- Publishes `AttendanceRecordedIntegrationEvent` — Payments can subscribe even before Payments is built.

**Step 3 — Payments Service** (depends on Scheduling for the integration event shape)
- Can be built independently of Scheduling at the code level but needs the event contract to exist.
- Consumes `AttendanceRecordedIntegrationEvent`, owns lesson packages and balance display.

**Rationale:** Organizations is a synchronous runtime dependency (Scheduling calls it on every command). Scheduling → Payments is asynchronous (Kafka), so Payments can be built and tested in isolation by publishing mock events.

---

## Multitenancy Isolation Strategy

### Isolation Approach: Shared Database, Separate Schemas per Service, Row-Level Tenant Filter

All three services share the same PostgreSQL cluster (existing Aspire setup) but each has its own database (`organizationsdb`, `schedulingdb`, `paymentsdb`). Within each database, every tenant-scoped table has a `tenant_id UUID NOT NULL` column with a non-nullable index.

| Layer | Enforcement Mechanism |
|-------|-----------------------|
| Application | `ITenantContext` populated from JWT, injected into `DbContext` constructor |
| ORM | EF Core 10 named global query filter — automatically appends `WHERE tenant_id = @tenantId` |
| Database (defense-in-depth) | Postgres Row-Level Security policy on tenant_id column — blocks any tenant_id mismatch even if ORM filter is bypassed |
| API | `tenantId` claim sourced exclusively from the JWT (Keycloak attribute) — never from URL or request body |

### Tenant ID Source of Truth

The `tenant_id` claim is set in Keycloak when the user authenticates. The school owner's account has a specific tenant (school) assigned at registration. A user can theoretically belong to multiple schools — the JWT carries the currently active `tenant_id`. Switching schools requires a new token (re-authentication or token refresh with a new `tenant_id` claim).

For v1, a user belongs to exactly one tenant — this simplifies the above. The multi-tenant model is forward-compatible.

---

## RBAC Authorization Middleware — How It Works

The permission check is a Mediator `MessagePreProcessor` (same pipeline mechanism as the existing `ValidationBehavior`). It runs before every command handler in the pipeline.

```
ICommand<TResult>  (has const PermissionKey)
        │
        ▼
PermissionBehavior (pre-processor)
    1. Reflect on TMessage — look for public static const string PermissionKey
    2. If absent → this is a query or a system command → skip
    3. Extract profileId + tenantId from IHttpContextAccessor
    4. Cache key = "perms:{tenantId}:{profileId}"
    5. Cache hit → deserialize HashSet<string>
       Cache miss → HTTP call to Organizations GET /api/v1/permissions/{tenantId}/{profileId}
                    → cache for 5 min with tags ["tenant:{tenantId}", "user:{profileId}"]
    6. If !permissions.Contains(PermissionKey) → throw ForbiddenException (maps to 403)
        │
        ▼
ValidationBehavior (existing)
        │
        ▼
CommandHandler
```

Permission identifiers are **string constants defined in the command class itself**, not in a central enum or database. The Organizations service stores the same strings in its `permissions` table. The convention `{service}.{action-in-kebab-case}` (e.g., `scheduling.create-slot`, `payments.add-package`) is enforced by code review, not by the type system — this is a deliberate trade-off for simplicity.

The Organizations service exposes a single permission check endpoint:

```
GET /api/v1/permissions/{tenantId}/{profileId}
→ 200 OK  ["scheduling.create-slot", "scheduling.list-slots", "payments.view-balance"]
→ 404 Not Found  (user has no role in this tenant)
```

The endpoint returns the full flat permission list (not a yes/no per permission). This design allows the cache to store the full set once and the behavior to check locally, avoiding one round-trip per permission.

---

## Integration Points

### Internal Service-to-Service

| Boundary | Communication | Direction | Notes |
|----------|---------------|-----------|-------|
| Scheduling → Organizations | HTTP (REST) | Sync | Permission check on every command. Cache with Redis HybridCache. |
| Scheduling → Payments | Kafka (CloudEvents) | Async | `AttendanceRecordedIntegrationEvent`. One-way. |
| Organizations → Scheduling/Payments | Kafka (CloudEvents) | Async | `TenantRoleAssignedIntegrationEvent` for cache invalidation. |

### External Services

| Service | Integration Pattern | Notes |
|---------|---------------------|-------|
| Keycloak | JWT validation (existing) | Adds `tenant_id` claim. No new Keycloak AuthZ rules. |
| PostgreSQL | EF Core per-service DB | Three new databases on existing cluster. |
| Kafka | MassTransit CloudEvents (existing) | New topics per service. |
| Redis | IHybridCache (existing) | Shared Redis for permission cache; key-scoped to prevent collisions. |

---

## Anti-Patterns

### Anti-Pattern 1: Storing Permission Checks Inside Domain Layer

**What people do:** Put the `organizationsClient.HasPermissionAsync()` call inside the aggregate or domain service to "protect" the domain.

**Why it's wrong:** The domain layer would then depend on an external HTTP service, making unit tests impossible without mocks and breaking the dependency inversion principle. Domain rules must be self-contained.

**Do this instead:** Permission checks belong in the application layer (CQRS pipeline behavior). The domain aggregate enforces invariants about its own state; authorization is enforced before the command reaches the handler.

### Anti-Pattern 2: Embedding TenantId in URL Path as Authorization

**What people do:** `/api/schools/{tenantId}/slots` and trust the `{tenantId}` path segment for data isolation.

**Why it's wrong:** A user could substitute a different `tenantId` in the URL and receive or modify another school's data if the filter is the URL parameter instead of the JWT claim.

**Do this instead:** `tenantId` is always sourced from the JWT claim via `ITenantContext`. URL path parameters (e.g., `/api/slots/{slotId}`) contain only resource IDs; the tenant filter is applied transparently by EF Core global filters.

### Anti-Pattern 3: Synchronous Cross-Service Payment Deduction

**What people do:** Call Payments HTTP API inline in the `RecordAttendanceCommandHandler` — one HTTP call per present student.

**Why it's wrong:** This creates tight coupling, multiplies latency (N HTTP calls for N students), and causes the attendance write to fail if Payments is unavailable.

**Do this instead:** Publish a single `AttendanceRecordedIntegrationEvent` per student to Kafka after the attendance is saved. Payments processes it asynchronously. Attendance recording succeeds regardless of Payments availability.

### Anti-Pattern 4: Central Permission Enum Shared Across Services

**What people do:** Define a shared `Permissions` enum or constants class in `Edvantix.Constants` and reference it from every service.

**Why it's wrong:** This creates a build-time coupling between all three services and `Edvantix.Constants`. Adding a new permission to Scheduling forces a release of all services.

**Do this instead:** Permission strings are `const` in each command class. The Organizations service validates permission strings only at assignment time (business validation in the domain), not through a shared type.

---

## Scaling Considerations

| Scale | Architecture Adjustments |
|-------|--------------------------|
| 0-500 concurrent users (v1) | Single instance per service, shared Postgres cluster, Redis permission cache with 5-min TTL is sufficient |
| 500-5k users | Permission cache TTL can be reduced to 1 min; add read replicas for Scheduling queries (schedule list is read-heavy) |
| 5k+ users | Consider read model (separate projections table) for schedule queries; Organizations permission check path should be fully cached — HTTP calls should be rare |

### Scaling Priorities

1. **First bottleneck:** Permission check HTTP call on cache miss. Fix: ensure Redis cache hit rate is high; pre-warm cache on login via `TenantRoleAssignedIntegrationEvent`.
2. **Second bottleneck:** Schedule list queries (teacher sees N slots per week, manager sees all). Fix: add a non-clustered index on `(tenant_id, teacher_id, starts_at)` and `(tenant_id, group_id, starts_at)`.

---

## Sources

- [EF Core Multi-tenancy documentation — Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/miscellaneous/multitenancy)
- [EF Core Global Query Filters — Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/querying/filters)
- [EF Core named global query filters (.NET 10) — codewithmukesh](https://codewithmukesh.com/blog/global-query-filters-efcore/)
- [MediatR pipeline behaviors for authorization — Austin Davies](https://medium.com/@austin.davies0101/creating-a-basic-authorization-pipeline-with-mediatr-and-asp-net-core-c257fe3cc76b)
- [Authentication and Authorization in DDD — José Luis Martínez](https://medium.com/@martinezdelariva/authentication-and-authorization-in-ddd-671f7a5596ac)
- [Domain event pattern — microservices.io](https://microservices.io/patterns/data/domain-event.html)
- Existing codebase: `Edvantix.Chassis` (CQRS pipelines, EventBus, Caching, Security), `Edvantix.Persona` (reference service structure)

---

*Architecture research for: Edvantix — Organizations/RBAC, Scheduling, Payments microservices*
*Researched: 2026-03-18*
