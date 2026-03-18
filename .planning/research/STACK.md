# Stack Research

**Domain:** Online school management SaaS — Organizations (custom RBAC), Scheduling (lesson slots), Payments (package-based lessons)
**Researched:** 2026-03-18
**Confidence:** HIGH (existing codebase patterns verified; library versions verified against Directory.Packages.props and Versions.props)

---

## Context: What Already Exists

This is a targeted addendum to an existing stack. The base infrastructure is fixed:

- .NET 10 + ASP.NET Core 10 (`AspNetVersion: 10.0.5`)
- PostgreSQL via `Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL` (`AspireVersion: 13.1.2`)
- Redis via `Aspire.Microsoft.Azure.StackExchangeRedis` (HybridCache layer in `Edvantix.Chassis`)
- Kafka via `MassTransit.Kafka` (`MassTransitVersion: 8.5.8`) with CloudEvents envelope
- gRPC via `Grpc.AspNetCore` (`GrpcVersion: 2.76.0`)
- CQRS via `Mediator.SourceGenerator` + `Mediator.Abstractions` (`3.0.1`)
- FluentValidation `12.1.1` in the CQRS pipeline
- Keycloak JWT authentication only (no Keycloak AuthZ)
- `Edvantix.Chassis` building block: `IRepository<T>`, `DatabaseContext`, `Specification`, `HybridCacheService`, `EventBus`

The three new services (Organizations, Scheduling, Payments) must follow the same internal patterns.

---

## Recommended Stack

### Core Technologies (all three new services)

| Technology | Version | Purpose | Why Recommended |
|------------|---------|---------|-----------------|
| EF Core (via `Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL`) | 13.1.2 (Aspire wraps EF Core 10) | Database ORM per service | Already in use; `PostgresContext` base class in Chassis handles migrations, OTel, config. Mandatory for consistency. |
| `MassTransit.Kafka` | 8.5.8 | Integration events (cross-service) | Already registered in Chassis `AddEventBus()`. Permission-changed, lesson-attended, and lesson-package-updated events must propagate via Kafka. Do not add a second bus. |
| `Mediator.SourceGenerator` + `Mediator.Abstractions` | 3.0.1 | In-process CQRS | Already used in Persona; source-generated dispatch, zero reflection overhead. All commands/queries go through it. Already wired with validation and activity pipelines in Chassis. |
| `Grpc.AspNetCore` | 2.76.0 | Inter-service RPC (permission checks) | Organizations service exposes a gRPC endpoint for permission verification. Scheduling and Payments call it via `AddGrpcServiceReference<T>` from ServiceDefaults. Same pattern as Persona's `ProfileService`. |
| `Microsoft.Extensions.Caching.Hybrid` | 10.4.0 | Permission cache per-user per-school | Already wrapped in `Edvantix.Chassis.Caching.HybridCacheService`. Scheduling/Payments cache the resolved permission set for a (userId, schoolId) pair. Invalidated via Kafka `RoleAssignmentChangedEvent`. |

### Organizations Service — Custom RBAC

| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| `Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL` | 13.1.2 | Organizations database (roles, permissions, assignments) | Always — own isolated DB, tenant-scoped via `schoolId` global query filter |
| `MassTransit.EntityFrameworkCore` | 8.5.8 | Transactional outbox for role-change events | When publishing `RoleAssignedToUserEvent`, `PermissionRevokedEvent` — guarantees event delivery within the same transaction as DB write |
| `Mediator.SourceGenerator` | 3.0.1 | CQRS dispatch | All create/assign/revoke commands go through Mediator |
| `Grpc.AspNetCore` | 2.76.0 | Serve `CheckPermission` RPC to other services | Organizations is the authority; other services call it synchronously at request time (first call), then cache the result |
| `EFCore.NamingConventions` | 10.0.1 | Snake_case column names | Already used project-wide (`UseSnakeCaseNamingConvention()`) |

### Scheduling Service — Lesson Slots + Attendance

| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| `Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL` | 13.1.2 | Scheduling database | Own DB; `Slot`, `Attendance` entities scoped by `schoolId` |
| `MassTransit.Kafka` | 8.5.8 | Publish `AttendanceMarkedEvent` | Consumed by Payments service to debit lesson from package |
| `MassTransit.EntityFrameworkCore` | 8.5.8 | Transactional outbox for attendance events | Guarantees Payments receives the event even if Kafka is momentarily unavailable |
| `Mediator.SourceGenerator` | 3.0.1 | CQRS dispatch | Slot creation, attendance marking, schedule queries |
| `Grpc.Tools` | 2.78.0 | gRPC client codegen (consume Organizations CheckPermission) | Used in Persona already; same pattern for Scheduling to call Organizations |
| `Microsoft.Extensions.Caching.Hybrid` | 10.4.0 | Cache permission set from Organizations | Per-request permission lookup is a gRPC call; cache TTL 5 min, invalidated by Kafka `RoleAssignmentChangedEvent` |

