# Phase 2: Organizations — Permission Cache - Research

**Researched:** 2026-03-19
**Domain:** gRPC server, Microsoft.Extensions.Caching.Hybrid, MassTransit outbox + Kafka, ASP.NET Core authorization handlers
**Confidence:** HIGH

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions

**Cache architecture:**
- Cache lives only in Organizations; downstream services always go via gRPC
- L1 = in-memory (30 s TTL), L2 = Redis (5 min TTL) — satisfies success criteria ≤ 60 s
- Cache key: `perm:{userId}:{schoolId}:{permission}` (one key per triple)
- Cache tag: `user:{userId}:{schoolId}` — RemoveByTagAsync evicts everything for a user in a school

**gRPC CheckPermission endpoint:**
- New gRPC server in Organizations: `CheckPermission(userId, schoolId, permission) → bool`
- Proto file: `src/Services/Organizations/Edvantix.Organizations/Grpc/Protos/organizations/v1/permissions.proto`
- Downstream services copy the proto and wrap it in an interface (pattern from Phase 1: `IPersonaProfileService`)

**GET /permissions endpoint (cache priming):**
- REST endpoint: `GET /permissions?userId={id}&schoolId={id}` → `string[]` of all permissions for the user in the school
- AllowAnonymous — service-to-service only (analogous to RegisterPermissions from Phase 1; TODO mTLS)
- Downstream service can call this on startup to warm L2 Redis

**Invalidation events:**
- Single generalised event: `UserPermissionsInvalidated` with payload `{ UserId: Guid, SchoolId: Guid, Timestamp: DateTimeOffset }`
- Published by Organizations on: AssignRole, RevokeRole, AssignPermissions (to role)
- EventMapper (`Infrastructure/EventServices/EventMapper.cs` stub) extended to map domain events → UserPermissionsInvalidated
- Organizations invalidates its own cache immediately in the command handler via RemoveByTagAsync before publish — atomic with commit

**Authorization in downstream services:**
- `AddPermissionAuthorization(organizationsGrpcBaseUrl)` extension in Chassis
- ASP.NET Core `IAuthorizationHandler` + `IAuthorizationRequirement` (pattern like ProfileRegisteredRequirement in Phase 1)
- Endpoint registration: `.RequireAuthorization("scheduling.create-slot")` — permission string as policy name
- On denial: 403 Forbidden via standard ASP.NET Core authz (Problem Details automatically)

### Claude's Discretion
- Concrete proto design (streaming vs unary — unary obvious)
- Class names for Requirement/Handler in Chassis
- Kafka topic name for UserPermissionsInvalidated (follow KebabCase formatter)

### Deferred Ideas (OUT OF SCOPE)
- None — discussion stayed within Phase 2 scope
</user_constraints>

<phase_requirements>
## Phase Requirements

| ID | Description | Research Support |
|----|-------------|-----------------|
| ORG-07 | Organizations service provides gRPC endpoint CheckPermission(userId, schoolId, permission) → bool | gRPC server pattern verified against existing proto + GrpcServices="Server" in csproj; unary RPC is correct choice |
| ORG-08 | CheckPermission result cached in HybridCache with invalidation on role change event | IHybridCache.GetOrCreateAsync + RemoveByTagAsync verified in Chassis; CachingExtensions.AddCaching wires L1+L2 |
| ORG-09 | On role/assignment change, integration event published for cache invalidation in other services | EventMapper + IEventDispatcher + MassTransit outbox pattern verified in Persona; same flow applies to Organizations |
</phase_requirements>

## Summary

Phase 2 builds the permission query surface on top of the Phase 1 RBAC domain. The core delivery is a gRPC `CheckPermission` unary RPC inside the Organizations service, backed by a two-level cache (in-process `Microsoft.Extensions.Caching.Hybrid` as L1; Redis as L2) using the existing `IHybridCache` Chassis abstraction. Cache invalidation is event-driven: domain events raised during role/assignment mutation are translated by `EventMapper` into a `UserPermissionsInvalidated` integration event, published through the MassTransit outbox that is already configured in `Extensions.cs`. Downstream services will call the gRPC endpoint via a Chassis `IAuthorizationHandler`; they do not maintain their own cache.

