# Phase 01: Organizations — RBAC Core - Research

**Researched:** 2026-03-18
**Domain:** Multi-tenant RBAC service scaffold, EF Core global query filters, ASP.NET Core middleware, ArchUnitNET architecture tests
**Confidence:** HIGH

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions

**Tenant isolation convention**
- Tenant context transmitted through `X-School-Id` HTTP header per-request
- Users can belong to multiple schools simultaneously — school context per-request, not per-JWT
- `ITenantContext` extracted in ASP.NET Core middleware, registered as scoped DI; DbContext receives it via constructor
- `IMessagePreProcessor<TMessage>` in Mediator pipeline may be used for additional tenant validation before handler
- Architecture test (NetArchTest.Rules → actual project uses ArchUnitNET/TUnit): any `ITenanted` entity in any service must have a registered EF Core global query filter by `schoolId` — otherwise build fails

**Permission string format**
- Format: `service.verb-noun` (kebab-case), e.g. `organizations.create-role`, `scheduling.create-slot`
- Declared in static class constants, e.g. `OrganizationsPermissions.CreateRole = "organizations.create-role"`
- Permission string catalogue stored in the Organizations DB (Permissions table)
- Services register their permissions on startup via upsert call to Organizations API
- Before saving `AssignPermissionsToRole` — validation: permission string must exist in catalogue

**Role management API**
- Soft-delete for roles (`ISoftDelete` pattern exists in SharedKernel), query filter hides deleted roles
- `DELETE /roles/{id}` returns 409 Conflict if role has user assignments — owner must revoke first
- `GET /roles` → `[{id, name}]` — without permissions in list
- `GET /roles/{id}/permissions` — separate endpoint for role permissions

**User identity in assignments**
- `UserRoleAssignment` stores `profileId` from Persona (not Keycloak sub)
- `profile_id` claim already in JWT (Persona adds it via `KeycloakClaimTypes.Profile = "profile_id"`)
- User can have multiple roles in one school (multiple `UserRoleAssignment` records)
- Schema: `UserRoleAssignment(profileId, schoolId, roleId)` — unique constraint on `(profileId, schoolId, roleId)`
- When assigning a role, Organizations validates `profileId` via gRPC to Persona service

### Claude's Discretion
- Table and column naming in EF Core configurations
- Folder structure inside Organizations service (follow Persona pattern)
- Concrete design of `IMessagePreProcessor` for tenant validation
- Error response format (follow existing Chassis patterns)

### Deferred Ideas (OUT OF SCOPE)
- Validating that the user actually belongs to the school when assigning a role — not a Phase 1 requirement; Organizations only stores assignments
- List of school users (member management) — separate feature, not in Phase 1
- Role templates (predefined role sets) — ORG-V2-02, deferred to v2
</user_constraints>

<phase_requirements>
## Phase Requirements

| ID | Description | Research Support |
|----|-------------|-----------------|
| ORG-01 | School owner can create a custom role with a name within their school | Domain model: `Role` aggregate with `Name` property, `CreateRole` command, EF config, tenant-scoped query filter |
| ORG-02 | Owner can view, edit, and delete roles in their school | `GetRoles` / `GetRole` queries, `UpdateRole` command, `DeleteRole` command with 409 guard (ISoftDelete) |
| ORG-03 | Owner can assign a set of permissions to a role (permission = CQRS command identifier string) | `AssignPermissionsToRole` command; `Permission` catalogue entity; validation against catalogue before save |
| ORG-04 | Services automatically register their permission strings in the catalogue on startup | `IHostedService` or `IHostApplicationLifetime` upsert on startup; endpoint that accepts `RegisterPermissions` payload |
| ORG-05 | Owner can assign a role to a user within their school | `UserRoleAssignment` aggregate, `AssignRole` command, gRPC call to Persona for profileId validation |
| ORG-06 | Owner can revoke a role from a user | `RevokeRole` command, hard-delete or deactivation of `UserRoleAssignment` |
| ORG-10 | EF Core global query filter by schoolId on all aggregates (tenant isolation) | `ITenanted` interface, `ITenantContext` scoped service, EF Core `HasQueryFilter` per entity configuration; ArchUnitNET test enforces convention |
</phase_requirements>

## Summary

Phase 1 creates the `Edvantix.Organizations` microservice from scratch, following the existing Persona service pattern exactly. The service introduces three cross-cutting conventions that all future services (Scheduling, Payments) will inherit: (1) `ITenantContext` scoped service populated by ASP.NET Core middleware from the `X-School-Id` header, (2) `ITenanted` entity interface with an EF Core global query filter per entity type, and (3) an ArchUnitNET test that enforces the filter convention at build time across every service assembly.

