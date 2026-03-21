# Phase 03: Scheduling — Slots and Views - Research

**Researched:** 2026-03-21
**Domain:** New microservice bootstrap, DDD aggregates (Group, GroupMembership, LessonSlot), teacher conflict detection, permission-filtered schedule query
**Confidence:** HIGH — all findings derived directly from the project codebase; no external library exploration needed

## Summary

Phase 03 creates the Scheduling service from scratch, strictly mirroring the Organizations service structure. The codebase already contains every pattern needed: the `ITenanted` + `ISoftDelete` aggregate base, the combined `HasQueryFilter` convention, the feature-folder CQRS layout, `PermissionSeeder` as `IHostedService`, and the `AddPermissionAuthorization` / `RequirePermission` extension pair that downstream services call to enforce Organizations gRPC checks.

The two non-trivial technical problems are: (1) teacher conflict detection must use `IgnoreQueryFilters()` to bypass the tenant filter — the same technique already used for cross-tenant role enumeration in Phase 02-02; and (2) the single `GET /schedule` endpoint must return different fields depending on which permission the caller holds, resolved inside the query handler by calling Organizations gRPC CheckPermission multiple times (one per candidate role) or by a single check for the most-privileged permission first.

The architecture test suite currently only scans six assemblies. The new `Edvantix.Scheduling` assembly must be added to `BaseTest.cs` and `ArchUnitBaseTest.cs`, and a parallel `TenantIsolationTests` class must be created for `SchedulingDbContext` — the existing test covers only `OrganizationsDbContext`. Both tasks belong in Plan 01 alongside the domain model.

**Primary recommendation:** Copy the Organizations service directory structure verbatim, replace every `Organizations` / `Role` name with `Scheduling` / `Group` or `LessonSlot`, then follow the exact order: domain model first → EF configuration + migrations → feature handlers → endpoints.

---

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions

**Group aggregate**
- D-01: Group properties: `Name` (string), `SchoolId` (ITenanted), `MaxCapacity` (int), `Color` (string — value from a UI-defined set; backend stores the identifier as-is, no validation against allowed values)
- D-02: Soft delete (like `Role`) — `ISoftDelete` interface with combined tenant + soft-delete `HasQueryFilter`
- D-03: Groups can exist without lesson slots (pure student container); no constraint enforcing at least one slot

**Teacher conflict detection**
- D-04: Conflict check is **global** — a teacher cannot be double-booked across different schools at the same time
- D-05: Conflict = exact time overlap (`slot.StartTime < existing.EndTime && slot.EndTime > existing.StartTime`); no buffer required
- D-06: When editing a slot (reschedule or teacher swap), the current slot is **automatically excluded** from the conflict query — the command handler filters out `slotId == currentSlotId`