The architecture leverages exclusively existing Chassis infrastructure. `IHybridCache` (with `GetOrCreateAsync` + `RemoveByTagAsync`) is already production-ready. The MassTransit + Kafka outbox is already configured in `OrganizationsDbContext` (`AddInboxStateEntity`, `AddOutboxMessageEntity`, `AddOutboxStateEntity`) and in `Extensions.cs` (`AddEntityFrameworkOutbox`). The gRPC client pattern (`GrpcServices="Client"`) already exists; this phase introduces the server-side mirror (`GrpcServices="Server"`). The Chassis `IAuthorizationRequirement` / `IAuthorizationHandler` pattern already demonstrated by `ProfileRegisteredRequirement` is the template for `PermissionRequirement` + `PermissionRequirementHandler`.

The main new wiring is: (1) add Redis reference to Organizations in AppHost; (2) call `builder.AddCaching()` in Organizations `Extensions.cs`; (3) add the new proto with `GrpcServices="Server"`; (4) publish `UserPermissionsInvalidated` on three commands; (5) extend `EventMapper` switch; (6) add `AddPermissionAuthorization` extension to Chassis.

**Primary recommendation:** Follow every existing pattern exactly — gRPC client↔server inversion, EventMapper switch extension, CachingExtensions wiring, PolicyBuilderExtensions model for Chassis authorization. Introduce no new abstractions.

## Standard Stack

### Core

| Library | Version | Purpose | Why Standard |
|---------|---------|---------|--------------|
| `Microsoft.Extensions.Caching.Hybrid` | Ships with .NET 9+ | L1 in-memory + L2 distributed cache | Already in Chassis; wraps both tiers uniformly |
| `Grpc.AspNetCore.Server.ClientFactory` | Already in Organizations.csproj | gRPC server + client | Project-standard; used for Persona client |
| `Grpc.Tools` | Already in Organizations.csproj | Proto code generation | Project-standard |
| `MassTransit.EntityFrameworkCore` | Already in Organizations.csproj | Outbox pattern | Already configured in DbContext + Extensions |
| `StackExchange.Redis` / `Aspire.StackExchange.Redis` | Via Aspire `AddAzureManagedRedis` | L2 distributed cache backend | Already in AppHost; Blog service uses it |

### Supporting

| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| `Edvantix.Chassis.Caching.IHybridCache` | Internal | Cache abstraction | All cache calls inside CheckPermission handler |
| `Edvantix.Chassis.EventBus.Dispatcher.IEventMapper` | Internal | Domain-to-integration mapping | Extending switch in `EventMapper.cs` |
| `Microsoft.AspNetCore.Authorization` | Framework | IAuthorizationRequirement / IAuthorizationHandler | Chassis PermissionRequirement |

### Alternatives Considered

| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| `IHybridCache` (Chassis) | Raw `HybridCache` | Chassis abstraction is already there; direct use breaks testability |
| Per-triple cache key | Per-user-school blob | Per-triple is simpler, tag-based eviction handles bulk invalidation |
| Single gRPC stream | Unary per call | Unary is correct for request/response; streaming is out of scope |

**Installation:** No new packages required. Redis reference added to AppHost for Organizations only.

```bash
# In AppHost.cs — add to organizationsApi:
.WithReference(redis)
.WaitFor(redis)
```

## Architecture Patterns

### Recommended Project Structure

New files follow existing feature-folder and infrastructure conventions:

```
src/Services/Organizations/Edvantix.Organizations/
├── Domain/
│   └── AggregatesModel/
│       ├── UserRoleAssignmentAggregate/
│       │   └── UserRoleAssignment.cs            # Add RegisterDomainEvent calls
│       └── RoleAggregate/
│           └── Role.cs                          # Add RegisterDomainEvent calls
├── Features/
│   └── Permissions/
│       └── CheckPermission/
│           ├── CheckPermissionQuery.cs          # IQuery<bool> + handler with IHybridCache
│           └── GetPermissionsEndpoint.cs        # GET /permissions cache-priming endpoint
├── Grpc/
│   ├── Protos/
│   │   └── organizations/v1/
│   │       └── permissions.proto                # New proto: CheckPermission unary RPC
│   └── Services/
│       └── PermissionsGrpcService.cs            # Inherits generated base class
├── Infrastructure/
│   ├── EventServices/
│   │   ├── EventMapper.cs                       # Extend switch for UserPermissionsInvalidated
│   │   └── Events/
│   │       ├── UserRoleAssignedEvent.cs         # Domain event (sealed, extends DomainEvent)
│   │       └── UserRoleRevokedEvent.cs          # Domain event (sealed, extends DomainEvent)
│   └── IntegrationEvents/
│       └── UserPermissionsInvalidatedIntegrationEvent.cs  # In Edvantix.Contracts namespace
└── Extensions/
    └── Extensions.cs                            # Add builder.AddCaching() call

src/BuildingBlocks/Edvantix.Chassis/
└── Security/
    └── Authorization/
        ├── PermissionRequirement.cs              # IAuthorizationRequirement with permission string
        ├── PermissionRequirementHandler.cs       # Calls Organizations gRPC
        └── (Extensions to PolicyBuilderExtensions.cs)
```

