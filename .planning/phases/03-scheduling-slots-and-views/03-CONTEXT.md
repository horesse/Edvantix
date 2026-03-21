# Phase 3: Scheduling — Slots and Views - Context

**Gathered:** 2026-03-21
**Status:** Ready for planning

<domain>
## Phase Boundary

Managers build and manage lesson schedules (groups + lesson slots with conflict detection). Teachers see only their own slots. Students see slots for their groups. All three views enforced via Organizations permission checks. Group membership management (add/remove students) is included. Attendance marking and outbox events are Phase 4.

Groups and GroupMembership live in the Organizations service, not the Scheduling service. The Scheduling service only stores LessonSlots, which reference a GroupId as a plain Guid value (no EF navigation property, no cross-service FK).

</domain>

<decisions>
## Implementation Decisions

### Group aggregate
- **D-01:** Group properties: `Name` (string), `SchoolId` (ITenanted), `MaxCapacity` (int), `Color` (string — value from a UI-defined set; backend stores the identifier as-is, no validation against allowed values)
- **D-02:** Soft delete (like `Role`) — `ISoftDelete` interface with combined tenant + soft-delete `HasQueryFilter`
- **D-03:** Groups can exist without lesson slots (pure student container); no constraint enforcing at least one slot

### Teacher conflict detection
- **D-04:** Conflict check is **global** — a teacher cannot be double-booked across different schools at the same time
- **D-05:** Conflict = exact time overlap (`slot.StartTime < existing.EndTime && slot.EndTime > existing.StartTime`); no buffer required
- **D-06:** When editing a slot (reschedule or teacher swap), the current slot is **automatically excluded** from the conflict query — the command handler filters out `slotId == currentSlotId`

