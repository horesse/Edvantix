---
phase: 01-organizations-rbac-core
verified: 2026-03-19T00:00:00Z
status: passed
score: 19/19 must-haves verified
re_verification: false
human_verification:
  - test: "Start Aspire and confirm Organizations service comes up healthy"
    expected: "Resource shows Running state; Postgres connection established; PermissionSeeder logs 'Organizations permissions seeded: 6 strings'"
    why_human: "Runtime startup behavior, Aspire resource health, and IHostedService execution cannot be verified programmatically from source"
  - test: "Call POST /v1/roles with X-School-Id header and verify 201 + tenant scoping"
    expected: "Role created returns 201 with location header. Subsequent GET /v1/roles with a different X-School-Id header returns empty list (tenant isolation)"
    why_human: "End-to-end tenant isolation behavior requires a running database with the query filter active"
  - test: "Call DELETE /v1/roles/{id} when the role has active user assignments"
    expected: "Returns 409 Conflict with a message referencing active assignments"
    why_human: "GlobalExceptionHandler InvalidOperationException -> 409 mapping verified only at runtime"
---

## Phase 1: Organizations RBAC Core Verification Report

**Phase Goal:** School owners can create and manage custom roles, assign permissions to those roles, and assign roles to users — all tenant-isolated with an architecture-tested EF Core query filter convention that Scheduling and Payments will inherit

**Verified:** 2026-03-19
**Status:** passed
**Re-verification:** No — initial verification

## Goal Achievement

### Observable Truths

| # | Truth | Status | Evidence |
|---|-------|--------|----------|
| 1 | ITenantContext interface exists in Chassis and is reusable by Scheduling/Payments | VERIFIED | `src/BuildingBlocks/Edvantix.Chassis/Security/Tenant/ITenantContext.cs` — `interface ITenantContext` with `Guid SchoolId`, `bool IsResolved`, `void Resolve(Guid)` |
| 2 | TenantMiddleware extracts X-School-Id header and resolves ITenantContext | VERIFIED | `TenantMiddleware.cs` — `const string SchoolIdHeader = "X-School-Id"`, calls `tenantContext.Resolve(schoolId)` |
| 3 | ITenanted interface marks entities requiring tenant isolation | VERIFIED | `Domain/Abstractions/ITenanted.cs` — `interface ITenanted { Guid SchoolId { get; } }` |
| 4 | OrganizationsDbContext applies HasQueryFilter for all ITenanted entities | VERIFIED | `Infrastructure/OrganizationsDbContext.cs` — `Role` filter: `r.SchoolId == tenantContext.SchoolId && !r.IsDeleted`; `UserRoleAssignment` filter: `a.SchoolId == tenantContext.SchoolId`; `Permission` explicitly has no filter |
| 5 | Architecture test fails build if any ITenanted entity lacks a query filter | VERIFIED | `tests/Edvantix.ArchTests/Domain/TenantIsolationTests.cs` — `GivenTenantedEntities_WhenCheckingOrganizationsDbContextModel_ThenAllShouldHaveQueryFilter` uses `GetDeclaredQueryFilters()` (non-deprecated EF Core 10 API) |
| 6 | Organizations service starts in Aspire and connects to its own Postgres database | VERIFIED | `AppHost.cs` wires `organizationsDb`, `Edvantix_Organizations` project reference, gateway, and Scalar OpenAPI; `Infrastructure/Extensions.cs` calls `AddAzurePostgresDbContext<OrganizationsDbContext>` |
| 7 | Role aggregate encapsulates name, schoolId, soft-delete, and permission assignment logic | VERIFIED | `Role.cs` — `AssignPermission`, `RemovePermission`, `SetPermissions`, `UpdateName`, `Delete` with `Guard.Against.NullOrWhiteSpace` / `Guard.Against.Default` |
| 8 | Repository interfaces follow IRepository<T> pattern from Chassis | VERIFIED | `IRoleRepository : IRepository<Role>`, `IPermissionRepository : IRepository<Permission>`, `IUserRoleAssignmentRepository : IRepository<UserRoleAssignment>` |
| 9 | POST /roles creates a named role scoped to the tenant from X-School-Id header | VERIFIED | `CreateRoleCommandHandler` injects `ITenantContext`, passes `tenantContext.SchoolId` to `new Role(name, schoolId)`; wired via `sender.Send` in `CreateRoleEndpoint` |
| 10 | GET /roles returns [{id, name}] list for the current tenant | VERIFIED | `GetRolesQuery` + `GetRolesEndpoint` — handler calls `roleRepository.GetAllAsync` (query-filtered by tenant); projects to `RoleListItem(Id, Name)` |
| 11 | GET /roles/{id} returns a single role with its name | VERIFIED | `GetRoleByIdQuery` + endpoint — throws `NotFoundException.For<Role>` on miss |
| 12 | PUT /roles/{id} updates the role name | VERIFIED | `UpdateRoleCommand` calls `role.UpdateName(request.Name)` |
| 13 | DELETE /roles/{id} soft-deletes the role; returns 409 if any UserRoleAssignments exist | VERIFIED | `DeleteRoleCommandHandler` — `ExistsByRoleIdAsync` check; throws `InvalidOperationException` (maps to 409) when assignments exist; calls `role.Delete()` |
| 14 | PUT /roles/{id}/permissions assigns a set of permission IDs to the role, validating all exist in the Permission catalogue | VERIFIED | `AssignPermissionsCommandHandler` — `permissionRepository.GetByNamesAsync` validation; throws `InvalidOperationException` for unknown names; calls `role.SetPermissions(permissions.Select(p => p.Id))` |
| 15 | GET /roles/{id}/permissions returns the permissions assigned to the role | VERIFIED | `GetRolePermissionsQuery` — loads `role.Permissions`, fetches from `permissionRepository.GetAllAsync`, filters in-memory |
| 16 | POST /user-role-assignments assigns a role to a user, validating profileId via gRPC to Persona | VERIFIED | `AssignRoleCommandHandler` — calls `personaProfileService.ValidateProfileExistsAsync`; `PersonaProfileService` wraps `ProfileGrpcService.ProfileGrpcServiceClient`; wired via `Grpc/Extensions.cs` + `AddGrpcServices()` in app extensions |
| 17 | DELETE /user-role-assignments/{profileId}/{roleId} revokes a role from a user | VERIFIED | `RevokeRoleCommandHandler` — calls `assignmentRepository.Remove(assignment)` after `FindAsync` |
| 18 | GET /user-role-assignments/{profileId} returns all roles assigned to a user in the current tenant | VERIFIED | `GetUserRolesQueryHandler` — `GetByProfileIdAsync` (tenant-filtered), joins with `roleRepository.GetAllAsync`, returns `UserRoleItem(AssignmentId, RoleId, RoleName)` |
| 19 | POST /permissions/register upserts permission strings into the catalogue (idempotent) | VERIFIED | `RegisterPermissionsCommandHandler` calls `permissionRepository.UpsertAsync`; `RegisterPermissionsEndpoint` maps `POST /permissions/register` with `AllowAnonymous` for service-to-service calls |