### Pattern 1: gRPC Server (inversion of existing client pattern)

**What:** Organizations exposes a gRPC server endpoint. The `.csproj` uses `GrpcServices="Server"` instead of `"Client"`. The generated base class is inherited by a concrete service class.

**When to use:** When Organizations needs to serve — not consume — a gRPC contract.

```csharp
// Source: existing pattern in Edvantix.Organizations.csproj (GrpcServices="Client" → Server)
// In Edvantix.Organizations.csproj:
<Protobuf Include="Grpc\Protos\organizations\v1\permissions.proto" GrpcServices="Server" />

// In Program.cs (or app.MapGrpcService<T>):
app.MapGrpcService<PermissionsGrpcService>();
```

```protobuf
// src/Services/Organizations/Edvantix.Organizations/Grpc/Protos/organizations/v1/permissions.proto
edition = "2023";

option csharp_namespace = "Edvantix.Organizations.Grpc.Services";

package organizations.v1;

service PermissionsGrpcService {
  rpc CheckPermission (CheckPermissionRequest) returns (CheckPermissionReply);
}

message CheckPermissionRequest {
  string user_id = 1;
  string school_id = 2;
  string permission = 3;
}

message CheckPermissionReply {
  bool has_permission = 1;
}
```

### Pattern 2: IHybridCache with tag-based eviction

**What:** `GetOrCreateAsync` stores the result under a key tagged with a user+school tag. `RemoveByTagAsync` evicts all keys for a user in a school atomically.

**When to use:** CheckPermission query handler; cache invalidation in command handlers.

```csharp
// Source: src/BuildingBlocks/Edvantix.Chassis/Caching/IHybridCache.cs (verified)
// In CheckPermissionQueryHandler:
var cacheKey = $"perm:{userId}:{schoolId}:{permission}";
var tag = $"user:{userId}:{schoolId}";

return await _cache.GetOrCreateAsync(
    cacheKey,
    async ct => await _repository.HasPermissionAsync(userId, schoolId, permission, ct),
    tags: [tag],
    cancellationToken: cancellationToken
);

// In RevokeRoleCommandHandler (after SaveEntitiesAsync):
var tag = $"user:{request.ProfileId}:{assignment.SchoolId}";
await _cache.RemoveByTagAsync(tag, cancellationToken);
```

### Pattern 3: Domain event → EventMapper → MassTransit outbox

**What:** Aggregate raises a domain event via `RegisterDomainEvent`. `EventDispatchInterceptor` fires after `SaveChangesAsync`. `EventDispatcher` calls `EventMapper.MapToIntegrationEvent` and publishes via `IBus`. MassTransit outbox delivers to Kafka.

**When to use:** AssignRole, RevokeRole, AssignPermissions mutation commands need to emit `UserPermissionsInvalidated`.

```csharp
// Source: src/Services/Persona/Edvantix.Persona/Infrastructure/EventServices/EventMapper.cs (verified pattern)
// In Organizations EventMapper:
public IntegrationEvent MapToIntegrationEvent(DomainEvent @event)
{
    return @event switch
    {
        UserRoleAssignedEvent e => new UserPermissionsInvalidatedIntegrationEvent(e.ProfileId, e.SchoolId),
        UserRoleRevokedEvent e => new UserPermissionsInvalidatedIntegrationEvent(e.ProfileId, e.SchoolId),
        _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null),
    };
}

// Integration event (in Edvantix.Contracts namespace, sealed record):
public sealed record UserPermissionsInvalidatedIntegrationEvent(
    Guid UserId,
    Guid SchoolId,
    DateTimeOffset Timestamp = default
) : IntegrationEvent;
```

