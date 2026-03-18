---
phase: 01-organizations-rbac-core
plan: "01"
subsystem: infra
tags: [aspire, efcore, multitenancy, archunit, postgres, masstransit, mediator]

# Dependency graph
requires: []
provides:
  - ITenantContext interface in Chassis (reusable by Scheduling, Payments)
  - TenantMiddleware resolving X-School-Id header into scoped tenant context
  - ITenanted domain marker interface for tenant-scoped entities
  - Organizations service project skeleton (compiles, wired in Aspire)
  - OrganizationsDbContext with combined tenant+soft-delete filter on Role, tenant filter on UserRoleAssignment, no filter on Permission
  - Architecture test enforcing all ITenanted entities have query filters
  - Organizations.UnitTests project scaffold
affects:
  - 01-02 (domain commands, validators — builds on this skeleton)
  - 01-03 (query endpoints — uses DbContext and ITenantContext)
  - 01-04 (Persona gRPC client — uses IOrganizationsApiMarker)
  - Scheduling, Payments phases (reuse ITenantContext from Chassis)

# Tech tracking
tech-stack:
  added:
    - Microsoft.EntityFrameworkCore.InMemory (test only, for ArchTests DbContext inspection)
  patterns:
    - Tenant isolation via combined HasQueryFilter (tenant + soft-delete in single EF Core expression)
    - Tenant context resolved in middleware BEFORE UseAuthorization for filter availability in handlers
    - Query filters set in DbContext.OnModelCreating (not in IEntityTypeConfiguration) because ITenantContext injection is not available inside ApplyConfigurationsFromAssembly
    - Architecture tests using EF Core model introspection (GetDeclaredQueryFilters) to enforce conventions

key-files:
  created:
    - src/BuildingBlocks/Edvantix.Chassis/Security/Tenant/ITenantContext.cs
    - src/BuildingBlocks/Edvantix.Chassis/Security/Tenant/TenantContext.cs
    - src/BuildingBlocks/Edvantix.Chassis/Security/Tenant/TenantMiddleware.cs
    - src/BuildingBlocks/Edvantix.Chassis/Security/Tenant/TenantServiceExtensions.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/Abstractions/ITenanted.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/OrganizationsDbContext.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/OrganizationsDbContextFactory.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/RoleConfiguration.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/PermissionConfiguration.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/RolePermissionConfiguration.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EntityConfigurations/UserRoleAssignmentConfiguration.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/RoleAggregate/Role.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/RoleAggregate/RolePermission.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/PermissionAggregate/Permission.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/UserRoleAssignmentAggregate/UserRoleAssignment.cs
    - src/Services/Organizations/Edvantix.Organizations/Program.cs
    - src/Services/Organizations/Edvantix.Organizations/Extensions/Extensions.cs
    - src/Services/Organizations/Edvantix.Organizations/Configurations/OrganizationsAppSettings.cs
    - tests/Edvantix.ArchTests/Domain/TenantIsolationTests.cs
  modified:
    - src/BuildingBlocks/Edvantix.Constants/Aspire/Services.cs (added Organizations)
    - src/BuildingBlocks/Edvantix.Constants/Aspire/Components.cs (added Organizations database)
    - src/Aspire/Edvantix.AppHost/AppHost.cs (wired organizationsApi)
    - src/Aspire/Edvantix.AppHost/Edvantix.AppHost.csproj (added project reference)
    - Edvantix.slnx (added Organizations folders)
    - tests/Edvantix.ArchTests/Abstractions/BaseTest.cs (added OrganizationsAssembly)
    - tests/Edvantix.ArchTests/Abstractions/ArchUnitBaseTest.cs (added Organizations to LoadAssemblies and GetServiceTypes)
    - tests/Edvantix.ArchTests/Edvantix.ArchTests.csproj (added EF InMemory + Organizations reference)
    - Directory.Packages.props (added Microsoft.EntityFrameworkCore.InMemory)

key-decisions:
  - "EF Core HasQueryFilter for tenanted entities is set in DbContext.OnModelCreating rather than in IEntityTypeConfiguration to enable ITenantContext injection via constructor — IEntityTypeConfiguration constructors cannot receive scoped DI services"
  - "Role HasQueryFilter combines tenant AND soft-delete in a single expression because EF Core supports only one HasQueryFilter per entity type (second call overwrites first)"
  - "ITenanted lives in Organizations domain (first consumer) not in Chassis — Chassis is infrastructure, not domain; moved to SharedKernel when Scheduling needs it"
  - "Permission has no HasQueryFilter — it is a global catalogue shared across all schools"
  - "TenantMiddleware called BEFORE UseAuthorization in Program.cs pipeline so tenant context is available in authorization handlers and DbContext filters"
  - "Architecture test uses EF Core GetDeclaredQueryFilters() instead of deprecated GetQueryFilter() API"

patterns-established:
  - "Tenant filter pattern: ITenanted entities must have HasQueryFilter in DbContext.OnModelCreating referencing ITenantContext.SchoolId"
  - "Combined filter pattern: soft-deletable tenanted entities use single HasQueryFilter with && joining both conditions"
  - "Convention test pattern: use EF Core model introspection (GetDeclaredQueryFilters) not ArchUnitNET IL scanning for infrastructure-level conventions"