### Calendar view endpoint
- **D-07:** Single `GET /schedule` endpoint with `dateFrom` / `dateTo` query parameters; the UI calendar controls the range
- **D-08:** Access is permission-based, not role-based: the same endpoint returns different data depending on which `scheduling.*` permission the caller holds (checked via Organizations gRPC `CheckPermission`)
- **D-09:** Returned fields per slot (Claude's discretion per best practices):
  - **All callers:** `id`, `startTime`, `endTime`, `groupId`, `groupName`, `groupColor`
  - **Manager** (`scheduling.view-schedule` with manager permission): + `teacherId`, `teacherName`, `studentCount`
  - **Teacher** (`scheduling.view-schedule`): + `studentCount`
  - **Student** (`scheduling.view-schedule`): + `teacherId`, `teacherName`

### Service bootstrap
- **D-10:** New `Edvantix.Scheduling` service created from scratch following Organizations service structure (feature folders, EF Core, DDD aggregates, `ITenanted`, permission seeder)
- **D-11:** `Services.Scheduling` and `Components.Database.Scheduling` constants added to `Edvantix.Constants.Aspire`
- **D-12:** `SchedulingPermissions` class added to `Edvantix.Constants.Permissions` following `OrganizationsPermissions` pattern; permissions registered:
  - `scheduling.create-lesson-slot`
  - `scheduling.edit-lesson-slot`
  - `scheduling.delete-lesson-slot`
  - `scheduling.view-schedule`
- **D-13:** Service added to `AppHost.cs` with: `schedulingDb` (same PostgreSQL instance, separate database), Keycloak, Kafka queue, reference to `personaApi` (gRPC profile validation), reference to `organizationsApi` (gRPC CheckPermission); added to Scalar OpenAPI dashboard
- **D-14:** Scheduling Keycloak client (`${CLIENT_SCHEDULING_ID}`) added to `EdvantixRealms.json` following the same pattern as Persona/Blog/Notification clients (clientType `API`)
- **D-15:** Groups and GroupMembership live in Organizations service, not Scheduling. Scheduling references GroupId as a plain Guid value.
- **D-16:** Group/membership permissions use `groups.*` namespace (GroupsPermissions class). Seeded by Organizations PermissionSeeder.
- **D-17:** Organization aggregate added to Organizations service with only Id in v1. It is the tenant root — not ITenanted itself.

### Claude's Discretion
- Exact EF Core index strategy for conflict detection query (partial index on teacher + time range vs. application-level query)
- Permission seeder startup pattern (same `IHostedService` approach as Organizations)
- Gateway routing entry for Scheduling

</decisions>

<specifics>
## Specific Ideas

- Teacher conflict detection must work globally — a teacher could be shared across schools in future v2; the global check future-proofs this
- The `GET /schedule` single-endpoint approach mirrors how Organizations uses one CheckPermission endpoint that returns different data by context — consistent API design
- `Color` stored as string (e.g., `"blue"`, `"red"`) — the UI hardcodes the visual set; the backend is just a label store

</specifics>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Scheduling requirements
- `.planning/REQUIREMENTS.md` S Scheduling — SCH-01-SCH-07, SCH-10: full requirement matrix for groups, slots, conflict detection, and views

### Domain patterns (reuse from Organizations)
- `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/RoleAggregate/Role.cs` — soft-delete + ITenanted aggregate pattern
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/OrganizationsDbContext.cs` — combined tenant + soft-delete HasQueryFilter
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/RoleConfiguration.cs` — partial unique index pattern
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/Seeding/PermissionSeeder.cs` — IHostedService permission seeder

### Constants (must extend)
- `src/BuildingBlocks/Edvantix.Constants/Aspire/Services.cs` — add `Scheduling` entry
- `src/BuildingBlocks/Edvantix.Constants/Aspire/Components.cs` — add `Database.Scheduling` entry
- `src/BuildingBlocks/Edvantix.Constants/Permissions/OrganizationsPermissions.cs` — template for new `SchedulingPermissions` and `GroupsPermissions` classes

### Aspire integration
- `src/Aspire/Edvantix.AppHost/AppHost.cs` — add `schedulingDb` + `schedulingApi` following `organizationsApi` pattern
- `src/Aspire/Edvantix.AppHost/Container/keycloak/realms/EdvantixRealms.json` — add `${CLIENT_SCHEDULING_ID}` client entry following Persona client pattern (lines ~1135-1290)
- `src/Aspire/Edvantix.AppHost/Extensions/Security/KeycloakExtensions.cs` — `.WithKeycloak()` extension for project resources

### Organizations gRPC (for permission checks in Scheduling)
- `src/Services/Organizations/Edvantix.Organizations/Grpc/Services/PermissionsGrpcService.cs` — gRPC service to call for CheckPermission
- `.planning/phases/02-organizations-permission-cache/02-CONTEXT.md` — decisions for CheckPermission gRPC + downstream `.RequireAuthorization(permissionString)` pattern

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `Entity` / `IAggregateRoot` / `ISoftDelete` / `ITenanted` — all needed for Group and LessonSlot aggregates; same as Organizations
- `HasDomainEvents` mixin — for LessonSlot domain events (e.g., `SlotTeacherChangedEvent` if cache invalidation needed in future)
- `IUnitOfWork` + `SaveEntitiesAsync` — dispatch domain events on save; same pattern as Organizations
- `Guard.Against.NullOrWhiteSpace` / `Guard.Against.Default` — available in Chassis (added in Phase 01-02)
- `DateTimeHelper.UtcNow()` — use instead of `DateTime.UtcNow` or `DateTimeOffset.UtcNow` for consistency

### Established Patterns
- Feature folders: `Features/{Feature}/{Action}/{Command|Query|Endpoint|Validator}.cs`
- `ICommand<T>` / `ICommandHandler<TCommand, T>` — use `ICommand<Unit>` for void commands
- `IQuery<T>` / `IQueryHandler` — query names must start with `Get` / `List` / `Visualize` / `Summarize` (arch rule)
- `HasQueryFilter` set in `DbContext.OnModelCreating` (not `IEntityTypeConfiguration`) to allow `ITenantContext` injection
- `IgnoreQueryFilters()` needed for cross-tenant operations (e.g., global teacher conflict check)
- Namespace-per-aggregate added to `GlobalUsings.cs`

### Integration Points
- `ITenantContext` — injected via scoped DI, populated by `TenantMiddleware` (must be registered before `UseAuthorization`)
- Organizations gRPC `CheckPermission` — Scheduling calls this to enforce `scheduling.*` permissions; reuse the client pattern from Phase 02-03 (proto in Chassis)
- Kafka + MassTransit outbox — Scheduling will need this for Phase 4 (outbox); wire up the Kafka consumer/producer infrastructure now even if no events are published in Phase 3

</code_context>

<deferred>
## Deferred Ideas

- **Organizations missing from Keycloak realm** — Organizations service client (`${CLIENT_ORGANIZATIONS_ID}`) is absent from `EdvantixRealms.json`; captured as a separate backlog item, not part of Phase 3
- **Recurring/repeating lesson slots** — v2 feature; Phase 3 handles one-off slots only
- **Group templates or copying** — not needed in v1
- **Teacher availability configuration** — explicit availability windows are out of scope; conflict detection is purely slot-based
- **Student capacity enforcement** — `MaxCapacity` is stored but enforcing it on `AddStudentToGroup` is deferred (log a warning vs. hard reject TBD in Phase 4 context)

</deferred>

---

*Phase: 03-scheduling-slots-and-views*
*Context gathered: 2026-03-21*