### Pattern 4: Chassis authorization extension (PermissionRequirement)

**What:** `IAuthorizationRequirement` carrying a permission string; `IAuthorizationHandler` resolves a gRPC client from DI and calls `CheckPermission`. A Chassis extension method `AddPermissionAuthorization(grpcBaseUrl)` registers the client and handlers. Downstream services call this on startup.

**When to use:** Any downstream service endpoint guarded by an Organizations permission.

```csharp
// Source: src/BuildingBlocks/Edvantix.Chassis/Security/Extensions/PolicyBuilderExtensions.cs (verified pattern)
// New in Chassis:
public static AuthorizationPolicyBuilder RequirePermission(
    this AuthorizationPolicyBuilder builder,
    string permission
) => builder.AddRequirements(new PermissionRequirement(permission));

public static IServiceCollection AddPermissionAuthorization(
    this IHostApplicationBuilder builder,
    string organizationsGrpcBaseUrl
)
{
    builder.Services.AddGrpcServiceReference<PermissionsGrpcService.PermissionsGrpcServiceClient>(
        organizationsGrpcBaseUrl, HealthStatus.Degraded
    );
    builder.Services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
    return builder.Services;
}
```

### Pattern 5: Wiring HybridCache with Redis in Organizations

**What:** `builder.AddCaching()` (from `CachingExtensions`) registers `IHybridCache`. Redis L2 backend is added by Aspire's `EnrichAzureManagedRedis` / `AddAzureRedisDistributedCache` via the `WithReference(redis)` in AppHost. `HybridCacheOptions` is read from `appsettings.json` section `"Caching"`.

**When to use:** In Organizations `Extensions.cs → AddApplicationServices()`.

```csharp
// Source: src/BuildingBlocks/Edvantix.Chassis/Caching/CachingExtensions.cs (verified)
// In Extensions.cs AddApplicationServices():
builder.AddCaching(options =>
{
    options.DefaultEntryOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(5),           // L2 Redis TTL
        LocalCacheExpiration = TimeSpan.FromSeconds(30), // L1 in-memory TTL
    };
});

// appsettings.json:
// "Caching": { "MaximumPayloadBytes": 1048576, "Expiration": "00:05:00" }
```

### Anti-Patterns to Avoid

- **Domain event on entity without `RegisterDomainEvent`:** `EventDispatchInterceptor` only sees events via `IHasDomainEvents.DomainEvents`. Never publish directly from a command handler using `IBus`; use the interceptor pipeline.
- **Cache invalidation after publish (not before):** Invalidation must happen before or atomically with `SaveEntitiesAsync`. If done after publish, a downstream service could re-populate the cache before invalidation arrives.
- **Separate HybridCache registration without Aspire Redis enrichment:** `AddCaching()` registers the .NET `HybridCache` service, but the L2 Redis distributed cache is wired by Aspire's `EnrichAzureManagedRedis`. Calling `AddCaching` without `WithReference(redis)` in AppHost results in L1-only caching with no error — a silent misconfiguration.
- **`GrpcServices="Both"` in .csproj:** Use `"Server"` for Organizations (serving) and `"Client"` for downstream services (consuming). `"Both"` generates unnecessary code.
- **Integration event in service namespace instead of `Edvantix.Contracts`:** ArchTest `IntegrationEventHandlerTests` asserts integration events reside in `Edvantix.Contracts` namespace. Violating this breaks the architecture gate.

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Cache with tag eviction | Custom dictionary + manual eviction | `IHybridCache.RemoveByTagAsync` | L1+L2 coherence, thread safety, TTL propagation |
| gRPC service registration | Manual `ServerCallContext` wiring | Aspire `AddGrpcServiceReference` | Health degradation, service discovery, resilience |
| Outbox pattern | Custom retry table | `AddEntityFrameworkOutbox<OrganizationsDbContext>` (already configured) | Exactly-once delivery, crash safety |
| Distributed cache | Direct `StackExchange.Redis` calls | `HybridCache` via Aspire Redis enrichment | L1/L2 abstraction, serialisation, TTL management |
| Permission policy per endpoint | Named policy per feature | `RequireAuthorization("permission.string")` with a single dynamic `PermissionRequirementHandler` | One handler resolves all permission strings; no per-feature registration |