The RBAC domain is simple but must be designed carefully: `Role` is a soft-delete aggregate per school; `Permission` is a catalogue record upserted by services on startup; `RolePermission` is a join between them; and `UserRoleAssignment` links a `profileId` (from Persona) to a `roleId` within a `schoolId`. The architecture test for tenant isolation is the phase's keystone deliverable — once it exists in `Edvantix.ArchTests`, all subsequent services that introduce new tenanted entities will automatically be guarded.

The codebase already contains all the building blocks needed: `Entity<TId>`, `ISoftDelete`, `IUnitOfWork`, `ValidationBehavior`, `IEndpoint`, feature-folder structure, sealed/internal conventions enforced by existing arch tests, and the gRPC client pattern for cross-service calls. No new libraries need to be added to `Directory.Packages.props` beyond what already exists, with the exception of adding `Grpc.Tools` + proto file for the Persona gRPC client inside the Organizations project.

**Primary recommendation:** Scaffold the Organizations service by cloning the Persona layout, introduce `ITenantContext` as the first task, then layer the domain model, CRUD endpoints, and user-role assignment on top — with the arch test written in the same commit as `ITenanted`.

## Standard Stack

### Core
| Library | Version | Purpose | Why Standard |
|---------|---------|---------|--------------|
| `Mediator.Abstractions` + `Mediator.SourceGenerator` | 3.0.1 | CQRS commands/queries/pipeline | Already used across all services; source-generator avoids reflection |
| `Microsoft.EntityFrameworkCore` (via Aspire Npgsql integration) | 10.0.5 | Data access, global query filters, migrations | Standard in all Edvantix services |
| `Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL` | 13.1.2 | Aspire-managed Postgres DbContext wiring | Used by Persona and Blog; provides health checks + OTLP |
| `FluentValidation.DependencyInjectionExtensions` | 12.1.1 | Request validation in Mediator pipeline | ValidationBehavior already depends on `IValidator<T>` |
| `Grpc.Tools` (PrivateAssets=all) | 2.78.0 | Proto → C# client codegen | Used by Persona.csproj; Organizations needs client stub for Persona |
| `Grpc.AspNetCore.Server.ClientFactory` | 2.76.0 | gRPC client DI registration | Used in Blog Grpc/Extensions.cs pattern |
| `TngTech.ArchUnitNET.TUnit` | 0.13.3 | Architecture tests | Already in `Edvantix.ArchTests`; ITenanted filter test goes here |

### Supporting
| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| `Scrutor` | 7.0.0 | Assembly scanning for `AddRepositories` | Used in `Chassis.Repository.Extensions` — organizations repo registration |
| `EFCore.NamingConventions` | 10.0.1 | snake_case column naming | Used project-wide (verify in `ServiceDefaults`) |
| `Bogus` + `Moq` + `Shouldly` | 35.6.5 / 4.20.72 / 4.3.0 | Unit test data/mocking/assertions | Standard test stack (see Persona unit tests) |

### Alternatives Considered
| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| ArchUnitNET for query filter test | NetArchTest.Rules | CONTEXT.md mentions NetArchTest, but the project already uses ArchUnitNET (TngTech) everywhere — use ArchUnitNET for consistency |
| Per-entity `HasQueryFilter` | Global filter on `DbContext.OnModelCreating` | Per-entity (in `IEntityTypeConfiguration`) matches existing Persona pattern and is required to combine with `ISoftDelete` filter correctly |
| Hard-delete for `UserRoleAssignment` | Soft-delete | Assignments need to be truly removed (CONTEXT decision); only `Role` uses `ISoftDelete` |

**No new packages needed in `Directory.Packages.props`** — all required packages are already defined. Organizations `.csproj` needs only to reference the same packages already defined.

## Architecture Patterns