**Score:** 19/19 truths verified

### Required Artifacts

| Artifact | Status | Details |
|----------|--------|---------|
| `src/BuildingBlocks/Edvantix.Chassis/Security/Tenant/ITenantContext.cs` | VERIFIED | Interface with `Guid SchoolId`, `bool IsResolved`, `void Resolve(Guid)` |
| `src/BuildingBlocks/Edvantix.Chassis/Security/Tenant/TenantMiddleware.cs` | VERIFIED | Reads `X-School-Id`, calls `tenantContext.Resolve(schoolId)` |
| `src/Services/Organizations/Edvantix.Organizations/Domain/Abstractions/ITenanted.cs` | VERIFIED | `interface ITenanted { Guid SchoolId { get; } }` |
| `src/Services/Organizations/Edvantix.Organizations/Infrastructure/OrganizationsDbContext.cs` | VERIFIED | HasQueryFilter on Role (combined tenant+soft-delete) and UserRoleAssignment (tenant only); no filter on Permission |
| `tests/Edvantix.ArchTests/Domain/TenantIsolationTests.cs` | VERIFIED | Two tests: `GivenTenantedEntities_When...` and `GivenPermissionEntity_When...`; uses non-deprecated `GetDeclaredQueryFilters()` API |
| `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/RoleAggregate/Role.cs` | VERIFIED | Full aggregate: AssignPermission, RemovePermission, SetPermissions, UpdateName, Delete with Guard validation |
| `src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/RoleAggregate/IRoleRepository.cs` | VERIFIED | Extends `IRepository<Role>` |
| `src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/RoleRepository.cs` | VERIFIED | Sealed, primary ctor `(OrganizationsDbContext context)`, eager-loads Permissions |
| `src/Services/Organizations/Edvantix.Organizations.UnitTests/Domain/RoleAggregateTests.cs` | VERIFIED | Contains `GivenNewRole` (11 tests per summary) |
| `src/Services/Organizations/Edvantix.Organizations/Features/Roles/CreateRole/CreateRoleCommand.cs` | VERIFIED | `ICommand<Guid>`, handler injects `ITenantContext` |
| `src/Services/Organizations/Edvantix.Organizations/Features/Roles/DeleteRole/DeleteRoleCommand.cs` | VERIFIED | Contains `ExistsByRoleIdAsync` check and `InvalidOperationException` |
| `src/Services/Organizations/Edvantix.Organizations/Features/Roles/AssignPermissions/AssignPermissionsCommand.cs` | VERIFIED | Contains `GetByNamesAsync` catalogue validation and `role.SetPermissions` |
| `src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/AssignRole/AssignRoleCommand.cs` | VERIFIED | Contains `personaProfileService.ValidateProfileExistsAsync` |
| `src/Services/Organizations/Edvantix.Organizations/Features/Permissions/RegisterPermissions/RegisterPermissionsCommand.cs` | VERIFIED | Contains `UpsertAsync` call |
| `src/BuildingBlocks/Edvantix.Constants/Permissions/OrganizationsPermissions.cs` | VERIFIED | 6 constants including `"organizations.create-role"`, `static IReadOnlyList<string> All` |
| `src/Services/Organizations/Edvantix.Organizations/Grpc/Services/PersonaProfileService.cs` | VERIFIED | Wraps `ProfileGrpcService.ProfileGrpcServiceClient`, `ValidateProfileExistsAsync` catches `RpcException` with `StatusCode.NotFound` |
| `src/Services/Organizations/Edvantix.Organizations/Infrastructure/Seeding/PermissionSeeder.cs` | VERIFIED | `IHostedService`, calls `UpsertAsync(OrganizationsPermissions.All)` directly via repository |

