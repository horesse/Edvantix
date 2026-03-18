---
phase: 01-organizations-rbac-core
plan: "04"
subsystem: api
tags: [grpc, cqrs, mediator, efcore, permissions, rbac, seeding]

requires:
  - phase: 01-03-SUMMARY.md
    provides: UserRoleAssignment aggregate, IUserRoleAssignmentRepository, IPermissionRepository, Role aggregate

provides:
  - POST /permissions/register — idempotent upsert of permission strings for any service
  - POST /user-role-assignments — assign role to user with Persona gRPC profileId validation
  - DELETE /user-role-assignments/{profileId}/{roleId} — revoke role (hard-delete)
  - GET /user-role-assignments/{profileId} — list all roles for a user in current tenant
  - PermissionSeeder IHostedService — seeds Organizations' own permissions directly on startup
  - IPersonaProfileService / PersonaProfileService — gRPC client wrapper for profile validation
  - OrganizationsPermissions static constants in Edvantix.Constants

affects:
  - Phase 02 (Organizations gRPC server)
  - Phase 03 (Scheduling — will call POST /permissions/register to register its own permissions)
  - Phase 05 (Payments — same pattern for permission registration)

tech-stack:
  added:
    - Grpc.Tools (protobuf codegen)
    - Grpc.AspNetCore.Server.ClientFactory (gRPC service discovery + health)
  patterns:
    - PermissionSeeder pattern: IHostedService for direct-DB seed (avoids self-HTTP race condition)
    - gRPC client wrapper: interface abstraction over generated client for testability
    - POST /permissions/register with AllowAnonymous for service-to-service startup calls

key-files:
  created:
    - src/BuildingBlocks/Edvantix.Constants/Permissions/OrganizationsPermissions.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Permissions/RegisterPermissions/RegisterPermissionsCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Permissions/RegisterPermissions/RegisterPermissionsEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Permissions/RegisterPermissions/RegisterPermissionsValidator.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/Seeding/PermissionSeeder.cs
    - src/Services/Organizations/Edvantix.Organizations/Grpc/Protos/persona/v1/profile.proto
    - src/Services/Organizations/Edvantix.Organizations/Grpc/Extensions.cs
    - src/Services/Organizations/Edvantix.Organizations/Grpc/Services/IPersonaProfileService.cs
    - src/Services/Organizations/Edvantix.Organizations/Grpc/Services/PersonaProfileService.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/AssignRole/AssignRoleCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/AssignRole/AssignRoleEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/AssignRole/AssignRoleValidator.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/RevokeRole/RevokeRoleCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/RevokeRole/RevokeRoleEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/GetUserRoles/GetUserRolesQuery.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/GetUserRoles/GetUserRolesEndpoint.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/RegisterPermissionsCommandHandlerTests.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/AssignRoleCommandHandlerTests.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/RevokeRoleCommandHandlerTests.cs
  modified:
    - src/Services/Organizations/Edvantix.Organizations/Edvantix.Organizations.csproj
    - src/Services/Organizations/Edvantix.Organizations/Extensions/Extensions.cs

key-decisions:
  - "PermissionSeeder uses IHostedService with direct repository access (not HTTP) to avoid self-calling race condition on startup"
  - "RegisterPermissionsEndpoint uses AllowAnonymous — intended for service-to-service calls; TODO secured via mTLS in production"
  - "PersonaProfileService catches RpcException with StatusCode.NotFound and returns false instead of propagating; other gRPC errors bubble up"
  - "AssignRoleCommand validates role first, then profile via gRPC — role check is cheap (local DB) so we fail fast before network call"
  - "GetUserRolesQueryHandler joins assignments with roles in-memory using roleRepository.GetAllAsync — acceptable for v1 small tenant role catalogue"

patterns-established:
  - "gRPC client pattern: copy proto from source service, set GrpcServices=Client in csproj, wrap in interface for testability"
  - "PermissionSeeder pattern: any service that owns permissions seeds them directly via IHostedService, never self-calls the HTTP endpoint"
  - "ValidateProfileExistsAsync: gRPC cross-service validation before any operation that references a foreign aggregate key"

requirements-completed: [ORG-04, ORG-05, ORG-06]

duration: 9min
completed: 2026-03-18
---

# Phase 01 Plan 04: User-Role Assignments, Permission Registration, and Persona gRPC Client Summary

**Three role-assignment endpoints (assign/revoke/list), POST /permissions/register for service-to-service self-registration, Persona gRPC client for profileId validation, and Organizations seeding its own permissions via IHostedService**

## Performance

- **Duration:** 9 min
- **Started:** 2026-03-18T22:05:03Z
- **Completed:** 2026-03-18T22:13:59Z
- **Tasks:** 2
- **Files modified:** 21

## Accomplishments

- Full RBAC feature set delivered: assign role (POST), revoke role (DELETE), list user roles (GET)
- POST /permissions/register endpoint enables any microservice to register its permission strings idempotently
- PermissionSeeder IHostedService seeds Organizations' own 6 permission strings on startup without self-calling HTTP
- Persona gRPC client (IPersonaProfileService) validates profileId cross-service before role assignment
- 8 new unit tests (41 total), all passing; 64 arch tests still passing; solution builds 0 errors

## Task Commits

Each task was committed atomically:

1. **Task 1: Permission registration endpoint, Persona gRPC client, and Organizations seeding** - `a761594` (feat)
2. **Task 2: Assign role and revoke role features with gRPC profileId validation** - `d64def4` (feat)
3. **Formatter cleanup** - `aae26cd` (style — pre-commit hook corrections)