### Recommended Project Structure
```
src/Services/Organizations/
├── Edvantix.Organizations/
│   ├── Configurations/
│   │   └── OrganizationsAppSettings.cs
│   ├── Domain/
│   │   ├── AggregatesModel/
│   │   │   ├── RoleAggregate/
│   │   │   │   ├── Role.cs                    # IAggregateRoot, ISoftDelete, ITenanted
│   │   │   │   ├── RolePermission.cs          # Entity owned by Role
│   │   │   │   └── IRoleRepository.cs
│   │   │   ├── PermissionAggregate/
│   │   │   │   ├── Permission.cs              # Catalogue entry, ITenanted = false (global)
│   │   │   │   └── IPermissionRepository.cs
│   │   │   └── UserRoleAssignmentAggregate/
│   │   │       ├── UserRoleAssignment.cs      # IAggregateRoot, ITenanted
│   │   │       └── IUserRoleAssignmentRepository.cs
│   │   └── Abstractions/
│   │       └── ITenanted.cs                   # New cross-cutting interface
│   ├── Features/
│   │   ├── Roles/
│   │   │   ├── CreateRole/
│   │   │   ├── GetRoles/
│   │   │   ├── GetRoleById/
│   │   │   ├── UpdateRole/
│   │   │   ├── DeleteRole/
│   │   │   └── AssignPermissions/
│   │   ├── Permissions/
│   │   │   └── RegisterPermissions/           # startup upsert endpoint
│   │   └── UserRoleAssignments/
│   │       ├── AssignRole/
│   │       └── RevokeRole/
│   ├── Grpc/
│   │   ├── Extensions.cs                      # Persona gRPC client registration
│   │   ├── Protos/persona/v1/profile.proto    # copy from Persona (client-side)
│   │   └── Services/
│   │       ├── IPersonaProfileService.cs
│   │       └── PersonaProfileService.cs
│   ├── Infrastructure/
│   │   ├── EntityConfigurations/
│   │   │   ├── RoleConfiguration.cs           # internal sealed, HasQueryFilter(schoolId + isDeleted)
│   │   │   ├── RolePermissionConfiguration.cs
│   │   │   ├── PermissionConfiguration.cs
│   │   │   └── UserRoleAssignmentConfiguration.cs
│   │   ├── Repositories/
│   │   │   ├── RoleRepository.cs              # sealed
│   │   │   ├── PermissionRepository.cs
│   │   │   └── UserRoleAssignmentRepository.cs
│   │   ├── Migrations/
│   │   ├── OrganizationsDbContext.cs          # sealed
│   │   ├── OrganizationsDbContextFactory.cs
│   │   └── Extensions.cs                      # internal static AddPersistenceServices
│   ├── Middleware/
│   │   └── TenantMiddleware.cs                # extracts X-School-Id → ITenantContext
│   ├── Extensions/
│   │   └── Extensions.cs                      # internal static AddApplicationServices
│   ├── GlobalUsings.cs
│   ├── IOrganizationsApiMarker.cs
│   └── Program.cs
└── Edvantix.Organizations.UnitTests/
    ├── Domain/
    ├── Features/
    └── GlobalUsings.cs
```

### Pattern 1: ITenantContext via Middleware → Scoped DI
**What:** A middleware reads `X-School-Id` header, resolves a scoped `ITenantContext` from DI, and sets `SchoolId`. The DbContext constructor receives `ITenantContext` and passes `SchoolId` into global query filters.
**When to use:** Every request to Organizations service; the same pattern will be copied to Scheduling and Payments.

```csharp
// ITenantContext definition (goes in Chassis or Organizations — recommend Chassis so Scheduling reuses)
public interface ITenantContext
{
    Guid SchoolId { get; }
    bool IsResolved { get; }
    void Resolve(Guid schoolId);
}

// TenantMiddleware
public sealed class TenantMiddleware(RequestDelegate next)
{
    private const string Header = "X-School-Id";

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (context.Request.Headers.TryGetValue(Header, out var value)
            && Guid.TryParse(value, out var schoolId))
        {
            tenantContext.Resolve(schoolId);
        }
        await next(context);
    }
}
```

### Pattern 2: ITenanted + EF Core HasQueryFilter per entity
**What:** All tenanted aggregates implement `ITenanted { Guid SchoolId { get; } }`. Each `IEntityTypeConfiguration<T>` calls `HasQueryFilter(x => x.SchoolId == tenantContext.SchoolId)`. When combined with `ISoftDelete`, two conditions are AND-combined in a single filter expression.
**When to use:** Every new aggregate that stores per-school data.

```csharp
// In RoleConfiguration.cs
internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    private readonly ITenantContext _tenantContext;

    public RoleConfiguration(ITenantContext tenantContext) =>
        _tenantContext = tenantContext;

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ConfigureSoftDeletable<Role>();   // sets HasQueryFilter(!IsDeleted)
        builder.HasQueryFilter(r =>
            r.SchoolId == _tenantContext.SchoolId && !r.IsDeleted);

        builder.Property(r => r.Name).IsRequired().HasMaxLength(150);
        builder.Property(r => r.SchoolId).IsRequired();
        builder.HasIndex(r => new { r.SchoolId, r.Name }).IsUnique();
    }
}
```

**IMPORTANT:** `ConfigureSoftDeletable` sets its own `HasQueryFilter`. EF Core only supports one `HasQueryFilter` per entity — the second call overwrites the first. The Organizations service must set a **combined** filter expression (`schoolId AND !isDeleted`) in a single `HasQueryFilter` call, NOT by calling `ConfigureSoftDeletable` and then adding another filter. The `ConfigureSoftDeletable` extension only sets the soft-delete filter on its own, which is fine for Persona (no tenant). For Organizations tenanted entities, write the combined filter directly without calling `ConfigureSoftDeletable`.

