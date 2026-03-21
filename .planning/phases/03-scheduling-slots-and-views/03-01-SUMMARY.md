---
phase: 03-scheduling-slots-and-views
plan: "01"
subsystem: scheduling
tags: [scheduling, aspire, keycloak, grpc, permissions, efcore]
dependency_graph:
  requires: []
  provides:
    - Edvantix.Scheduling service project (compiles, Aspire-visible)
    - SchedulingPermissions constants (4 permissions)
    - SchedulingDbContext shell
    - PermissionSeeder via HTTP to Organizations
    - Scheduling Keycloak client in realm JSON
  affects:
    - src/Aspire/Edvantix.AppHost/AppHost.cs
    - src/Aspire/Edvantix.AppHost/Container/keycloak/realms/EdvantixRealms.json
    - src/BuildingBlocks/Edvantix.Constants/Aspire/Services.cs
    - src/BuildingBlocks/Edvantix.Constants/Aspire/Components.cs
tech_stack:
  added:
    - Microsoft.Extensions.Http.Resilience (retry for PermissionSeeder HTTP client)
  patterns:
    - IHostedService PermissionSeeder via IHttpClientFactory HTTP POST
    - gRPC client copy pattern (persona proto + IPersonaProfileService wrapper)
    - AddStandardResilienceHandler on named HttpClient for startup resilience
key_files:
  created:
    - src/Services/Scheduling/Edvantix.Scheduling/Edvantix.Scheduling.csproj
    - src/Services/Scheduling/Edvantix.Scheduling/ISchedulingApiMarker.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Program.cs
    - src/Services/Scheduling/Edvantix.Scheduling/GlobalUsings.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Extensions/Extensions.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/Extensions.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/SchedulingDbContext.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/SchedulingDbContextFactory.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/Seeding/PermissionSeeder.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Configurations/SchedulingAppSettings.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Extensions.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Services/IPersonaProfileService.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Services/PersonaProfileService.cs
    - src/Services/Scheduling/Edvantix.Scheduling/Grpc/Protos/persona/v1/profile.proto
    - src/BuildingBlocks/Edvantix.Constants/Permissions/SchedulingPermissions.cs
  modified:
    - src/BuildingBlocks/Edvantix.Constants/Aspire/Services.cs
    - src/BuildingBlocks/Edvantix.Constants/Aspire/Components.cs
    - src/Aspire/Edvantix.AppHost/AppHost.cs
    - src/Aspire/Edvantix.AppHost/Edvantix.AppHost.csproj
    - src/Aspire/Edvantix.AppHost/Container/keycloak/realms/EdvantixRealms.json
    - Edvantix.slnx
decisions:
  - "[03-01]: PermissionSeeder uses IHttpClientFactory HTTP POST to Organizations (not direct DB) — Scheduling does not own the Permission aggregate; uses AddStandardResilienceHandler for startup-ordering resilience"
  - "[03-01]: No MapGrpcService in Program.cs — Scheduling is a pure gRPC client (Persona), not a server"
  - "[03-01]: Microsoft.Extensions.Http.Resilience used instead of raw Polly — already in Directory.Packages.props, provides standard retry + circuit breaker pattern"
  - "[03-01]: SchedulingDbContext has no DbSets — added as empty shell; domain entities and HasQueryFilter calls deferred to Plan 02 when LessonSlot entity is created"
metrics:
  duration_minutes: 18
  completed_date: "2026-03-21"
  tasks_completed: 2
  files_created: 15
  files_modified: 6
---

## Summary

Scheduling service skeleton bootstrapped: compiling .NET 10 web service visible as Aspire resource with Keycloak client, Persona gRPC client, Organizations HTTP permission seeder (4 scheduling.* permissions), and empty SchedulingDbContext shell ready for Plan 02 domain entities.

## Tasks Completed

| Task | Name | Commit | Files |
|------|------|--------|-------|
| 1 | Create Scheduling service project, constants, settings, and Program.cs | 6769a94 | 16 files |
| 2 | Aspire wiring, Keycloak client, and gRPC Persona client setup | a44a798 | 8 files |

## Verification Results

1. `dotnet build src/Services/Scheduling/Edvantix.Scheduling/Edvantix.Scheduling.csproj` — PASSED (0 errors, 0 warnings)
2. `dotnet build src/Aspire/Edvantix.AppHost/Edvantix.AppHost.csproj` — PASSED (0 errors, 0 warnings)
3. `grep -c "scheduling\." SchedulingPermissions.cs` — returns 5 (4 const declarations + 1 comment); 4 distinct permission strings

## Deviations from Plan

### Auto-fixed Issues

None — plan executed exactly as written.

### Minor Adjustments

**1. Microsoft.Extensions.Http.Resilience instead of raw Polly**
- **Found during:** Task 1 — checking Directory.Packages.props
- **Issue:** Plan suggested Polly retry for PermissionSeeder HTTP client; raw `Polly` package not in Directory.Packages.props
- **Fix:** Used `Microsoft.Extensions.Http.Resilience` (already managed in Directory.Packages.props) with `AddStandardResilienceHandler()` — provides retry + circuit breaker with one line
- **Impact:** Same resilience outcome, consistent with project package management

**2. SchedulingDbContext uses `_ = tenantContext`**
- **Found during:** Task 1 — `tenantContext` is a primary constructor parameter needed in Plan 02 but unused in the shell
- **Fix:** Added `_ = tenantContext;` in `OnModelCreating` with comment noting filters come in Plan 02; avoids CS8618 and TreatWarningsAsErrors failure

## Known Stubs

- `SchedulingDbContext` has no `DbSet<>` properties — intentional per plan; LessonSlot domain entity and query filters are added in Plan 02
- No API endpoints yet — Plan 04/05 add scheduling features

## Self-Check: PASSED

| Item | Status |
|------|--------|
| src/Services/Scheduling/Edvantix.Scheduling/Edvantix.Scheduling.csproj | FOUND |
| src/BuildingBlocks/Edvantix.Constants/Permissions/SchedulingPermissions.cs | FOUND |
| src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/SchedulingDbContext.cs | FOUND |
| src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/Seeding/PermissionSeeder.cs | FOUND |
| Commit 6769a94 (Task 1) | FOUND |
| Commit a44a798 (Task 2) | FOUND |