**Plan metadata:** to be added after STATE/ROADMAP update.

_Note: TDD tasks have multiple commits (test → feat) as designed._

## Files Created/Modified

- `src/BuildingBlocks/Edvantix.Constants/Permissions/OrganizationsPermissions.cs` - 6 permission constants + All collection
- `src/Services/Organizations/Edvantix.Organizations/Features/Permissions/RegisterPermissions/RegisterPermissionsCommand.cs` - ICommand handler calling UpsertAsync
- `src/Services/Organizations/Edvantix.Organizations/Features/Permissions/RegisterPermissions/RegisterPermissionsEndpoint.cs` - POST /permissions/register, AllowAnonymous
- `src/Services/Organizations/Edvantix.Organizations/Features/Permissions/RegisterPermissions/RegisterPermissionsValidator.cs` - Regex kebab-case format validation
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/Seeding/PermissionSeeder.cs` - IHostedService direct-DB seeder
- `src/Services/Organizations/Edvantix.Organizations/Grpc/Protos/persona/v1/profile.proto` - Persona proto copy for client codegen
- `src/Services/Organizations/Edvantix.Organizations/Grpc/Extensions.cs` - AddGrpcServices() wires client + singleton
- `src/Services/Organizations/Edvantix.Organizations/Grpc/Services/IPersonaProfileService.cs` - ValidateProfileExistsAsync interface
- `src/Services/Organizations/Edvantix.Organizations/Grpc/Services/PersonaProfileService.cs` - gRPC client wrapper, handles NotFound
- `src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/AssignRole/AssignRoleCommand.cs` - Handler with gRPC validation + duplicate check
- `src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/AssignRole/AssignRoleEndpoint.cs` - POST /user-role-assignments
- `src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/AssignRole/AssignRoleValidator.cs` - ProfileId + RoleId not empty
- `src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/RevokeRole/RevokeRoleCommand.cs` - Handler hard-deletes assignment
- `src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/RevokeRole/RevokeRoleEndpoint.cs` - DELETE /user-role-assignments/{profileId}/{roleId}
- `src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/GetUserRoles/GetUserRolesQuery.cs` - In-memory join, returns UserRoleItem list
- `src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/GetUserRoles/GetUserRolesEndpoint.cs` - GET /user-role-assignments/{profileId}
- `src/Services/Organizations/Edvantix.Organizations/Edvantix.Organizations.csproj` - Added Grpc.Tools + Grpc.AspNetCore.Server.ClientFactory + Protobuf item
- `src/Services/Organizations/Edvantix.Organizations/Extensions/Extensions.cs` - Wired AddGrpcServices() + AddHostedService<PermissionSeeder>()
- `src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/RegisterPermissionsCommandHandlerTests.cs` - 2 tests
- `src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/AssignRoleCommandHandlerTests.cs` - 4 tests
- `src/Services/Organizations/Edvantix.Organizations.UnitTests/Features/RevokeRoleCommandHandlerTests.cs` - 2 tests

## Decisions Made

- **PermissionSeeder as IHostedService:** Other services call POST /permissions/register on their own startup. Organizations cannot do the same — it would be calling itself before it's fully started. Direct repository access in StartAsync avoids this race condition.
- **AllowAnonymous on /permissions/register:** Service-to-service calls during startup don't carry a user JWT. Left with a TODO comment noting production should use mTLS or a shared secret policy.
- **NotFoundException.For<T>() pattern:** Used existing static factory instead of 2-arg constructor (which doesn't exist in the codebase).
- **Grpc path follows Blog pattern exactly:** proto copied to `Grpc/Protos/persona/v1/`, csharp_namespace preserved as `Edvantix.Persona.Grpc.Services` to match generated types already used by Blog.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] NotFoundException constructor mismatch**
- **Found during:** Task 2 (AssignRoleCommand implementation)
- **Issue:** Plan spec used `new NotFoundException(nameof(Role), request.RoleId)` — 2-arg constructor that doesn't exist. `NotFoundException` only takes a single string message (or uses static `For<T>()` factory).
- **Fix:** Changed to `NotFoundException.For<Role>(request.RoleId)` for role not found; used `new NotFoundException($"Profile with id {id} not found.")` for cross-service profile not found.
- **Files modified:** AssignRoleCommand.cs, RevokeRoleCommand.cs
- **Verification:** Solution builds with 0 errors
- **Committed in:** d64def4 (Task 2 commit)

---

**Total deviations:** 1 auto-fixed (Rule 1 - Bug)
**Impact on plan:** Fix necessary for compilation. No scope creep.

## Issues Encountered

None beyond the NotFoundException constructor mismatch documented above.

## User Setup Required

None — no external service configuration required. The Persona gRPC service URL is resolved via Aspire service discovery using `Constants.Aspire.Services.Persona`.

## Next Phase Readiness

- Phase 1 RBAC core is complete: tenant isolation, domain model, role management, permission management, role-assignment, and permission seeding are all delivered.
- Phase 2 (Organizations gRPC server) can now expose role-assignment queries to other services.
- Phase 3 (Scheduling) can call POST /permissions/register at startup to self-register scheduling permissions.
- The PermissionSeeder pattern is established for all future services to follow.

---
*Phase: 01-organizations-rbac-core*
*Completed: 2026-03-18*