### Pattern 3: DbContext receives ITenantContext via constructor
**What:** `OrganizationsDbContext` receives `ITenantContext` as a constructor parameter and passes it to entity configurations via `OnModelCreating`.

```csharp
// Source: Persona PersonaDbContext pattern + tenant extension
public sealed class OrganizationsDbContext(
    DbContextOptions<OrganizationsDbContext> options,
    ITenantContext tenantContext
) : DbContext(options), IUnitOfWork
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();   // keep for Phase 2 outbox readiness
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganizationsDbContext).Assembly);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}
```

Because `ApplyConfigurationsFromAssembly` resolves configurations from the assembly, the configurations need access to `ITenantContext`. **Pattern:** inject `ITenantContext` directly into the configurations using the `IEntityTypeConfiguration<T>` constructor — EF Core does not support constructor injection for configurations resolved via `ApplyConfigurationsFromAssembly`. Instead, override `OnModelCreating` to manually `new` each configuration with `tenantContext`, OR store `tenantContext` as a field on `OrganizationsDbContext` and access it inside an anonymous lambda `HasQueryFilter`.

**Recommended approach:** Store `ITenantContext` as a private field on `DbContext` and call `HasQueryFilter` directly inside `OnModelCreating` after `ApplyConfigurationsFromAssembly`, OR write configurations that accept a delegate. The simplest approach that follows existing patterns:

```csharp
// In OnModelCreating, after ApplyConfigurationsFromAssembly:
modelBuilder.Entity<Role>().HasQueryFilter(r =>
    r.SchoolId == tenantContext.SchoolId && !r.IsDeleted);
modelBuilder.Entity<UserRoleAssignment>().HasQueryFilter(a =>
    a.SchoolId == tenantContext.SchoolId);
```

`Permission` is NOT tenanted — it is a global catalogue. No `HasQueryFilter` needed.

### Pattern 4: Permission catalogue upsert on startup
**What:** Each service has a static `Permissions` class with string constants. On startup, an `IHostedService` (or `IHostApplicationLifetime.ApplicationStarted` callback) calls `POST /v1/permissions/register` on the Organizations service with its permission list. Organizations upserts — inserts new strings, ignores existing ones.
**When to use:** Organizations itself, Scheduling (Phase 3), Payments (Phase 5).

```csharp
// OrganizationsPermissions.cs in Edvantix.Constants
public static class OrganizationsPermissions
{
    public const string CreateRole     = "organizations.create-role";
    public const string UpdateRole     = "organizations.update-role";
    public const string DeleteRole     = "organizations.delete-role";
    public const string AssignRole     = "organizations.assign-role";
    public const string RevokeRole     = "organizations.revoke-role";
    public const string AssignPermissions = "organizations.assign-permissions";
}
```

### Pattern 5: ArchUnitNET test for tenant isolation
**What:** A test in `Edvantix.ArchTests` loads the Organizations assembly and asserts that every class implementing `ITenanted` has a registered `HasQueryFilter` for `SchoolId`. Because ArchUnitNET works at the IL level, the test inspects that no `ITenanted` entity exists in the loaded assemblies without a corresponding EF Core configuration that calls `HasQueryFilter`.

**Approach limitation:** ArchUnitNET cannot introspect EF Core model metadata at test time without running `DbContext`. The practical approach used by this project is a runtime convention check: an integration test (or a `ModelCreating` test) that instantiates `OrganizationsDbContext` with an in-memory configuration and asserts that every `IEntityType` whose CLR type implements `ITenanted` has `GetQueryFilter() != null`. This test lives in `Edvantix.ArchTests` as a convention test. It does NOT use ArchUnitNET IL scanning for this specific check; it uses the EF Core model API.

```csharp
// In tests/Edvantix.ArchTests/Domain/TenantIsolationTests.cs
[Test]
public void GivenTenantedEntities_WhenCheckingEfCoreModel_ThenShouldHaveQueryFilter()
{
    // Build OrganizationsDbContext with a stub ITenantContext
    // Assert every IEntityType where ClrType implements ITenanted has GetQueryFilter() != null
}
```

### Pattern 6: Gated DELETE /roles/{id}
**What:** The `DeleteRole` command handler checks `UserRoleAssignment` for active assignments before soft-deleting the role. If any assignments exist, throws `InvalidOperationException` (which `GlobalExceptionHandler` maps to 409 Conflict).