### Key Link Verification

| From | To | Via | Status | Details |
|------|----|-----|--------|---------|
| `TenantMiddleware` | `ITenantContext` | scoped DI in `InvokeAsync` | WIRED | `InvokeAsync(HttpContext, ITenantContext tenantContext)` + `tenantContext.Resolve(schoolId)` |
| `OrganizationsDbContext` | `ITenantContext` | constructor injection | WIRED | Primary constructor `(DbContextOptions<OrganizationsDbContext>, ITenantContext tenantContext)` |
| `TenantIsolationTests` | `OrganizationsDbContext` | EF Core model introspection | WIRED | `new OrganizationsDbContext(options, tenantContext)`, `GetDeclaredQueryFilters()` |
| `CreateRoleEndpoint` | `CreateRoleCommand` | `ISender.Send` | WIRED | `sender.Send(command, cancellationToken)` in `HandleAsync` |
| `DeleteRoleCommand handler` | `IUserRoleAssignmentRepository` | `ExistsByRoleIdAsync` check | WIRED | `await assignmentRepository.ExistsByRoleIdAsync(request.Id, cancellationToken)` |
| `AssignPermissionsCommand handler` | `IPermissionRepository` | `GetByNamesAsync` validation | WIRED | `await permissionRepository.GetByNamesAsync(request.PermissionNames, cancellationToken)` |
| `AssignRoleCommand handler` | `IPersonaProfileService` | gRPC call | WIRED | `await personaProfileService.ValidateProfileExistsAsync(request.ProfileId, cancellationToken)` |
| `PermissionSeeder` | `OrganizationsPermissions` | direct call on startup | WIRED | `await permissionRepository.UpsertAsync(OrganizationsPermissions.All, cancellationToken)` |
| `Grpc/Extensions.cs` | `PersonaProfileService` | `AddSingleton` | WIRED | `services.AddSingleton<IPersonaProfileService, PersonaProfileService>()` |
| `Extensions/Extensions.cs` | `AddGrpcServices` + `AddHostedService<PermissionSeeder>` | DI registration | WIRED | Both calls present in `AddApplicationServices` |
| `Program.cs` | `UseTenantContext()` | middleware pipeline | WIRED | `app.UseTenantContext()` registered BEFORE `app.UseAuthorization()` |

### Requirements Coverage