### Payments Service — Package Balance + Lesson Debit

| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| `Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL` | 13.1.2 | Payments database | Own DB; `LessonPackage`, `LessonTransaction` (append-only ledger) |
| `MassTransit.Kafka` | 8.5.8 | Consume `AttendanceMarkedEvent` from Scheduling | Debit lesson from package balance when attendance confirmed |
| `MassTransit.EntityFrameworkCore` | 8.5.8 | Transactional inbox (idempotent consumption) | Prevents double-debit if `AttendanceMarkedEvent` is delivered more than once |
| `Mediator.SourceGenerator` | 3.0.1 | CQRS dispatch | Package purchase commands, balance queries |
| `Grpc.Tools` | 2.78.0 | gRPC client codegen | Payments calls Organizations for permission checks; same Chassis pattern |
| `Microsoft.Extensions.Caching.Hybrid` | 10.4.0 | Permission cache | Same as Scheduling service |

### Development Tools

| Tool | Purpose | Notes |
|------|---------|-------|
| `Microsoft.EntityFrameworkCore.Tools` | EF migrations | Add to each service `.csproj`; already in `Directory.Packages.props` |
| `EFCore.NamingConventions` | Snake_case in PostgreSQL | Already versioned at `10.0.1`; call `UseSnakeCaseNamingConvention()` in `OnConfiguring` |
| `Bogus` + `TUnit` | Test data generation + unit tests | Already versioned project-wide |
| `Verify.TUnit` + `Verify.MassTransit` | Contract / snapshot testing | Follow existing `*.ContractTests` project pattern per service |

---

## Installation

```bash
# Per service .csproj — copy from Persona as baseline
# Core (all three new services)
<PackageReference Include="Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL" />
<PackageReference Include="MassTransit.EntityFrameworkCore" />
<PackageReference Include="Mediator.SourceGenerator" PrivateAssets="all" />
<PackageReference Include="Mediator.Abstractions" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
<PackageReference Include="Microsoft.Extensions.Caching.Hybrid" />
<PackageReference Include="EFCore.NamingConventions" />

# Organizations only (serves gRPC)
<PackageReference Include="Grpc.Tools" PrivateAssets="all" />
<Protobuf Include="Grpc/Protos/permissions/v1/permissions.proto" GrpcService="Server" />

# Scheduling + Payments (consume gRPC from Organizations)
<PackageReference Include="Grpc.Tools" PrivateAssets="all" />
<Protobuf Include="Grpc/Protos/permissions/v1/permissions.proto" GrpcService="Client" />
```

All version pins live in `Directory.Packages.props` via `Versions.props` — never hardcode versions in service `.csproj` files.

---

## EF Core Patterns Per Service

### Multitenancy: Global Query Filter on schoolId

Every entity in all three services that is school-scoped must carry a `Guid SchoolId` property. The `DbContext` applies a global filter during `OnModelCreating`:

```csharp
// In each DbContext (Organizations, Scheduling, Payments)
modelBuilder.Entity<Role>().HasQueryFilter(r => r.SchoolId == _currentSchoolId);
```

`_currentSchoolId` is resolved from `IHttpContextAccessor` → JWT claim `school_id` injected into the DbContext via constructor. This is the same approach as the existing `ProfileRegisteredRequirement` pattern in Chassis — pull the claim, fail fast if absent.

Do NOT use PostgreSQL Row-Level Security for this. RLS requires database-level role switching or `SET LOCAL` calls that are incompatible with connection pooling (Npgsql PgBouncer transaction mode). The EF Core global query filter achieves the same isolation at the application layer without connection pool penalty.

### Outbox (Organizations, Scheduling)

Both services publish integration events that must not be lost. Follow the Persona pattern exactly:

```csharp
// In OnModelCreating
modelBuilder.AddOutboxMessageEntity();
modelBuilder.AddOutboxStateEntity();
modelBuilder.AddInboxStateEntity();
```

MassTransit transactional outbox is already configured via `MassTransit.EntityFrameworkCore`.

### Ledger Pattern (Payments only)