```csharp
// GlobalExceptionHandler already maps InvalidOperationException → 409
InvalidOperationException or NotSupportedException => (StatusCodes.Status409Conflict, exception.Message)
```

### Pattern 7: Persona gRPC client in Organizations
**What:** Organizations calls Persona's `GetProfile` gRPC to validate that a `profileId` exists before creating a `UserRoleAssignment`. Follow the Blog → Persona pattern exactly.
- Copy `profile.proto` to `Organizations/Grpc/Protos/persona/v1/profile.proto` with `GrpcService="Client"`
- Register: `services.AddGrpcServiceReference<ProfileGrpcService.ProfileGrpcServiceClient>(url, HealthStatus.Degraded)`
- Wrap in `IPersonaProfileService` / `PersonaProfileService` (singleton)
- In `AssignRole` command handler, call `personaProfileService.ValidateProfileExistsAsync(profileId)`

### Anti-Patterns to Avoid
- **Two HasQueryFilter calls on same entity:** EF Core silently replaces the first with the second. Always write the combined `schoolId AND !isDeleted` expression in one call.
- **Calling `ConfigureSoftDeletable` on tenanted entities:** The extension only sets `!IsDeleted` filter, which would drop the schoolId filter. For tenanted soft-delete entities, write the combined filter manually.
- **Storing SchoolId in JWT:** The architecture decision explicitly uses per-request header. Do not attempt to read `schoolId` from JWT claims.
- **Non-sealed DbContext / Configuration classes:** Arch tests already enforce sealed — will fail build if violated.
- **Public EntityConfigurations:** Arch tests enforce `internal` — will fail build.
- **Static permissions catalogue bypass:** All permission strings used in `AssignPermissionsToRole` must exist in the `Permission` table. Do not assign hardcoded strings without first seeding through the registration endpoint.
- **Keycloak sub as user identity:** `UserRoleAssignment` must use `profileId` (from `profile_id` claim / `KeycloakClaimTypes.Profile`), not `sub`.

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| EF Core default config (Id + UUIDv7) | Custom base config | `UseDefaultConfiguration()` from Chassis EF | Already handles `HasKey(Id)` + `HasDefaultValueSql(NewUuidV7)` |
| Request validation pipeline | Custom middleware | `ValidationBehavior<TMessage,TResponse>` via `ApplyValidationBehavior()` | Already registered; any `IValidator<T>` is auto-discovered |
| Repository auto-registration | `services.AddScoped<>` per repo | `services.AddRepositories(typeof(IOrganizationsApiMarker))` | Scrutor scan already built into `Chassis.Repository.Extensions` |
| Problem Details error responses | Custom error serializer | `services.AddExceptionHandler<*>()` + `services.AddProblemDetails()` | GlobalExceptionHandler + ValidationExceptionHandler + NotFoundExceptionHandler cover all cases |
| gRPC exception handling | Custom interceptor | `GrpcExceptionInterceptor` in Chassis | Already registered in Blog gRPC setup |
| Pipeline behaviors | Custom middleware pipeline | `ApplyActivityBehavior().ApplyLoggingBehavior().ApplyValidationBehavior()` | All behaviors exist in Chassis |
| Endpoint discovery | Manual `app.MapPost(...)` in Program.cs | `services.AddEndpoints(typeof(IOrganizationsApiMarker))` + `app.MapEndpoints(apiVersionSet)` | Used by Persona/Blog |

**Key insight:** Do not re-invent infrastructure. Every cross-cutting concern (validation, logging, tracing, error handling, versioning, OpenAPI) already exists in Chassis and ServiceDefaults. The Organizations service should be nearly identical in its `Extensions.cs` boilerplate to Persona and Blog.

## Common Pitfalls

### Pitfall 1: EF Core HasQueryFilter overwrite
**What goes wrong:** `ConfigureSoftDeletable<Role>()` sets `HasQueryFilter(x => !x.IsDeleted)`. If the developer also calls `HasQueryFilter(r => r.SchoolId == ...)` for tenant isolation, the second call silently replaces the first. The result: either soft-deleted records appear, or tenant isolation is missing.
**Why it happens:** EF Core only supports one `HasQueryFilter` per entity type. Each call overwrites the previous.
**How to avoid:** For tenanted soft-delete entities (`Role`, `UserRoleAssignment`), write ONE combined lambda: `HasQueryFilter(r => r.SchoolId == _tenantContext.SchoolId && !r.IsDeleted)`. Do NOT call `ConfigureSoftDeletable` on these entities.
**Warning signs:** Soft-deleted roles appearing in `GET /roles`; roles from other schools appearing in queries.