**Key insight:** Every infrastructure concern in this phase is already solved in the Chassis or Aspire layer. The only novel code is domain logic (domain events on aggregates) and the thin gRPC service shim.

## Common Pitfalls

### Pitfall 1: Missing `app.MapGrpcService<T>()` call

**What goes wrong:** The gRPC service is registered in DI but never mapped; all incoming gRPC calls return 404/unimplemented.
**Why it happens:** Unlike REST endpoints wired by `MapEndpoints`, gRPC services require explicit `MapGrpcService<T>()` in `Program.cs`.
**How to avoid:** Add `app.MapGrpcService<PermissionsGrpcService>()` in `Program.cs` after `app.MapEndpoints`.
**Warning signs:** gRPC client receives `StatusCode.Unimplemented` or connection refused.

### Pitfall 2: `CachingOptions` validation failure on startup

**What goes wrong:** Service crashes with `OptionsValidationException` if `appsettings.json` is missing the `"Caching"` section.
**Why it happens:** `CachingOptions` uses `[OptionsValidator]` + `[Required]` on `MaximumPayloadBytes` and `Expiration`.
**How to avoid:** Add `"Caching": { "MaximumPayloadBytes": 1048576, "Expiration": "00:05:00" }` to `appsettings.json` and `appsettings.Development.json`.
**Warning signs:** `IValidateOptions<CachingOptions>` throws on first DI resolution.

### Pitfall 3: `HybridCacheOptions` TTL overrides not splitting L1/L2

**What goes wrong:** Both L1 and L2 get the same TTL (e.g. 5 min), so in-memory never evicts within 30 s.
**Why it happens:** `CachingExtensions.AddCaching` sets `Expiration` and `LocalCacheExpiration` to the same value from `CachingOptions.Expiration`. Per-call overrides must be passed as `HybridCacheEntryOptions`.
**How to avoid:** Pass explicit `HybridCacheEntryOptions` in `GetOrCreateAsync` to override `LocalCacheExpiration = 30 s` and `Expiration = 5 min` independently.
**Warning signs:** Cache invalidation takes 5 min instead of 30 s for the L1 layer.

### Pitfall 4: `EventMapper` switch not covering all mutating commands

**What goes wrong:** `AssignPermissions` to a role changes what users can do, but if the mapper only handles `AssignRole`/`RevokeRole` domain events, role-permission changes won't invalidate the cache.
**Why it happens:** A role's permissions changing affects all users assigned that role, but the CONTEXT specifies UserPermissionsInvalidated must also be triggered by `AssignPermissions`.
**How to avoid:** Add a `RolePermissionsChangedEvent` domain event on `Role.SetPermissions` and map it to `UserPermissionsInvalidated`. The handler must resolve all users with that role and emit per-user invalidation (or emit a school-scoped event).
**Warning signs:** After `AssignPermissions`, existing cached permission checks remain stale for the full TTL.

### Pitfall 5: gRPC Kestrel port conflict — HTTP/2 requires separate endpoint

**What goes wrong:** gRPC requires HTTP/2; if Organizations only exposes HTTPS (with HTTP/1.1 fallback), gRPC negotiation fails.
**Why it happens:** ASP.NET Core supports gRPC over HTTP/2; the Kestrel configuration must explicitly allow HTTP/2 or use `UseHttps` with TLS negotiation.
**How to avoid:** In `appsettings.json`, configure a dedicated Kestrel endpoint with `Protocols: "Http2"` for the gRPC port, or rely on Aspire service defaults which configure HTTP/2 automatically for gRPC services via `MapGrpcService`.
**Warning signs:** `RpcException` with `StatusCode.Internal` and "ALPN negotiation failed".

### Pitfall 6: Downstream services receive the consumer event before cache invalidation completes

**What goes wrong:** Organizations publishes `UserPermissionsInvalidated` via outbox, but hasn't completed `RemoveByTagAsync` yet. A downstream service re-primes the cache (via its own gRPC call) and gets stale data.
**How to avoid:** As decided, invalidation via `RemoveByTagAsync` happens in the command handler _before_ the domain event is raised, and the event is raised before `SaveEntitiesAsync`. The outbox ensures at-least-once delivery after the DB commit. This ordering is correct.
**Warning signs:** Race condition under load test — check ordering of cache eviction vs `SaveEntitiesAsync`.