`LessonTransaction` is append-only — never update or delete rows. Balance is always computed as `SUM(credits) - SUM(debits)` over all transactions for a `(studentId, packageId)`. Never store a mutable `balance` column. This avoids lost-update concurrency bugs without needing distributed locks.

```
LessonPackage (aggregate root)
  |-- LessonTransaction[] (append-only, event sourced)
       type: Credit | Debit
       amount: int (lesson count, not money)
       referenceId: Guid (lesson slot or manual adjustment)
       createdAt: DateTimeOffset
```

---

## Integration Events Between Services

| Event | Publisher | Consumer | Transport | Purpose |
|-------|-----------|----------|-----------|---------|
| `RoleAssignedToUserEvent` | Organizations | Scheduling, Payments | Kafka | Invalidate cached permission sets in downstream services |
| `PermissionRevokedEvent` | Organizations | Scheduling, Payments | Kafka | Same — cache invalidation |
| `AttendanceMarkedEvent` | Scheduling | Payments | Kafka | Trigger lesson debit from student package |
| `LessonPackagePurchasedEvent` | Payments | (future) | Kafka | For audit / future notifications |

All events use the existing `CloudEvents` envelope (`CloudEventKafkaSerializer`) and the `kebab-case` topic naming convention already in `AddEventBus()`.

Event contract placement: Each service defines its own events under `IntegrationEvents/`. Consuming services reference a shared contract project or duplicate the record — follow the pattern in existing services (Scheduler already has `IntegrationEvents/` under its project).

---

## Permission Check Architecture

Organizations exposes a gRPC `PermissionsService` with a single RPC:

```protobuf
// permissions/v1/permissions.proto
rpc CheckPermission (CheckPermissionRequest) returns (CheckPermissionResponse);

message CheckPermissionRequest {
  string user_id = 1;
  string school_id = 2;
  string permission = 3;  // e.g. "scheduling.create-slot"
}

message CheckPermissionResponse {
  bool allowed = 1;
}
```

Scheduling and Payments register the gRPC client via the existing `AddGrpcServiceReference<PermissionsService.PermissionsServiceClient>()` extension from ServiceDefaults.

Permission results are cached in `HybridCacheService` with key `perm:{userId}:{schoolId}:{permission}` and tag `user-perms:{userId}:{schoolId}`. When `RoleAssignmentChangedEvent` arrives via Kafka, the consumer calls `RemoveByTagAsync("user-perms:{userId}:{schoolId}")` to invalidate the set atomically.

This avoids a synchronous gRPC round-trip on every request after the first.

---

## Alternatives Considered

| Recommended | Alternative | Why Not |
|-------------|-------------|---------|
| Custom RBAC via Organizations gRPC | Keycloak Authorization Services | Keycloak AuthZ is fine for static roles defined at deploy time; dynamic runtime creation of roles/permissions by school owners requires the Keycloak Admin API and tight coupling to Keycloak internals. Too rigid and operationally fragile for per-tenant customization. |
| EF Core global query filter for multitenancy | PostgreSQL Row-Level Security | RLS requires `SET LOCAL` per-connection for tenant context, incompatible with transaction-mode connection pooling (PgBouncer). The project uses Azure Flexible Server with Npgsql; adding RLS introduces connection pool management complexity for no measurable benefit at this scale. |
| Append-only ledger for lesson balance | Mutable `balance` column with optimistic concurrency | Mutable balance requires concurrency tokens and retry logic. Append-only ledger is simpler to implement, naturally auditable, and idempotent when replaying `AttendanceMarkedEvent`. |
| MassTransit transactional outbox for cross-service events | Direct Kafka publish in command handler | Direct publish can lose events if the service crashes after DB write but before Kafka send. Outbox + EF Core savepoint keeps DB write and event publication atomic. Already established pattern in Persona. |
| `Mediator.SourceGenerator` | MediatR | Project already uses `Mediator.SourceGenerator`. MediatR is reflection-based and would be an inconsistency. Do not mix. |
| Refit for HTTP service references | HttpClientFactory + manual `HttpClient` | `AddHttpServiceReference<T>()` in ServiceDefaults already wraps Refit. Use it for any REST-over-HTTP inter-service calls (vs gRPC for low-latency permission checks). |

---

## What NOT to Use

