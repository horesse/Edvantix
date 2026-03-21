---
phase: 02-organizations-permission-cache
plan: "01"
subsystem: api
tags: [grpc, protobuf, hybridcache, redis, permissions, rbac, caching]

requires:
  - phase: 01-organizations-rbac-core
    provides: Role, UserRoleAssignment, Permission domain aggregates + EF Core repositories

provides:
  - gRPC CheckPermission(userId, schoolId, permission) endpoint with HybridCache L1+L2 caching
  - GET /permissions?userId&schoolId REST endpoint for cache priming
  - HybridCache configured with L1=30s (in-memory) + L2=5min (Redis) TTLs
  - Redis wired to Organizations service via AppHost

affects:
  - 02-02 (gRPC client integration in downstream services)
  - 02-03 (cache invalidation on role assignment changes)

tech-stack:
  added:
    - permissions.proto (edition 2023, csharp_namespace Edvantix.Organizations.Grpc.Generated)
    - Grpc.AspNetCore.Server.ClientFactory (Server-side gRPC already in .csproj)
    - Microsoft.Extensions.Caching.Hybrid (via AddCaching extension)
  patterns:
    - gRPC server namespace separated from service class namespace to avoid naming conflict (Generated vs Services)
    - Repository methods with IgnoreQueryFilters() + explicit schoolId filter for gRPC paths without ambient tenant context
    - HybridCache key pattern: perm:{userId}:{schoolId}:{permission}, tag: user:{userId}:{schoolId}
    - Query naming: GetUserPermissionGrantQuery (not CheckPermissionQuery) to comply with arch rule requiring Get/List/Visualize/Summarize prefix

key-files:
  created:
    - src/Services/Organizations/Edvantix.Organizations/Grpc/Protos/organizations/v1/permissions.proto
    - src/Services/Organizations/Edvantix.Organizations/Grpc/Services/PermissionsGrpcService.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Permissions/CheckPermission/CheckPermissionQuery.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Permissions/CheckPermission/GetPermissionsEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/Permissions/CheckPermissionQueryHandlerTests.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/Permissions/PermissionsGrpcServiceTests.cs
  modified:
    - src/Services/Organizations/Edvantix.Organizations/Edvantix.Organizations.csproj
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/UserRoleAssignmentAggregate/IUserRoleAssignmentRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/RoleAggregate/IRoleRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/UserRoleAssignmentRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/RoleRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Extensions/Extensions.cs
    - src/Services/Organizations/Edvantix.Organizations/Program.cs
    - src/Services/Organizations/Edvantix.Organizations/appsettings.json
    - src/Services/Organizations/Edvantix.Organizations/appsettings.Development.json
    - src/Aspire/Edvantix.AppHost/AppHost.cs

key-decisions:
  - "gRPC csharp_namespace set to Edvantix.Organizations.Grpc.Generated (not Services) to avoid class name conflict between generated PermissionsGrpcService and the concrete implementation class"
  - "GetBySchoolAsync added to IRoleRepository (bypasses tenant filter) — needed because gRPC calls lack ambient ITenantContext; explicit schoolId filter preserves data isolation"
  - "GetUserPermissionGrantQuery (not CheckPermissionQuery) to comply with arch rule requiring Get/List prefix on IQuery types"
  - "GetPermissionsEndpoint uses inline lambda (not typed IEndpoint<T,R>) because query params (userId, schoolId) don't map cleanly to the typed HandleAsync signature"

patterns-established:
  - "gRPC-to-mediator pattern: PermissionsGrpcService parses Guids from string proto fields, dispatches IQuery via IMediator"
  - "Tenant filter bypass pattern: repository methods with IgnoreQueryFilters() + explicit predicate for cross-service calls"
  - "Permission cache key: perm:{userId}:{schoolId}:{permission} — one entry per permission check; tag user:{userId}:{schoolId} for bulk invalidation"

requirements-completed: [ORG-07, ORG-08]

duration: 9min
completed: 2026-03-21
---

# Phase 02 Plan 01: Organizations gRPC Permission Cache Summary

**gRPC CheckPermission endpoint with HybridCache (L1 in-memory 30s + L2 Redis 5min) and GET /permissions REST priming endpoint for the Organizations service**

## Performance

- **Duration:** 9 min
- **Started:** 2026-03-21T06:35:38Z
- **Completed:** 2026-03-21T06:44:59Z
- **Tasks:** 2
- **Files modified:** 19

## Accomplishments

- gRPC `CheckPermission(userId, schoolId, permission)` server endpoint that resolves whether a user holds a permission via their assigned roles, with HybridCache caching the result
- `GET /permissions?userId&schoolId` REST endpoint for cache priming, querying DB directly and returning all permission strings for a user in a school
- Redis wired to Organizations via AppHost (`.WithReference(redis).WaitFor(redis)`)
- HybridCache configured with L1=30s (in-memory) and L2=5min (Redis) TTLs via `AddCaching()` in Extensions
- 6 new unit tests covering: permission found, not found, cache key/tag format, no assignments, gRPC delegation
- All 47 unit tests and 64 architecture tests pass

## Task Commits

Each task was committed atomically:

1. **Task 1: Proto, gRPC server, CheckPermissionQuery with HybridCache, unit tests (TDD)** - `a22a91f` (feat)
2. **Task 2: HybridCache wiring, AppHost redis, gRPC mapping, GET /permissions endpoint** - `d0d219d` (feat)

## Files Created/Modified

- `Grpc/Protos/organizations/v1/permissions.proto` - gRPC contract for CheckPermission RPC
- `Grpc/Services/PermissionsGrpcService.cs` - gRPC server implementation, delegates to IMediator
- `Features/Permissions/CheckPermission/CheckPermissionQuery.cs` - GetUserPermissionGrantQuery + handler using IHybridCache, GetBySchoolAsync, GetByProfileAndSchoolAsync
- `Features/Permissions/CheckPermission/GetPermissionsEndpoint.cs` - GET /permissions endpoint + GetUserPermissionsQuery handler
- `Edvantix.Organizations.csproj` - added Protobuf Server entry for permissions.proto
- `IUserRoleAssignmentRepository.cs` - added GetByProfileAndSchoolAsync (bypasses tenant filter)
- `IRoleRepository.cs` - added GetBySchoolAsync (bypasses tenant filter, explicit schoolId)
- `UserRoleAssignmentRepository.cs` - implements GetByProfileAndSchoolAsync with IgnoreQueryFilters()
- `RoleRepository.cs` - implements GetBySchoolAsync with IgnoreQueryFilters() + Include(Permissions)
- `Extensions.cs` - AddCaching() with L1=30s, L2=5min TTLs
- `Program.cs` - MapGrpcService<PermissionsGrpcService>()
- `appsettings.json` + `appsettings.Development.json` - added Caching section
- `AppHost.cs` - added WithReference(redis) + WaitFor(redis) to organizationsApi chain
- `CheckPermissionQueryHandlerTests.cs` - 4 handler tests
- `PermissionsGrpcServiceTests.cs` - 2 gRPC service tests

## Decisions Made

- **gRPC namespace separation:** `csharp_namespace = "Edvantix.Organizations.Grpc.Generated"` avoids a naming conflict between the generated `PermissionsGrpcService` class and the concrete implementation in `Grpc.Services`.
- **Tenant filter bypass:** Added `GetBySchoolAsync` to `IRoleRepository` and `GetByProfileAndSchoolAsync` to `IUserRoleAssignmentRepository` using `IgnoreQueryFilters()` with explicit `schoolId` predicate. gRPC calls don't pass through `TenantMiddleware`, so the ambient `ITenantContext` is empty.
- **Query naming:** Renamed `CheckPermissionQuery` to `GetUserPermissionGrantQuery` to comply with the existing architecture rule that all `IQuery` types must start with Get/List/Visualize/Summarize.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Renamed CheckPermissionQuery to GetUserPermissionGrantQuery**
- **Found during:** Task 2 verification (architecture tests)
- **Issue:** `CheckPermissionQuery` failed arch test `GivenQueries_WhenQueryHandler_ThenShouldContainListOrGetOrVisualizeOrSummarize` — the project enforces naming conventions on IQuery types
- **Fix:** Renamed to `GetUserPermissionGrantQuery` and `GetUserPermissionGrantQueryHandler` throughout; updated gRPC service and all test files
- **Files modified:** CheckPermissionQuery.cs, PermissionsGrpcService.cs, CheckPermissionQueryHandlerTests.cs, PermissionsGrpcServiceTests.cs
- **Verification:** Architecture tests pass (64/64)
- **Committed in:** d0d219d (Task 2 commit)

**2. [Rule 2 - Missing Critical] Added IRoleRepository.GetBySchoolAsync**
- **Found during:** Task 1 implementation
- **Issue:** Plan specified using `IRoleRepository.FindByIdAsync` but that applies tenant query filter — gRPC context has no ambient tenant, so it would silently return no roles
- **Fix:** Added `GetBySchoolAsync(schoolId)` to `IRoleRepository` and implementation using `IgnoreQueryFilters().Where(schoolId && !IsDeleted)`; handler uses this instead of per-role FindByIdAsync
- **Files modified:** IRoleRepository.cs, RoleRepository.cs, CheckPermissionQuery.cs
- **Verification:** Unit tests pass; DB isolation maintained via explicit schoolId filter
- **Committed in:** a22a91f (Task 1 commit)

---

**Total deviations:** 2 auto-fixed (1 bug/naming, 1 missing critical)
**Impact on plan:** Both fixes necessary for correctness. No scope creep.

## Issues Encountered

- Proto `csharp_namespace` naming conflict discovered during implementation — `Edvantix.Organizations.Grpc.Services` would make the generated base class ambiguous with the concrete class. Resolved by using `Edvantix.Organizations.Grpc.Generated` namespace in the proto file.

## Next Phase Readiness

- gRPC CheckPermission server is live and compiled. Downstream services (Scheduling, Payments) can copy the proto and create gRPC clients.
- Cache invalidation (tag-based RemoveByTagAsync) is ready to be triggered from AssignRole/RevokeRole command handlers in phase 02-02.
- GET /permissions priming endpoint is available for downstream service startup registration.

---
*Phase: 02-organizations-permission-cache*
*Completed: 2026-03-21*