## Code Examples

### gRPC service implementation (Organizations server side)

```csharp
// Source: inverted pattern from PersonaProfileService.cs (verified)
// src/Services/Organizations/Edvantix.Organizations/Grpc/Services/PermissionsGrpcService.cs
using Edvantix.Organizations.Grpc.Services;
using Grpc.Core;

namespace Edvantix.Organizations.Grpc.Services;

/// <summary>
/// gRPC server implementation for permission checks.
/// Delegates to CheckPermissionQuery via IHybridCache-backed handler.
/// </summary>
public sealed class PermissionsGrpcService(IMediator mediator)
    : PermissionsGrpcService.PermissionsGrpcServiceBase
{
    public override async Task<CheckPermissionReply> CheckPermission(
        CheckPermissionRequest request,
        ServerCallContext context
    )
    {
        var hasPermission = await mediator.Send(
            new CheckPermissionQuery(
                Guid.Parse(request.UserId),
                Guid.Parse(request.SchoolId),
                request.Permission
            ),
            context.CancellationToken
        );

        return new CheckPermissionReply { HasPermission = hasPermission };
    }
}
```

### Domain event registration pattern

```csharp
// Source: src/BuildingBlocks/Edvantix.SharedKernel/SeedWork/HasDomainEvents.cs (verified)
// In UserRoleAssignment aggregate:
public void Revoke()
{
    RegisterDomainEvent(new UserRoleRevokedEvent(ProfileId, SchoolId, RoleId));
}
```

### Cache invalidation in command handler (correct ordering)

```csharp
// 1. Find assignment
// 2. Revoke (raises domain event on aggregate)
assignment.Revoke();
// 3. Persist (EventDispatchInterceptor fires domain event → EventMapper → outbox)
assignmentRepository.Remove(assignment);
await assignmentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
// 4. Invalidate local cache immediately (belt-and-suspenders before event reaches L2)
var tag = $"user:{request.ProfileId}:{assignment.SchoolId}";
await _cache.RemoveByTagAsync(tag, cancellationToken);
```

### IConsumer for UserPermissionsInvalidated (downstream side)

```csharp
// Source: CleanUpSentEmailIntegrationEventHandler.cs pattern (verified)
// Namespace convention: Edvantix.{Service}.IntegrationEvents.EventHandlers
public sealed class UserPermissionsInvalidatedHandler(IHybridCache cache)
    : IConsumer<UserPermissionsInvalidatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserPermissionsInvalidatedIntegrationEvent> context)
    {
        var tag = $"user:{context.Message.UserId}:{context.Message.SchoolId}";
        await cache.RemoveByTagAsync(tag, context.CancellationToken);
    }
}
```

## State of the Art

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| `IMemoryCache` + `IDistributedCache` separately | `Microsoft.Extensions.Caching.Hybrid` (two-level unified API) | .NET 9 GA (Nov 2024) | Single API handles L1+L2, stampede protection, tag eviction |
| Manual outbox table | `MassTransit.EntityFrameworkCore` outbox | MassTransit v8+ | EF Core integration, exactly-once, inbox dedup |
| Static policy registration per permission | Dynamic `IAuthorizationHandler` resolving permission from requirement | ASP.NET Core 6+ | No per-endpoint policy registration; single handler scales to N permissions |

**Deprecated/outdated:**
- `IDistributedCache` direct usage: superseded by `HybridCache` in this codebase — Chassis already wraps it.
- `IMemoryCache` alone: no L2 tier, no cross-instance invalidation.

## Open Questions

1. **RolePermissionsChangedEvent → which users to invalidate**
   - What we know: When `AssignPermissions` to a role changes permission set, all users assigned that role are affected. The `UserPermissionsInvalidated` event requires `UserId`.
   - What's unclear: The command handler doesn't currently load all users of a role before persisting. Options: (a) emit a school-scoped invalidation without UserId (requires cache tag to be `school:{schoolId}`), or (b) load all assignments for the role and emit one event per user.
   - Recommendation: Use option (a) — add a `school:{schoolId}` tag to all permission cache entries alongside the user tag, and emit `UserPermissionsInvalidated` with `UserId = Guid.Empty` / use a separate `SchoolPermissionsInvalidatedEvent`. This avoids N events per role change. Decision should be made in the plan phase before implementing AssignPermissions mutation.