### Pitfall 2: ITenantContext not resolved before DbContext is used
**What goes wrong:** `ITenantContext.SchoolId` returns `Guid.Empty` or throws when `IsResolved == false`. Since `HasQueryFilter` closes over the `ITenantContext` instance (not its value at model-creation time), this is a runtime issue: queries return no results or wrong results if middleware hasn't set the context.
**Why it happens:** Middleware registration order; or gRPC calls / background services that don't go through the middleware pipeline.
**How to avoid:** Middleware must be registered before any endpoint execution. Guard `ITenantContext.SchoolId` access — throw `InvalidOperationException("Tenant context not resolved")` if called when `IsResolved == false`. For the gRPC server endpoint (if any in Phase 2), tenant context must be set separately.
**Warning signs:** All `GetRoles` queries return empty lists; `UserRoleAssignment` queries hit all-school data.

### Pitfall 3: Architecture test assembly not updated
**What goes wrong:** The new `Edvantix.Organizations` assembly is never added to `ArchUnitBaseTest.Architecture` or `BaseTest`, so the tenant-isolation convention test only covers existing services.
**Why it happens:** `ArchUnitBaseTest` manually lists all service assemblies; new services must be added.
**How to avoid:** Add `OrganizationsAssembly = typeof(IOrganizationsApiMarker).Assembly` to `BaseTest.cs` and include it in the `ArchLoader.LoadAssemblies(...)` call in `ArchUnitBaseTest`.
**Warning signs:** Build passes but Organizations entities have no filters — test was never checking them.

### Pitfall 4: Permission validation race condition on startup
**What goes wrong:** The Organizations service itself tries to validate `organizations.*` permission strings that it registers on startup — but if it validates before its own startup registration completes, validation fails.
**Why it happens:** Self-registration timing — Organizations is both the registrar and the first consumer.
**How to avoid:** Organizations service seeds its own permission strings during DB migration (via `HasData` or a migration seed step), not via the HTTP endpoint. Only other services (Scheduling, Payments) use the HTTP registration endpoint.

### Pitfall 5: HasQueryFilter on Permission catalogue causes cross-service registration failure
**What goes wrong:** If `Permission` entity is accidentally given a `SchoolId` tenant filter, then when other services call `POST /permissions/register`, their permission strings get filtered out (they have no schoolId) and registrations silently fail.
**Why it happens:** Copy-paste from tenanted entity configurations.
**How to avoid:** `Permission` is a global catalogue — no `SchoolId`, no `HasQueryFilter`. It must NOT implement `ITenanted`.

### Pitfall 6: Aspire app host requires restart when new service added
**What goes wrong:** Adding `Edvantix.Organizations` to `AppHost.cs` and running without restarting — the old Aspire host doesn't see the new resource.
**Why it happens:** `AppHost.cs` changes require restart per `CLAUDE.md`.
**How to avoid:** After modifying `AppHost.cs`, run `aspire run` fresh.

## Code Examples

### Combined tenant + soft-delete query filter

```csharp
// Source: Persona ProfileConfiguration.cs pattern + combined filter requirement
internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Do NOT call ConfigureSoftDeletable — it would set a filter that gets overwritten
        builder.UseDefaultConfiguration();   // sets key + UUIDv7 default

        builder.Property(r => r.IsDeleted).HasComment("Soft-delete flag");
        builder.Property(r => r.SchoolId).IsRequired();
        builder.Property(r => r.Name).IsRequired().HasMaxLength(150);

        // Single combined filter — EF Core only allows one HasQueryFilter per entity
        builder.HasQueryFilter(r => r.SchoolId == EF.Property<Guid>(r, "_schoolId")
            && !r.IsDeleted);
        // OR: capture ITenantContext via DbContext field approach (see Architecture section)

        builder.HasIndex(r => new { r.SchoolId, r.Name }).IsUnique()
            .HasFilter("is_deleted = false");  // partial unique index (active roles only)
    }
}
```

### Mediator command pattern (follows existing codebase)

```csharp
// Source: Persona RegistrationCommand.cs pattern
public sealed class CreateRoleCommand : ICommand<Guid>
{
    public required string Name { get; init; }
}

public sealed class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    ITenantContext tenantContext
) : ICommandHandler<CreateRoleCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var role = new Role(request.Name, tenantContext.SchoolId);
        await roleRepository.AddAsync(role, cancellationToken);
        await roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return role.Id;
    }
}
```

### Endpoint pattern (follows existing codebase)

```csharp
// Source: Persona RegistrationEndpoint.cs pattern
public sealed class CreateRoleEndpoint
    : IEndpoint<Created<Guid>, CreateRoleCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/roles",
                async ([FromBody] CreateRoleCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken) =>
                    await HandleAsync(command, sender, linker, cancellationToken))
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("CreateRole")
            .WithTags("Roles")
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateRoleCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default)
    {
        var id = await sender.Send(command, cancellationToken);
        var location = linker.GetPathByName("GetRoleById", new { id }) ?? $"/api/roles/{id}";
        return TypedResults.Created(location, id);
    }
}
```