**Calendar view endpoint**
- D-07: Single `GET /schedule` endpoint with `dateFrom` / `dateTo` query parameters; the UI calendar controls the range
- D-08: Access is permission-based, not role-based: the same endpoint returns different data depending on which `scheduling.*` permission the caller holds (checked via Organizations gRPC `CheckPermission`)
- D-09: Returned fields per slot (Claude's discretion per best practices):
  - **All callers:** `id`, `startTime`, `endTime`, `groupId`, `groupName`, `groupColor`
  - **Manager** (`scheduling.view-schedule` with manager permission): + `teacherId`, `teacherName`, `studentCount`
  - **Teacher** (`scheduling.view-schedule`): + `studentCount`
  - **Student** (`scheduling.view-schedule`): + `teacherId`, `teacherName`

**Service bootstrap**
- D-10: New `Edvantix.Scheduling` service created from scratch following Organizations service structure (feature folders, EF Core, DDD aggregates, `ITenanted`, permission seeder)
- D-11: `Services.Scheduling` and `Components.Database.Scheduling` constants added to `Edvantix.Constants.Aspire`
- D-12: `SchedulingPermissions` class added to `Edvantix.Constants.Permissions` following `OrganizationsPermissions` pattern; permissions registered: `scheduling.create-group`, `scheduling.update-group`, `scheduling.delete-group`, `scheduling.create-lesson-slot`, `scheduling.edit-lesson-slot`, `scheduling.delete-lesson-slot`, `scheduling.view-schedule`, `scheduling.manage-group-membership`
- D-13: Service added to `AppHost.cs` with: `schedulingDb` (same PostgreSQL instance, separate database), Keycloak, Kafka queue, reference to `personaApi` (gRPC profile validation), reference to `organizationsApi` (gRPC CheckPermission); added to Scalar OpenAPI dashboard
- D-14: Scheduling Keycloak client (`${CLIENT_SCHEDULING_ID}`) added to `EdvantixRealms.json` following the same pattern as Persona/Blog/Notification clients (clientType `API`)

### Claude's Discretion
- Exact EF Core index strategy for conflict detection query (partial index on teacher + time range vs. application-level query)
- Whether `GroupMembership` is a separate aggregate or a value object/entity within `Group`
- Permission seeder startup pattern (same `IHostedService` approach as Organizations)
- Gateway routing entry for Scheduling

### Deferred Ideas (OUT OF SCOPE)
- Organizations missing from Keycloak realm — Organizations service client (`${CLIENT_ORGANIZATIONS_ID}`) is absent from `EdvantixRealms.json`; captured as a separate backlog item, not part of Phase 3
- Recurring/repeating lesson slots — v2 feature; Phase 3 handles one-off slots only
- Group templates or copying — not needed in v1
- Teacher availability configuration — explicit availability windows are out of scope; conflict detection is purely slot-based
- Student capacity enforcement — `MaxCapacity` is stored but enforcing it on `AddStudentToGroup` is deferred (log a warning vs. hard reject TBD in Phase 4 context)
</user_constraints>

---

<phase_requirements>
## Phase Requirements

| ID | Description | Research Support |
|----|-------------|------------------|
| SCH-01 | Manager can create a lesson slot (group + date/time + teacher) with teacher conflict check | LessonSlot aggregate; cross-tenant conflict query via `IgnoreQueryFilters()`; CreateLessonSlotCommand handler |
| SCH-02 | Manager can edit and delete lesson slots | EditLessonSlotCommand excludes current slot from conflict check (D-06); DeleteLessonSlotCommand soft-delete or hard delete per decision |
| SCH-03 | Manager sees schedule of all groups in their school | `GET /schedule` query — no data filtering beyond tenant; returns all slots in date range |
| SCH-04 | Teacher sees only their own slots | `GET /schedule` — data filtered to `teacherId == callerId` |
| SCH-05 | Student sees only slots for groups they belong to | `GET /schedule` — data filtered to groups where student is a `GroupMembership` member |
| SCH-06 | Manager can add a student to a group | `AddStudentToGroupCommand` → creates `GroupMembership`; validates student profile via Persona gRPC |
| SCH-07 | Manager can remove a student from a group | `RemoveStudentFromGroupCommand` → removes `GroupMembership` |
| SCH-10 | All dates/times stored as DateTimeOffset | All aggregate properties, EF column type `timestamp with time zone`; use `DateTimeHelper.UtcNow()` |
</phase_requirements>

---

## Standard Stack

### Core

| Library | Version | Purpose | Why Standard |
|---------|---------|---------|--------------|
| Edvantix.Chassis | (local) | CQRS, EF helpers, endpoints, event bus, tenant, guards | Project's cross-cutting lib — all services use it |
| Edvantix.SharedKernel | (local) | `Entity`, `IAggregateRoot`, `ISoftDelete`, `HasDomainEvents` | All aggregate bases live here |
| Edvantix.Constants | (local) | Aspire service names, permission strings | Adding `Services.Scheduling`, `Components.Database.Scheduling`, `SchedulingPermissions` |
| Microsoft.EntityFrameworkCore | (via ServiceDefaults) | ORM | Already used; adds `SchedulingDbContext` |
| MassTransit.EntityFrameworkCore | (via csproj, same ver as Organizations) | Outbox tables in DbContext (wire infra now; events in Phase 4) | Consistent with Organizations |
| Mediator (Mediator.SourceGenerator) | (via csproj) | Source-gen CQRS dispatch — no reflection overhead | Same as Organizations |
| FluentValidation | (via ServiceDefaults) | Command/query input validation | Same as Organizations |
| Grpc.AspNetCore.Server.ClientFactory | (via csproj) | Consume Persona gRPC (profile validation) | Same as Organizations |

### Supporting

| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| TngTech.ArchUnitNET.TUnit | (via ArchTests csproj) | Architecture tests | Must add Scheduling assembly to `BaseTest.cs` + `ArchUnitBaseTest.cs` |
| Microsoft.EntityFrameworkCore.InMemory | (via ArchTests csproj) | `TenantIsolationTests` for SchedulingDbContext | Already in ArchTests project |

### Installation

No new external NuGet packages needed. The Scheduling `.csproj` mirrors `Edvantix.Organizations.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <ItemGroup>
    <InternalsVisibleTo Include="DynamicProxyGenAssembly2" />
    <InternalsVisibleTo Include="$(AssemblyName).UnitTests" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Grpc.Tools" PrivateAssets="all" />
    <PackageReference Include="Grpc.AspNetCore.Server.ClientFactory" />
    <PackageReference Include="MassTransit.EntityFrameworkCore" />
    <PackageReference Include="Mediator.SourceGenerator" PrivateAssets="all" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\..\Aspire\Edvantix.ServiceDefaults\Edvantix.ServiceDefaults.csproj" />
  </ItemGroup>
  <ItemGroup>
    <!-- Persona gRPC client for profile validation in group membership commands -->
    <Protobuf Include="Grpc\Protos\persona\v1\profile.proto" GrpcServices="Client" />
  </ItemGroup>
</Project>
```

---

## Architecture Patterns

### Recommended Project Structure

```
src/Services/Scheduling/Edvantix.Scheduling/
├── Configurations/
│   └── SchedulingAppSettings.cs
├── Domain/
│   ├── AggregatesModel/
│   │   ├── GroupAggregate/
│   │   │   ├── Group.cs                  # Entity + IAggregateRoot + ISoftDelete + ITenanted
│   │   │   ├── GroupMembership.cs        # Entity within Group (owned or navigation)
│   │   │   └── IGroupRepository.cs
│   │   └── LessonSlotAggregate/
│   │       ├── LessonSlot.cs             # Entity + IAggregateRoot + ITenanted
│   │       └── ILessonSlotRepository.cs
│   └── EventHandlers/                    # Domain event handlers (Mediator INotificationHandler)
├── Features/
│   ├── Groups/
│   │   ├── CreateGroup/
│   │   ├── UpdateGroup/
│   │   ├── DeleteGroup/
│   │   └── GetGroups/
│   ├── LessonSlots/
│   │   ├── CreateLessonSlot/
│   │   ├── EditLessonSlot/
│   │   └── DeleteLessonSlot/
│   ├── Schedule/
│   │   └── GetSchedule/                  # Permission-filtered query
│   └── GroupMembership/
│       ├── AddStudentToGroup/
│       └── RemoveStudentFromGroup/
├── Grpc/
│   ├── Extensions.cs                     # AddGrpcServices() — Persona client
│   ├── Services/
│   │   ├── IPersonaProfileService.cs
│   │   └── PersonaProfileService.cs
│   └── Protos/persona/v1/profile.proto   # copy from Organizations
├── Infrastructure/
│   ├── EntityConfigurations/
│   │   ├── GroupConfiguration.cs
│   │   ├── GroupMembershipConfiguration.cs
│   │   └── LessonSlotConfiguration.cs
│   ├── Repositories/
│   │   ├── GroupRepository.cs
│   │   └── LessonSlotRepository.cs
│   ├── Seeding/
│   │   └── PermissionSeeder.cs           # IHostedService, same pattern as Organizations
│   ├── Extensions.cs                     # AddPersistenceServices()
│   └── SchedulingDbContext.cs
├── Extensions/
│   └── Extensions.cs                     # AddApplicationServices()
├── GlobalUsings.cs
├── ISchedulingApiMarker.cs
├── Program.cs
└── Edvantix.Scheduling.csproj
```

### Pattern 1: ITenanted Aggregate with Combined HasQueryFilter

Every ITenanted aggregate in Scheduling must have `HasQueryFilter` in `SchedulingDbContext.OnModelCreating`. Copy the Organizations pattern exactly — EF Core allows only one `HasQueryFilter` per entity type, so combine tenant + soft-delete into a single lambda.

```csharp
// Source: OrganizationsDbContext.cs (verified in codebase)
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.AddInboxStateEntity();
    modelBuilder.AddOutboxMessageEntity();
    modelBuilder.AddOutboxStateEntity();
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);

    // Group: tenant + soft-delete combined (EF Core allows only one HasQueryFilter per entity)
    modelBuilder
        .Entity<Group>()
        .HasQueryFilter(g => g.SchoolId == tenantContext.SchoolId && !g.IsDeleted);

    // LessonSlot: tenant-only (no soft delete — hard delete per Phase 3 scope)
    modelBuilder
        .Entity<LessonSlot>()
        .HasQueryFilter(s => s.SchoolId == tenantContext.SchoolId);

    // GroupMembership: tenant-scoped (inherits through Group navigation or explicit)
    modelBuilder
        .Entity<GroupMembership>()
        .HasQueryFilter(m => m.SchoolId == tenantContext.SchoolId);
}
```

### Pattern 2: Cross-Tenant Conflict Check via IgnoreQueryFilters

Teacher conflict detection is global (D-04). Use `IgnoreQueryFilters()` to bypass the per-school filter, then filter by `teacherId` and time overlap manually.

```csharp
// Source: derived from Organizations Phase 02-02 pattern — GetAllByRoleIdAsync uses IgnoreQueryFilters()
// Conflict query in ILessonSlotRepository or inline in CreateLessonSlotCommandHandler:
var conflict = await dbContext.LessonSlots
    .IgnoreQueryFilters()                          // bypass tenant filter — global check
    .AnyAsync(s =>
        s.TeacherId == teacherId
        && s.StartTime < endTime                   // D-05: exact overlap
        && s.EndTime > startTime
        && s.Id != excludedSlotId,                 // D-06: exclude self when editing
        cancellationToken);
```

### Pattern 3: Permission-Filtered Schedule Query (D-08 / D-09)

The `GetSchedule` query handler calls `PermissionsGrpcService.CheckPermission` (via the Chassis gRPC client) to determine the caller's access level, then shapes the response accordingly. The handler receives the caller's `profileId` from the injected `ClaimsPrincipal` (via `IServiceProvider.TryGetProfileId()`) and `schoolId` from `ITenantContext`.

```csharp
// Source: PolicyBuilderExtensions.cs + PermissionsGrpcService.cs (verified in codebase)
// Pattern for determining caller role level inside the query handler:

// 1. Endpoint registered with RequireAuthorization("scheduling.view-schedule") — baseline gate
// 2. Inside GetScheduleQueryHandler, check for manager-level:
var isManager = (await grpcClient.CheckPermissionAsync(new CheckPermissionRequest
{
    UserId = profileId.ToString(),
    SchoolId = schoolId.ToString(),
    Permission = SchedulingPermissions.CreateLessonSlot   // proxy for manager
})).HasPermission;
// Shape response based on isManager / isTeacher / default (student)
```

**Note:** The endpoint uses `.RequireAuthorization(SchedulingPermissions.ViewSchedule)` as the baseline (one gRPC call via `PermissionRequirementHandler`). Inside the handler an additional call distinguishes manager from teacher/student. Two gRPC calls per schedule request is acceptable for v1; the Organizations HybridCache absorbs the cost.

### Pattern 4: GroupMembership Design Decision

`GroupMembership` is a **navigation entity within `Group`** (not a separate `IAggregateRoot`). This mirrors how `RolePermission` lives inside `Role`. The `Group` aggregate root owns the collection and exposes `AddMember` / `RemoveMember` methods. The backing field uses `PropertyAccessMode.Field` (same as `Role._permissions`).

`GroupMembership` carries: `Id`, `GroupId`, `StudentId` (profileId from Persona), `SchoolId` (for tenant filter), `JoinedAt` (DateTimeOffset). A unique index on `(GroupId, StudentId)` prevents duplicate membership — same approach as `UserRoleAssignment` unique constraint.

### Pattern 5: PermissionSeeder (IHostedService)

Identical to `Organizations/Infrastructure/Seeding/PermissionSeeder.cs`. Injects `IServiceProvider`, creates a scope, resolves `IPermissionRepository`, calls `UpsertAsync(SchedulingPermissions.All, ct)`, saves. Registered via `services.AddHostedService<PermissionSeeder>()` in `AddApplicationServices`.

**CRITICAL:** The `IPermissionRepository` lives in the **Organizations** service domain. The Scheduling service must call the Organizations HTTP `POST /v1/permissions/register` endpoint on startup to seed its permissions — OR follow the same direct-DB pattern. Because Scheduling has its own DB that does NOT contain the Permission catalogue (which lives in Organizations DB), Scheduling must call the Organizations HTTP registration endpoint. This is the same `RegisterPermissionsEndpoint` (AllowAnonymous, service-to-service) used by other services.

**Revision:** Looking at the Organizations seeder — it calls `IPermissionRepository.UpsertAsync` which writes to the `Organizations` DB. Scheduling's own DB does NOT have a Permission table. Therefore Scheduling must use the HTTP endpoint approach: call `POST {organizationsApi}/v1/permissions/register` on startup with its permission strings. Use `IHostedService` with a typed `HttpClient` pointing at the Organizations service.

### Pattern 6: Aspire Bootstrap (D-13)

Following the Organizations pattern in `AppHost.cs`:

```csharp
// Source: AppHost.cs (verified in codebase)
var schedulingDb = postgres.AddDatabase(Components.Database.Scheduling);

var schedulingApi = builder
    .AddProject<Edvantix_Scheduling>(Services.Scheduling)
    .WithReference(schedulingDb)
    .WaitFor(schedulingDb)
    .WithKeycloak(keycloak)
    .WaitFor(keycloak)
    .WithReference(personaApi)      // gRPC profile validation for membership commands
    .WaitFor(personaApi)
    .WithReference(organizationsApi) // gRPC CheckPermission for authorization
    .WaitFor(organizationsApi)
    .WithReference(queue)           // Kafka — wire now, events in Phase 4
    .WaitFor(queue)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls();

// Add to gateway:
var gateway = builder.AddApiGatewayProxy()
    // ... existing services ...
    .WithService(schedulingApi, true)  // useProtobuf: true (same as others)
    .Build();

// Add to Scalar dashboard:
if (builder.ExecutionContext.IsRunMode)
{
    builder.AddScalar(keycloak)
        // ... existing ...
        .WithOpenAPI(schedulingApi);
}
```

### Pattern 7: Architecture Test Extension

The `TenantIsolationTests` currently only tests `OrganizationsDbContext`. A parallel test class `SchedulingTenantIsolationTests` must be added to the `Edvantix.ArchTests` project. The `BaseTest` and `ArchUnitBaseTest` must load the `Scheduling` assembly.

### Anti-Patterns to Avoid

- **HasQueryFilter in `IEntityTypeConfiguration`**: EF Core cannot inject `ITenantContext` inside `ApplyConfigurationsFromAssembly`. All `HasQueryFilter` calls MUST be in `SchedulingDbContext.OnModelCreating`.
- **Using `DateTime.Now` or `DateTime.UtcNow`**: Use `DateTimeHelper.UtcNow()` from Chassis. All `DateTimeOffset` column types.
- **Checking permission inside `Group` aggregate**: Authorization is the application layer's concern. Aggregates enforce domain invariants only.
- **Calling `SaveEntitiesAsync` without dispatching domain events**: `IUnitOfWork.SaveEntitiesAsync` dispatches domain events; calling `SaveChangesAsync` directly bypasses the event pipeline.
- **Forgetting `IgnoreQueryFilters()` on conflict check**: Without it the conflict query is tenant-scoped and a teacher booked at another school will not be detected.
- **Registering Scheduling permissions via direct DB**: Scheduling has no `Permission` table. Must use Organizations HTTP registration endpoint (or copy-paste the Organizations IPermissionRepository injection — which would introduce a cross-service DB dependency, which is wrong). Use HTTP.

---

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Permission check | Custom JWT claim parser | `builder.AddPermissionAuthorization(...)` + `.RequirePermission(permission)` | Already built in Chassis; calls Organizations gRPC with caching |
| Tenant isolation | Manual `WHERE school_id = ?` in every query | `ITenanted` + `HasQueryFilter` in DbContext | EF Core enforces automatically; arch test catches violations |
| Conflict detection overlap math | Custom interval library | Inline LINQ condition `s.StartTime < endTime && s.EndTime > startTime` | The two-condition half-open interval overlap is simple and proven |
| Domain event dispatch | Custom mediator publish | `entity.RegisterDomainEvent(evt)` + `IUnitOfWork.SaveEntitiesAsync` | Chassis `EventDispatchInterceptor` handles dispatch automatically on save |
| gRPC client for Persona | Manual HttpClient + JSON | `services.AddGrpcServiceReference<ProfileGrpcServiceClient>(url, HealthStatus.Degraded)` | Aspire service discovery + health integration |
| EF Core migrations | Custom SQL scripts | `services.AddMigration<SchedulingDbContext>()` (Chassis extension) | Auto-runs on startup; same as Organizations |
| API versioning | Custom headers | `services.AddVersioning()` + `.MapToApiVersion(ApiVersions.V1)` | Already abstracted in Chassis |

---

## Common Pitfalls

### Pitfall 1: ITenanted on GroupMembership Forgotten

**What goes wrong:** GroupMembership entity implements `IAggregateRoot` (wrong) or does not implement `ITenanted` and has no `HasQueryFilter`. The arch test for tenant isolation will fail.

**Why it happens:** Membership looks like a simple join table and developers skip the tenancy concern.

**How to avoid:** GroupMembership must carry `SchoolId` and have `HasQueryFilter(m => m.SchoolId == tenantContext.SchoolId)` in `SchedulingDbContext`. Add it alongside `Group` and `LessonSlot`.

**Warning signs:** `TenantIsolationTests` fails with "Entity GroupMembership implements ITenanted but has no HasQueryFilter".

### Pitfall 2: Conflict Check Uses Tenant-Scoped Query

**What goes wrong:** `ILessonSlotRepository.HasConflict(teacherId, start, end)` omits `IgnoreQueryFilters()`. The query only checks slots within the current school. A teacher booked at another school at the same time passes the check.

**Why it happens:** The repository method is written against the typed `DbSet<LessonSlot>` which carries the query filter.

**How to avoid:** The conflict check method must call `dbContext.LessonSlots.IgnoreQueryFilters()`. Document in the repository interface why this is intentional.

**Warning signs:** No test failure at first; discovered only in integration tests with two-school scenarios.

### Pitfall 3: PermissionSeeder Tries to Write to Scheduling DB

**What goes wrong:** Developer copies `PermissionSeeder` from Organizations and injects `IPermissionRepository` — which does not exist in Scheduling's DI container because Scheduling has no `Permission` DbSet.

**Why it happens:** Direct copy-paste without adapting to cross-service scenario.

**How to avoid:** Scheduling's `PermissionSeeder` must use an `HttpClient` pointed at `Services.Organizations` to call `POST /v1/permissions/register`. Use a named `HttpClient` registered with service discovery. The endpoint is `AllowAnonymous` by design (service-to-service bootstrap).

**Warning signs:** `InvalidOperationException: No service registered for IPermissionRepository` at startup.

### Pitfall 4: GetSchedule Returns Wrong Fields

**What goes wrong:** The query handler returns the same DTO to all callers instead of shaping by permission level.

**Why it happens:** Simpler to return a single flat DTO.

**How to avoid:** Inside `GetScheduleQueryHandler`, after the baseline authorization gate (`PermissionRequirementHandler` has already confirmed `scheduling.view-schedule`), make a second gRPC call to check `scheduling.create-lesson-slot` (manager proxy) and shape the response. The second call is served from HybridCache.

**Warning signs:** Students can see teacher names; teachers can see student counts without permission.

### Pitfall 5: Arch Tests Do Not Cover Scheduling Assembly

**What goes wrong:** Scheduling service violates handler or query naming conventions but no test fails because `Edvantix.Scheduling` assembly is not in the ArchUnitNET scan list.

**Why it happens:** `BaseTest.cs` and `ArchUnitBaseTest.cs` list assemblies explicitly; new services must be added manually.

**How to avoid:** In Plan 01, add `SchedulingAssembly = typeof(ISchedulingApiMarker).Assembly` to `BaseTest`, add it to `ArchUnitBaseTest.Architecture` loader, add `SchedulingServiceTypes` object provider, and add a `SchedulingTenantIsolationTests` class.

**Warning signs:** All arch tests pass but Scheduling has unchecked violations.

### Pitfall 6: DateTimeOffset Stored as Local Time

**What goes wrong:** EF Core stores `DateTimeOffset` without specifying `HasColumnType("timestamp with time zone")`. PostgreSQL stores it as `timestamp` (no timezone), and the offset information is silently discarded.

**Why it happens:** EF Core's default mapping for `DateTimeOffset` on Npgsql may vary.

**How to avoid:** In `LessonSlotConfiguration`, explicitly set `.HasColumnType("timestamp with time zone")` on `StartTime` and `EndTime`. Apply to `GroupMembership.JoinedAt` as well.

**Warning signs:** SCH-10 integration test fails; slots appear at wrong times across timezones.

---

## Code Examples

### Group Aggregate

```csharp
// Source: Role.cs pattern (verified in codebase) + D-01 / D-02 decisions
public sealed class Group : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
    private readonly List<GroupMembership> _members = [];

    public string Name { get; private set; } = string.Empty;
    public Guid SchoolId { get; private set; }
    public int MaxCapacity { get; private set; }
    public string Color { get; private set; } = string.Empty;
    public bool IsDeleted { get; set; }

    public IReadOnlyCollection<GroupMembership> Members => _members.AsReadOnly();

    private Group() { }

    public Group(string name, Guid schoolId, int maxCapacity, string color)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.Default(schoolId, nameof(schoolId));
        Name = name.Trim();
        SchoolId = schoolId;
        MaxCapacity = maxCapacity;
        Color = color;
    }

    public void Delete() => IsDeleted = true;

    public void AddMember(Guid studentId, DateTimeOffset joinedAt)
    {
        Guard.Against.Default(studentId, nameof(studentId));
        if (_members.Any(m => m.StudentId == studentId)) return;
        _members.Add(new GroupMembership(Id, SchoolId, studentId, joinedAt));
    }

    public void RemoveMember(Guid studentId)
    {
        var existing = _members.FirstOrDefault(m => m.StudentId == studentId);
        if (existing is not null) _members.Remove(existing);
    }
}
```

### LessonSlot Aggregate

```csharp
// Source: Entity.cs + UserRoleAssignment.cs pattern (verified in codebase)
public sealed class LessonSlot : Entity, IAggregateRoot, ITenanted
{
    public Guid SchoolId { get; private set; }
    public Guid GroupId { get; private set; }
    public Guid TeacherId { get; private set; }       // profileId from Persona
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }

    private LessonSlot() { }

    public LessonSlot(Guid schoolId, Guid groupId, Guid teacherId,
                      DateTimeOffset startTime, DateTimeOffset endTime)
    {
        Guard.Against.Default(schoolId, nameof(schoolId));
        Guard.Against.Default(groupId, nameof(groupId));
        Guard.Against.Default(teacherId, nameof(teacherId));
        SchoolId = schoolId;
        GroupId = groupId;
        TeacherId = teacherId;
        StartTime = startTime;
        EndTime = endTime;
    }

    public void Reschedule(DateTimeOffset newStart, DateTimeOffset newEnd)
    {
        StartTime = newStart;
        EndTime = newEnd;
    }

    public void ChangeTeacher(Guid newTeacherId)
    {
        Guard.Against.Default(newTeacherId, nameof(newTeacherId));
        TeacherId = newTeacherId;
    }
}
```

### Conflict Detection Query (in Repository or Command Handler)

```csharp
// Source: Phase 02-02 IgnoreQueryFilters pattern (GetAllByRoleIdAsync) — verified in codebase
// D-04: global across schools; D-05: exact overlap; D-06: exclude self
public async Task<bool> HasConflictAsync(
    Guid teacherId, DateTimeOffset startTime, DateTimeOffset endTime,
    Guid? excludedSlotId, CancellationToken ct)
{
    return await _context.LessonSlots
        .IgnoreQueryFilters()           // cross-tenant — global check
        .AnyAsync(s =>
            s.TeacherId == teacherId
            && s.StartTime < endTime    // D-05 overlap condition (half-open intervals)
            && s.EndTime > startTime
            && (excludedSlotId == null || s.Id != excludedSlotId.Value),  // D-06
            ct);
}
```

### PermissionSeeder Using HTTP (cross-service)

```csharp
// Scheduling cannot inject IPermissionRepository — that belongs to Organizations DB.
// Use HTTP to call Organizations POST /v1/permissions/register on startup.
internal sealed class PermissionSeeder(
    IHttpClientFactory httpClientFactory,
    ILogger<PermissionSeeder> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering Scheduling permission strings via Organizations API...");

        var client = httpClientFactory.CreateClient("organizations");
        var response = await client.PostAsJsonAsync(
            "/v1/permissions/register",
            new { Names = SchedulingPermissions.All },
            cancellationToken
        );
        response.EnsureSuccessStatusCode();

        logger.LogInformation("Scheduling permissions registered: {Count}", SchedulingPermissions.All.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
// Register: services.AddHttpClient("organizations", c => c.BaseAddress = new Uri($"http://{Services.Organizations}"))
//           services.AddHostedService<PermissionSeeder>();
```

### AddPermissionAuthorization in Scheduling Extensions

```csharp
// Source: PolicyBuilderExtensions.cs (verified in codebase)
// Call this in Scheduling's AddApplicationServices() BEFORE defining policies:
builder.AddPermissionAuthorization(
    HttpUtilities.AsUrlBuilder()
        .WithScheme(builder.GetScheme())
        .WithHost(Services.Organizations)
        .Build()
);

// Then define policies per permission (or use .RequirePermission inline):
services.AddAuthorizationBuilder()
    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireScope($"{Services.Scheduling}_{Authorization.Actions.Read}")
        .Build())
    .AddPolicy(SchedulingPermissions.CreateGroup,
        p => p.RequirePermission(SchedulingPermissions.CreateGroup))
    // ... one policy per permission string ...
```

### Endpoint Example with RequireAuthorization

```csharp
// Source: CreateRoleEndpoint.cs pattern (verified in codebase)
app.MapPost(
        "/groups",
        async (CreateGroupCommand command, ISender sender, ...) =>
            await HandleAsync(command, sender, ...)
    )
    .Produces<Guid>(StatusCodes.Status201Created)
    .WithName("CreateGroup")
    .WithTags("Groups")
    .MapToApiVersion(ApiVersions.V1)
    .RequireAuthorization(SchedulingPermissions.CreateGroup);
```

---

## State of the Art

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| `DateTime.UtcNow` | `DateTimeHelper.UtcNow()` (Chassis) | Phase 01 | Use consistently in Scheduling (SCH-10) |
| Manual tenant WHERE clause | EF Core `HasQueryFilter` + `ITenanted` | Phase 01 | All new entities get the filter |
| Reflection-based Mediator (MediatR) | Mediator source-generator (compile-time) | Phase 01 | Use `ICommand<T>` / `IQuery<T>` — NOT MediatR interfaces |
| Ardalis GuardClauses | Custom `Guard.Against` extension methods | Phase 01-02 | Use project's `Guard.Against.NullOrWhiteSpace` / `Default` |

---

## Open Questions

1. **Should `GroupMembership.SchoolId` be denormalized or derived via Group FK join?**
   - What we know: `UserRoleAssignment` denormalizes `SchoolId` for the tenant filter. It does not navigate through `Role.SchoolId`.
   - What's unclear: Whether the tenant filter lambda can traverse navigation properties (`m => m.Group.SchoolId == ...`) or requires a direct column.
   - Recommendation: Denormalize `SchoolId` on `GroupMembership` (same pattern as `UserRoleAssignment`). EF Core query filters on navigation properties work but add a required join on every query; direct column is simpler and consistent.

2. **Index strategy for conflict detection**
   - What we know: The conflict query filters on `TeacherId + StartTime + EndTime` across all rows (no tenant filter). With hundreds of slots this is a full table scan without an index.
   - What's unclear: Whether to use a partial index (PostgreSQL-specific `CREATE INDEX ON lesson_slots (teacher_id, start_time, end_time) WHERE is_deleted = false`) or a covering index.
   - Recommendation: Add a regular composite index on `(TeacherId, StartTime, EndTime)` in `LessonSlotConfiguration` via EF Core `.HasIndex(s => new { s.TeacherId, s.StartTime, s.EndTime })`. This satisfies v1 scale. A partial index can be added in a later migration when needed.

3. **How to distinguish Manager from Teacher in GetSchedule handler**
   - What we know: D-08 says permission-based, not role-based. `scheduling.view-schedule` is the baseline. The proxy for "manager" is holding `scheduling.create-lesson-slot`.
   - Recommendation: In the handler, after baseline auth: check `scheduling.create-lesson-slot` (manager). If false, check whether the caller's `profileId` matches any `TeacherId` in the returned slots (implicit teacher detection). Otherwise treat as student. This avoids a second distinct permission string for "teacher view".

---

## Validation Architecture

### Test Framework

| Property | Value |
|----------|-------|
| Framework | TUnit (TngTech.ArchUnitNET.TUnit) |
| Config file | `tests/Edvantix.ArchTests/Edvantix.ArchTests.csproj` |
| Quick run command | `dotnet test tests/Edvantix.ArchTests` |
| Full suite command | `dotnet test` |

### Phase Requirements → Test Map

| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|-------------|
| SCH-01 | Tenant filter on Group + LessonSlot; conflict check is cross-tenant | Arch/unit | `dotnet test tests/Edvantix.ArchTests` | Wave 0 — new class |
| SCH-02 | Edit excludes self from conflict query; delete works | Unit | `dotnet test tests/Edvantix.ArchTests` | Wave 0 |
| SCH-03 | Manager gets all-tenant slots | Unit (query handler) | `dotnet test tests/Edvantix.ArchTests` | Wave 0 |
| SCH-04 | Teacher gets only own slots | Unit (query handler) | Manual-only in v1 (no integration test project) | — |
| SCH-05 | Student gets only group-membership slots | Unit (query handler) | Manual-only in v1 | — |
| SCH-06 | Add student creates GroupMembership; duplicate is no-op | Unit (aggregate) | Wave 0 — unit test file | ❌ |
| SCH-07 | Remove student removes GroupMembership | Unit (aggregate) | Wave 0 — unit test file | ❌ |
| SCH-10 | No `DateTime.Now`/`UtcNow` usage | Arch test | `dotnet test tests/Edvantix.ArchTests` | Existing test may cover — verify |

### Sampling Rate

- **Per task commit:** `dotnet test tests/Edvantix.ArchTests`
- **Per wave merge:** `dotnet test`
- **Phase gate:** Full suite green before `/gsd:verify-work`

### Wave 0 Gaps

- [ ] `tests/Edvantix.ArchTests/Abstractions/BaseTest.cs` — add `SchedulingAssembly`
- [ ] `tests/Edvantix.ArchTests/Abstractions/ArchUnitBaseTest.cs` — add `SchedulingAssembly` to `Architecture` loader and `SchedulingServiceTypes`
- [ ] `tests/Edvantix.ArchTests/Domain/SchedulingTenantIsolationTests.cs` — parallel to `TenantIsolationTests` but for `SchedulingDbContext`
- [ ] `Edvantix.ArchTests.csproj` — add `<ProjectReference>` to `Edvantix.Scheduling.csproj` once the project exists
- [ ] Unit test project for aggregate behavior (Group.AddMember, LessonSlot.Reschedule) — new `.UnitTests` csproj following Organizations pattern (if one exists)

---

## Sources

### Primary (HIGH confidence)

- `src/Services/Organizations/Edvantix.Organizations/` — all patterns verified directly in codebase
- `src/BuildingBlocks/Edvantix.Chassis/Security/Extensions/PolicyBuilderExtensions.cs` — `AddPermissionAuthorization` / `RequirePermission` confirmed
- `src/BuildingBlocks/Edvantix.Chassis/Security/Authorization/PermissionRequirementHandler.cs` — gRPC permission check handler confirmed
- `src/Aspire/Edvantix.AppHost/AppHost.cs` — Aspire bootstrap pattern confirmed
- `src/Aspire/Edvantix.AppHost/Extensions/Network/ProxyExtensions.cs` — gateway `WithService` pattern confirmed
- `tests/Edvantix.ArchTests/` — all architecture test conventions confirmed

### Secondary (MEDIUM confidence)

- `.planning/phases/03-scheduling-slots-and-views/03-CONTEXT.md` — locked decisions D-01 through D-14
- `.planning/STATE.md` — accumulated decisions from Phase 01-02

### Tertiary (LOW confidence)

- Timestamp with time zone recommendation for PostgreSQL + EF Core `DateTimeOffset` mapping — common PostgreSQL knowledge; should be verified with an EF Core migration dry-run

---

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH — all packages already in use; Scheduling .csproj is a direct copy
- Architecture: HIGH — every pattern has a concrete counterpart in the Organizations service codebase
- Pitfalls: HIGH — most derived from STATE.md accumulated decisions and direct code inspection; conflict check pitfall is HIGH because the `IgnoreQueryFilters` technique is already established in Phase 02-02
- Validation: MEDIUM — unit test project for aggregates may not yet exist; needs verification when Scheduling project is created

**Research date:** 2026-03-21
**Valid until:** 2026-06-21 (stable stack — 90 days)