2. **L1 TTL override — per-call `HybridCacheEntryOptions`**
   - What we know: `CachingExtensions.AddCaching` sets a single `Expiration` from config for both L1 and L2. The locked decision requires L1 = 30 s, L2 = 5 min.
   - What's unclear: Whether `HybridCacheEntryOptions` overrides are applied per-call or globally. Verified from source: `IHybridCache.GetOrCreateAsync` signature does NOT currently expose `HybridCacheEntryOptions` as a parameter — only `key`, `factory`, `tags`, `cancellationToken`.
   - Recommendation: Either (a) override `HybridCacheOptions.DefaultEntryOptions` with L2 values globally and accept L1 = L2, or (b) extend `IHybridCache` / `HybridCacheService` to accept `HybridCacheEntryOptions`. The existing `HybridCache.GetOrCreateAsync` in Microsoft.Extensions.Caching.Hybrid DOES accept `HybridCacheEntryOptions` — the Chassis wrapper just doesn't expose it yet. Plan should add an overload to `IHybridCache` that accepts options, or configure the defaults as L1=30s, L2=5min via two separate calls at registration.

## Validation Architecture

### Test Framework

| Property | Value |
|----------|-------|
| Framework | TUnit (latest) |
| Config file | Detected from existing `.csproj` files (e.g., `Edvantix.Persona.UnitTests.csproj`) |
| Quick run command | `dotnet test --filter "FullyQualifiedName~Organizations" -x` |
| Full suite command | `dotnet test` |

### Phase Requirements → Test Map

| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|-------------|
| ORG-07 | CheckPermission returns true when user has permission | Unit | `dotnet test --filter "GivenUserHasPermission_WhenCheckPermission" -x` | ❌ Wave 0 |
| ORG-07 | CheckPermission returns false when user lacks permission | Unit | `dotnet test --filter "GivenUserLacksPermission_WhenCheckPermission" -x` | ❌ Wave 0 |
| ORG-07 | gRPC service delegates to mediator query | Unit | `dotnet test --filter "GivenValidRequest_WhenCheckPermissionCalled" -x` | ❌ Wave 0 |
| ORG-08 | Second call within TTL returns cached result without DB hit | Unit | `dotnet test --filter "GivenCachedPermission_WhenCheckedAgain" -x` | ❌ Wave 0 |
| ORG-08 | Cache key and tag format matches spec | Unit | `dotnet test --filter "GivenCacheEntry_WhenStored_ThenKeyMatchesSpec" -x` | ❌ Wave 0 |
| ORG-09 | RevokeRole raises UserRoleRevokedEvent domain event | Unit | `dotnet test --filter "GivenAssignment_WhenRevoked_ThenDomainEventRaised" -x` | ❌ Wave 0 |
| ORG-09 | EventMapper maps UserRoleRevokedEvent to UserPermissionsInvalidated | Unit | `dotnet test --filter "GivenRevokedEvent_WhenMapped_ThenIntegrationEvent" -x` | ❌ Wave 0 |
| ORG-09 | RemoveByTagAsync called after role revocation | Unit | `dotnet test --filter "GivenRoleRevoked_WhenHandled_ThenCacheInvalidated" -x` | ❌ Wave 0 |
| ORG-07, ORG-08 | Architecture: domain events end with "Event" | Arch | `dotnet test --project tests/Edvantix.ArchTests` | ✅ |
| ORG-09 | Architecture: integration events in Edvantix.Contracts, sealed record | Arch | `dotnet test --project tests/Edvantix.ArchTests` | ✅ |

### Sampling Rate

- **Per task commit:** `dotnet test --filter "FullyQualifiedName~Organizations" -x`
- **Per wave merge:** `dotnet test`
- **Phase gate:** Full suite green before `/gsd:verify-work`

### Wave 0 Gaps