### Aspire AppHost addition

```csharp
// Source: AppHost.cs existing pattern
var organizationsDb = postgres.AddDatabase(Components.Database.Organizations);
// Add to Components.Database: public static readonly string Organizations = $"{nameof(Organizations).ToLowerInvariant()}{Suffix}";

var organizationsApi = builder
    .AddProject<Edvantix_Organizations>(Services.Organizations)
    .WithReference(organizationsDb)
    .WaitFor(organizationsDb)
    .WithKeycloak(keycloak)
    .WaitFor(keycloak)
    .WithReference(personaApi)   // gRPC client dependency
    .WaitFor(personaApi)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls();

// Add to gateway
gateway.WithService(organizationsApi, true);
```

### Architecture test for tenant isolation (EF Core model check)

```csharp
// Source: existing SoftDeleteTests.cs pattern + EF Core model API
// File: tests/Edvantix.ArchTests/Domain/TenantIsolationTests.cs
public sealed class TenantIsolationTests
{
    [Test]
    public void GivenTenantedEntities_WhenCheckingDbContextModel_ThenAllShouldHaveSchoolIdQueryFilter()
    {
        // Build OrganizationsDbContext with test options + stub ITenantContext
        // Enumerate model.GetEntityTypes()
        // For each type where ClrType implements ITenanted:
        //   Assert entityType.GetQueryFilter() is not null
        //   Assert query filter expression contains "SchoolId"
    }
}
```

## State of the Art

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| Per-HTTP-client `tenantId` in JWT sub | `X-School-Id` header per-request | Phase 1 design decision | Users can belong to multiple schools without re-issuing JWT |
| `NetArchTest.Rules` (mentioned in CONTEXT) | `TngTech.ArchUnitNET.TUnit` 0.13.3 | Already in project | Use ArchUnitNET for new arch tests; no NetArchTest.Rules package exists in Directory.Packages.props |
| `PostgreSQL Row-Level Security` | EF Core global query filters | Out of scope decision | RLS incompatible with PgBouncer transaction mode; EF Core filters used instead |
| MediatR | `Mediator` (source-generator, v3.0.1) | Already established | Source-generator produces zero-reflection dispatch code |

**Deprecated/outdated:**
- `NetArchTest.Rules`: mentioned in CONTEXT.md but not in `Directory.Packages.props`. Use `TngTech.ArchUnitNET.TUnit` (already present). The tenant isolation test for EF Core query filters should use the EF Core model API directly rather than IL scanning.

## Open Questions

1. **Where does `ITenantContext` interface live?**
   - What we know: It's new — doesn't exist yet. Chassis is the natural home (reused by Scheduling and Payments).
   - What's unclear: Does Chassis need a breaking change, or is it easier to define `ITenantContext` in Organizations and copy it to Scheduling later?
   - Recommendation: Define `ITenantContext` in `Edvantix.Chassis` from the start, alongside a `TenantContext` default implementation. This avoids duplication in Phase 3.

2. **How to inject `ITenantContext` into EF Core configurations when using `ApplyConfigurationsFromAssembly`?**
   - What we know: `ApplyConfigurationsFromAssembly` uses parameterless constructor by default; it won't inject `ITenantContext`.
   - What's unclear: Whether to override `OnModelCreating` to pass context into each configuration manually, or to use a closure on a DbContext field.
   - Recommendation: Keep it simple — declare `_tenantContext` as a `private readonly ITenantContext` field on `OrganizationsDbContext`, do NOT use `ApplyConfigurationsFromAssembly` for tenanted entities, and instead call `modelBuilder.ApplyConfiguration(new RoleConfiguration(_tenantContext))` etc. explicitly. Non-tenanted entities (Permission) can still use assembly-scan.

3. **Does `Permission.SchoolId` exist or is the catalogue truly global?**
   - What we know: CONTEXT.md says "Permissions table" stores catalogue strings; services upsert on startup. No per-school filter.
   - What's unclear: Whether future phases will need per-school permission namespacing.
   - Recommendation: Treat as global (no `SchoolId`) for Phase 1. The unique constraint is on `(Name)` only.

4. **Startup permission registration — HTTP call vs DB seed**
   - What we know: CONTEXT.md says "services register their permissions on startup via upsert call to Organizations API".
   - What's unclear: How Organizations itself registers its own permissions (it can't call itself).
   - Recommendation: Organizations seeds its own permissions in the EF Core migration (use `HasData` or a migration-time seed). Other services use `IHostedService` + `HttpClient` typed client to call the registration endpoint.