requirements-completed:
  - ORG-10

# Metrics
duration: 15min
completed: "2026-03-19"
---

# Phase 1 Plan 01: Organizations Service Scaffold + Tenant Isolation Summary

**ITenantContext in Chassis with TenantMiddleware, OrganizationsDbContext applying combined tenant+soft-delete query filters, and architecture test enforcing the ITenanted convention**

## Performance

- **Duration:** ~15 min
- **Started:** 2026-03-19T00:19:58Z
- **Completed:** 2026-03-19T00:34:26Z
- **Tasks:** 2
- **Files modified:** 38

## Accomplishments

- Created `ITenantContext` / `TenantContext` / `TenantMiddleware` / `TenantServiceExtensions` in Chassis — reusable by all future services
- Scaffolded Organizations service with full project structure, EF Core DbContext (combined tenant+soft-delete filter on Role, tenant-only on UserRoleAssignment, none on Permission), Aspire wiring, gateway, Scalar, and solution file
- Added `TenantIsolationTests` architecture test that uses EF Core model introspection to enforce all `ITenanted` entities have query filters — 64/64 tests pass

## Task Commits

Each task was committed atomically:

1. **Task 1: ITenantContext in Chassis + Organizations project scaffold + Aspire wiring** - `c82d7ca` (feat)
2. **Task 2: Architecture test for tenant isolation + Organizations unit test project scaffold** - `f705f22` (feat)

## Files Created/Modified

Key files (38 total):

- `src/BuildingBlocks/Edvantix.Chassis/Security/Tenant/ITenantContext.cs` — Scoped tenant context interface
- `src/BuildingBlocks/Edvantix.Chassis/Security/Tenant/TenantMiddleware.cs` — Resolves X-School-Id header
- `src/Services/Organizations/Edvantix.Organizations/Domain/Abstractions/ITenanted.cs` — Tenant marker interface
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/OrganizationsDbContext.cs` — EF Core DbContext with query filters
- `tests/Edvantix.ArchTests/Domain/TenantIsolationTests.cs` — Convention enforcement test

## Decisions Made

- `HasQueryFilter` for tenanted entities set in `DbContext.OnModelCreating` (not in `IEntityTypeConfiguration`) because scoped services cannot be injected into configuration constructors
- `Role` uses a single combined expression `r.SchoolId == tenantContext.SchoolId && !r.IsDeleted` — EF Core allows only one `HasQueryFilter` per entity, so combining avoids the second call silently overwriting the first
- `ITenanted` placed in Organizations domain (not Chassis) — Chassis is infrastructure; domain interfaces belong with their first consumer

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Used GetDeclaredQueryFilters() instead of deprecated GetQueryFilter()**
- **Found during:** Task 2 (architecture test implementation)
- **Issue:** Plan used `entityType.GetQueryFilter()` which is marked `[Obsolete]` in EF Core 10 — compiler error with TreatWarningsAsErrors
- **Fix:** Replaced with `entityType.GetDeclaredQueryFilters()` returning `IEnumerable<IQueryFilter>`; simplified assertion to check non-empty collection rather than string-inspecting the expression body (EF Core renders `IQueryFilter.ToString()` as type name, not expression)
- **Files modified:** `tests/Edvantix.ArchTests/Domain/TenantIsolationTests.cs`
- **Verification:** 64/64 tests pass including 2 new TenantIsolation tests
- **Committed in:** f705f22

**2. [Rule 2 - Missing Critical] Added EventMapper stub to Organizations service**
- **Found during:** Task 1 (Extensions.cs implementation)
- **Issue:** Persona's Extensions.cs registers `IEventMapper` with a concrete `EventMapper` — Organizations needed the same registration pattern but had no mapper yet
- **Fix:** Created `Infrastructure/EventServices/EventMapper.cs` with a stub that throws `ArgumentOutOfRangeException` until domain events are defined in Plan 02
- **Files modified:** `src/Services/Organizations/Edvantix.Organizations/Infrastructure/EventServices/EventMapper.cs`
- **Verification:** Build succeeds, compile-time error resolved
- **Committed in:** c82d7ca

---

**Total deviations:** 2 auto-fixed (1 bug — obsolete API, 1 missing critical — required DI registration)
**Impact on plan:** Both necessary for compilation and correctness. No scope creep.

## Issues Encountered

- UnitTests project had to be created before Task 1 commit because the solution file referenced it — the pre-commit hook's analyzer task failed on first attempt when the project was in `Edvantix.slnx` but didn't exist on disk. Fixed by creating the UnitTests scaffold before committing Task 1.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- Organizations service compiles and is wired into Aspire AppHost — Plan 02 can add commands, repositories, and feature handlers immediately
- `ITenantContext` in Chassis is ready for reuse by Scheduling and Payments services
- Architecture test convention is live — any new `ITenanted` entity without a query filter will fail the build

---
*Phase: 01-organizations-rbac-core*
*Completed: 2026-03-19*