- [ ] `src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/Permissions/CheckPermissionQueryHandlerTests.cs` — covers ORG-07, ORG-08
- [ ] `src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/Permissions/PermissionsGrpcServiceTests.cs` — covers ORG-07
- [ ] `src/Services/Organizations/Edvantix.Organizations.UnitTests/Infrastructure/EventMapper/EventMapperTests.cs` — covers ORG-09
- [ ] `src/Services/Organizations/Edvantix.Organizations.UnitTests/Domain/UserRoleAssignmentDomainEventTests.cs` — covers ORG-09
- [ ] `src/Services/Organizations/Edvantix.Organizations.UnitTests/GlobalUsings.cs` — shared test usings

## Sources

### Primary (HIGH confidence)

- `src/BuildingBlocks/Edvantix.Chassis/Caching/IHybridCache.cs` — verified `GetOrCreateAsync` + `RemoveByTagAsync` signatures
- `src/BuildingBlocks/Edvantix.Chassis/Caching/HybridCacheService.cs` — verified wraps `Microsoft.Extensions.Caching.Hybrid.HybridCache`
- `src/BuildingBlocks/Edvantix.Chassis/Caching/CachingExtensions.cs` — verified `AddCaching()` registers `IHybridCache` with `HybridCacheOptions` from config
- `src/BuildingBlocks/Edvantix.Chassis/EventBus/Dispatcher/EventDispatcher.cs` — verified `IBus.Publish` + `IEventMapper` pipeline
- `src/BuildingBlocks/Edvantix.Chassis/EF/EventDispatchInterceptor.cs` — verified post-`SaveChangesAsync` event dispatch
- `src/BuildingBlocks/Edvantix.Chassis/Security/Authorization/ProfileRegisteredRequirementHandler.cs` — verified `IAuthorizationHandler` pattern
- `src/BuildingBlocks/Edvantix.Chassis/Security/Extensions/PolicyBuilderExtensions.cs` — verified `AddProfileRequiredServices` as template
- `src/Services/Organizations/Edvantix.Organizations/Grpc/Services/PersonaProfileService.cs` — verified gRPC client wrapper pattern
- `src/Services/Organizations/Edvantix.Organizations/Edvantix.Organizations.csproj` — verified `GrpcServices="Client"` syntax and existing packages
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/EventServices/EventMapper.cs` — verified stub state
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/OrganizationsDbContext.cs` — verified outbox entities registered
- `src/Services/Organizations/Edvantix.Organizations/Extensions/Extensions.cs` — verified outbox config + event bus wiring
- `src/Aspire/Edvantix.AppHost/AppHost.cs` — verified Redis resource exists; Organizations does NOT yet reference it
- `src/Services/Persona/Edvantix.Persona/Infrastructure/EventServices/EventMapper.cs` — verified full switch pattern
- `src/Services/Notification/Edvantix.Notification/IntegrationEvents/EventHandlers/CleanUpSentEmailIntegrationEventHandler.cs` — verified `IConsumer<T>` handler pattern
- `src/Services/Notification/Edvantix.Notification/IntegrationEvents/Events/CleanUpSentEmailIntegrationEvent.cs` — verified `Edvantix.Contracts` namespace for integration events
- `src/BuildingBlocks/Edvantix.SharedKernel/SeedWork/HasDomainEvents.cs` — verified `RegisterDomainEvent` method
- `tests/Edvantix.ArchTests/Features/IntegrationEventHandlerTests.cs` — verified arch constraints on event namespaces
- `tests/Edvantix.ArchTests/Domain/DomainEventTests.cs` — verified arch constraints on domain event naming

### Secondary (MEDIUM confidence)

- `src/Aspire/Edvantix.AppHost/Extensions/Infrastructure/AzureExtensions.Run.cs` — verified Redis `RunAsLocalContainer` + `WithRedisInsight`

### Tertiary (LOW confidence)

- None — all findings are codebase-verified

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH — all packages already in csproj; Redis in AppHost
- Architecture: HIGH — all patterns lifted directly from existing code
- Pitfalls: HIGH (structural) / MEDIUM (Pitfall 4 role-permission scope) — structural pitfalls verified from code; race condition pitfall is reasoned from the architecture
- Open Questions: MEDIUM — IHybridCache options gap is verified from source; role-scope invalidation requires planner decision

**Research date:** 2026-03-19
**Valid until:** 2026-04-19 (stable framework stack; 30-day validity)