## Validation Architecture

### Test Framework
| Property | Value |
|----------|-------|
| Framework | TUnit 1.19.74 + TngTech.ArchUnitNET.TUnit 0.13.3 |
| Config file | `global.json` → `"test": { "runner": "Microsoft.Testing.Platform" }` |
| Quick run command | `dotnet test tests/Edvantix.Organizations.UnitTests/ -x` |
| Full suite command | `dotnet test --filter "FullyQualifiedName~Organizations"` |

### Phase Requirements → Test Map
| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|-------------|
| ORG-01 | `CreateRole` command creates role with name and schoolId | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "CreateRole"` | ❌ Wave 0 |
| ORG-02 | `UpdateRole` changes name; `DeleteRole` soft-deletes; 409 when assignments exist | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "Role"` | ❌ Wave 0 |
| ORG-03 | `AssignPermissionsToRole` validates strings against catalogue before persisting | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "AssignPermissions"` | ❌ Wave 0 |
| ORG-04 | Permission registration upserts strings and is idempotent | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "RegisterPermissions"` | ❌ Wave 0 |
| ORG-05 | `AssignRole` validates profileId via gRPC mock, creates `UserRoleAssignment` | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "AssignRole"` | ❌ Wave 0 |
| ORG-06 | `RevokeRole` removes `UserRoleAssignment` | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "RevokeRole"` | ❌ Wave 0 |
| ORG-10 | Every `ITenanted` entity in Organizations (and future services) has `HasQueryFilter` | arch | `dotnet test tests/Edvantix.ArchTests/` | ❌ Wave 0 |

### Sampling Rate
- **Per task commit:** `dotnet test tests/Edvantix.Organizations.UnitTests/ -x` (fast unit tests only)
- **Per wave merge:** `dotnet test` (full suite including arch tests)
- **Phase gate:** Full suite green before `/gsd:verify-work`

### Wave 0 Gaps
- [ ] `tests/Edvantix.Organizations.UnitTests/` — new test project, mirrors `Edvantix.Persona.UnitTests` structure
- [ ] `tests/Edvantix.Organizations.UnitTests/GlobalUsings.cs` — standard global usings (Moq, Bogus, Shouldly)
- [ ] `tests/Edvantix.ArchTests/Domain/TenantIsolationTests.cs` — EF Core model query filter test (covers ORG-10)
- [ ] `tests/Edvantix.ArchTests/Abstractions/BaseTest.cs` — add `OrganizationsAssembly` reference
- [ ] Framework install: already present — no new packages needed

## Sources

### Primary (HIGH confidence)
- Direct code inspection of `E:/projects/Edvantix/src/BuildingBlocks/Edvantix.Chassis/EF/Configurations/EntityTypeConfigurationExtensions.cs` — confirmed `ConfigureSoftDeletable` sets a standalone `HasQueryFilter`; tenanted entities need combined filter
- Direct code inspection of `E:/projects/Edvantix/src/Services/Persona/Edvantix.Persona/Infrastructure/PersonaDbContext.cs` — confirmed DbContext pattern, outbox entities, `ApplyConfigurationsFromAssembly`
- Direct code inspection of `E:/projects/Edvantix/src/Services/Blog/Edvantix.Blog/Grpc/Extensions.cs` and `ProfileService.cs` — confirmed gRPC client registration pattern via `AddGrpcServiceReference`
- Direct code inspection of `E:/projects/Edvantix/tests/Edvantix.ArchTests/` — confirmed ArchUnitNET (not NetArchTest.Rules) is the test library; confirmed assembly registration pattern
- `E:/projects/Edvantix/Directory.Packages.props` + `Versions.props` — confirmed package versions
- `E:/projects/Edvantix/src/BuildingBlocks/Edvantix.Chassis/Exceptions/GlobalExceptionHandler.cs` — confirmed `InvalidOperationException` → 409 Conflict mapping

### Secondary (MEDIUM confidence)
- CONTEXT.md locked decisions — user-confirmed architectural choices

### Tertiary (LOW confidence)
- EF Core `HasQueryFilter` single-per-entity limitation — well-documented behavior but not verified against EF 10.0 release notes; however, this behavior has been stable since EF Core 2.x

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH — all packages confirmed in `Directory.Packages.props`
- Architecture: HIGH — all patterns directly observed in existing service code
- Pitfalls: HIGH — combined HasQueryFilter limitation is a well-known EF Core constraint, directly relevant to this service design

**Research date:** 2026-03-18
**Valid until:** 2026-04-18 (stable stack; EF Core / Aspire versions pinned in Versions.props)