| Avoid | Why | Use Instead |
|-------|-----|-------------|
| Keycloak Authorization Services / UMA | Dynamic per-tenant roles require Admin API calls on every role change; AuthZ evaluation is external HTTP latency per request; no offline evaluation. Overly complex for this use case. | Custom RBAC in Organizations service with gRPC + HybridCache |
| PostgreSQL schemas per tenant (schema-per-school) | Requires dynamic schema creation; EF migrations become unmanageable at scale; Npgsql connection strings hardcode schema. | `schoolId` column + global query filter |
| Event sourcing full CQRS (separate read/write DBs) | Three new services are CRUD-heavy with simple read paths. Full event sourcing + projection stores add operational overhead without benefit at this scope. | Standard EF Core with append-only ledger only where audit is required (Payments) |
| Ardalis.Specification NuGet package | Project already has a custom `Specification` implementation in `Edvantix.Chassis.Specification`. Adding Ardalis would create two competing specification systems. | Existing `Specification<T>` base class in Chassis |
| AutoMapper | Not present in project. Persona uses manual mappers (see `Features/Profiles/Mappers/`). Adding AutoMapper creates inconsistency. | Manual mapping classes following existing `*Mapper.cs` conventions |
| Quartz.NET in new services | Quartz is the internal scheduler service's concern (job-based background tasks for notifications). Lesson slot scheduling is calendar data stored in the DB, not background jobs. | EF Core entities for `LessonSlot` with datetime fields |

---

## Version Compatibility

| Package | Compatible With | Notes |
|---------|-----------------|-------|
| `MassTransit.EntityFrameworkCore` 8.5.8 | EF Core 10 (via Aspire 13.1.2) | HIGH confidence. MassTransit 8.x officially supports EF Core 8+ and tracks EF Core releases. Verified: `Directory.Packages.props` already pins both together in Persona. |
| `Mediator.SourceGenerator` 3.0.1 | .NET 10, C# 13 | HIGH confidence. NuGet page confirms 3.0.1 supports net8.0+ targets. Project's `global.json` sets SDK to .NET 10. |
| `Microsoft.Extensions.Caching.Hybrid` 10.4.0 | ASP.NET Core 10 | HIGH confidence. `MsExtVersion` pin in `Versions.props` tracks `Microsoft.Extensions.*` at same major version as runtime. |
| `Grpc.Tools` 2.78.0 | `Grpc.AspNetCore` 2.76.0 | HIGH confidence. Grpc.Tools is a build-time codegen tool; minor version mismatch (2.78 vs 2.76) is safe and already in `Directory.Packages.props`. |
| `EFCore.NamingConventions` 10.0.1 | EF Core 10 | HIGH confidence. Package major version matches EF Core major version by convention. Already versioned at 10.0.1 in `Directory.Packages.props`. |

---

## Sources

- `E:/projects/Edvantix/Directory.Packages.props` — version pins for all existing packages (HIGH confidence)
- `E:/projects/Edvantix/Versions.props` — Aspire 13.1.2, MassTransit 8.5.8, Mediator 3.0.1, Grpc 2.76.0
- `E:/projects/Edvantix/src/Services/Persona/Edvantix.Persona/Edvantix.Persona.csproj` — gRPC server + outbox + HybridCache pattern (HIGH confidence)
- `E:/projects/Edvantix/src/BuildingBlocks/Edvantix.Chassis/EventBus/Extensions.cs` — Kafka/CloudEvents/outbox wiring pattern (HIGH confidence)
- `E:/projects/Edvantix/src/BuildingBlocks/Edvantix.Chassis/Caching/HybridCacheService.cs` — tag-based cache invalidation pattern (HIGH confidence)
- `E:/projects/Edvantix/src/Aspire/Edvantix.ServiceDefaults/Kestrel/ServiceReferenceExtensions.cs` — `AddGrpcServiceReference<T>` pattern (HIGH confidence)
- `E:/projects/Edvantix/src/Aspire/Edvantix.AppHost/AppHost.cs` — Aspire resource wiring: Postgres, Redis, Kafka, gRPC references (HIGH confidence)
- [MassTransit EF Core Outbox docs](https://masstransit.io/documentation/patterns/saga/entity-framework) — outbox + inbox configuration (MEDIUM confidence — web reference)
- [ASP.NET Core HybridCache docs](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-10.0) — tag-based invalidation API (HIGH confidence — official MS docs)
- [ASP.NET Core custom authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-8.0) — `IAuthorizationHandler` + `IAuthorizationRequirement` for permission policy gate (MEDIUM confidence — web reference, ASP.NET Core 8 docs apply to 10)

---

*Stack research for: Edvantix — Organizations, Scheduling, Payments microservices*
*Researched: 2026-03-18*