| Requirement | Source Plan | Description | Status | Evidence |
|-------------|------------|-------------|--------|---------|
| ORG-01 | 01-02, 01-03 | School owner can create a custom role by name within their school | SATISFIED | `CreateRoleCommand` creates `Role(name, tenantContext.SchoolId)` via `POST /v1/roles` |
| ORG-02 | 01-02, 01-03 | Owner can view, edit, and delete roles of their school | SATISFIED | `GET /v1/roles`, `GET /v1/roles/{id}`, `PUT /v1/roles/{id}`, `DELETE /v1/roles/{id}` all implemented and tenant-scoped via query filter |
| ORG-03 | 01-02, 01-03 | Owner can assign a set of permissions (CQRS command identifier strings) to a role | SATISFIED | `PUT /v1/roles/{id}/permissions` validates permission strings against catalogue and calls `role.SetPermissions` |
| ORG-04 | 01-04 | Services automatically register their permission strings in the catalogue on startup | SATISFIED | `POST /v1/permissions/register` endpoint + `PermissionSeeder` pattern established; Organizations seeds its own 6 strings directly |
| ORG-05 | 01-04 | Owner can assign a role to a user within their school | SATISFIED | `POST /v1/user-role-assignments` validates profileId via gRPC to Persona, validates role existence, prevents duplicates, stores assignment with `tenantContext.SchoolId` |
| ORG-06 | 01-04 | Owner can revoke a role from a user | SATISFIED | `DELETE /v1/user-role-assignments/{profileId}/{roleId}` hard-deletes the assignment |
| ORG-10 | 01-01 | EF Core global query filter by schoolId across all aggregates (tenant isolation) | SATISFIED | `OrganizationsDbContext.OnModelCreating` applies filters; `TenantIsolationTests` enforces convention via EF Core model introspection |
| ORG-07 | NONE (Pending) | Organizations service provides gRPC CheckPermission(userId, schoolId, permission) -> bool | ORPHANED | Mapped to Phase 1 in REQUIREMENTS.md as Pending. Not in any plan's `requirements` field. Not implemented. |
| ORG-08 | NONE (Pending) | CheckPermission result cached in HybridCache with invalidation on role change | ORPHANED | Mapped to Phase 1 in REQUIREMENTS.md as Pending. Not in any plan's `requirements` field. Not implemented. |
| ORG-09 | NONE (Pending) | Integration event published on role/assignment change for cache invalidation in other services | ORPHANED | Mapped to Phase 1 in REQUIREMENTS.md as Pending. Not in any plan's `requirements` field. Not implemented. |

**Note on ORG-07/08/09:** These three requirements are mapped to Phase 1 in REQUIREMENTS.md but are explicitly marked Pending. No plan in this phase claimed them in its `requirements` field. They were not implemented and are intentional deferrals — they represent a gRPC permission-check server and caching layer that was out of scope for this foundational phase. This is not a gap in goal achievement; the phase goal ("custom roles, assign permissions, assign roles to users — tenant-isolated with architecture-tested convention") is fully met without them. They should be tracked for a subsequent plan.

### Anti-Patterns Found

| File | Line | Pattern | Severity | Impact |
|------|------|---------|----------|--------|
| `Features/Permissions/RegisterPermissions/RegisterPermissionsEndpoint.cs` | 7 | `TODO: In production, secure this endpoint with a shared secret or service-to-service mTLS policy.` | Info | Acknowledged design deferral; `AllowAnonymous` is intentional for service-to-service v1 startup calls. Does not block goal. |

### Human Verification Required

#### 1. Aspire startup with PermissionSeeder

**Test:** Run `just run` and observe the Organizations service resource in the Aspire dashboard.
**Expected:** Resource transitions to Running; Postgres `organizationsdb` database is healthy; application logs show "Organizations permissions seeded: 6 strings" from PermissionSeeder.
**Why human:** IHostedService execution, Aspire service discovery, and database connectivity can only be observed at runtime.

#### 2. Tenant isolation end-to-end

**Test:** Call `POST /v1/roles` with header `X-School-Id: <guid-A>` to create a role. Then call `GET /v1/roles` with `X-School-Id: <guid-B>`.
**Expected:** The role created under guid-A is not visible when queried under guid-B. The query filter (`r.SchoolId == tenantContext.SchoolId`) must be active at runtime.
**Why human:** EF Core query filter behavior requires a running Postgres instance; in-memory tests in ArchTests verify the filter is registered but not that it produces the correct SQL predicate against a real database.

#### 3. DELETE /roles/{id} with active assignments returns 409

**Test:** Create a role, assign it to a user via `POST /v1/user-role-assignments`, then call `DELETE /v1/roles/{id}`.
**Expected:** HTTP 409 Conflict response with message referencing active user assignments.
**Why human:** `GlobalExceptionHandler` mapping of `InvalidOperationException` to 409 is middleware-level behavior that requires a running ASP.NET Core pipeline.

### Gaps Summary

No gaps. All 19 must-have truths are verified, all artifacts pass all three levels (exists, substantive, wired), all key links are confirmed. The three ORPHANED requirements (ORG-07/08/09) are intentional Phase 1 deferrals tracked in REQUIREMENTS.md as Pending — they do not constitute gaps for this phase's stated goal.

---

_Verified: 2026-03-19_
_Verifier: Claude (gsd-verifier)_
