---
phase: 02-organizations-permission-cache
plan: "03"
subsystem: chassis
tags: [grpc, authorization, rbac, permissions, chassis, aspnet-core]

requires:
  - phase: 02-organizations-permission-cache
    plan: "01"
    provides: gRPC CheckPermission server endpoint in Organizations service

provides:
  - PermissionRequirement (IAuthorizationRequirement carrying permission string)
  - PermissionRequirementHandler (AuthorizationHandler calling Organizations gRPC CheckPermission)
  - RequirePermission extension method on AuthorizationPolicyBuilder
  - AddPermissionAuthorization extension method on IHostApplicationBuilder

affects:
  - Downstream services (Scheduling, Payments) — can now call AddPermissionAuthorization + RequirePermission

tech-stack:
  added:
    - Grpc.AspNetCore.Server.ClientFactory (to Edvantix.Chassis — provides AddGrpcClient extension)
    - Grpc.Tools (to Edvantix.Chassis — proto code generation)
    - Google.Protobuf 3.33.0 (to Directory.Packages.props — required for proto message types in Chassis)
    - permissions.proto (copied to Chassis with GrpcServices=Both — shared Server+Client types)
  patterns:
    - Proto-in-Chassis pattern: permissions.proto lives in Chassis (GrpcServices=Both), Organizations removes its own proto entry to avoid CS0436 duplicate type conflict in same csharp_namespace
    - Permission handler resolves school via ITenantContext (scoped) through IHttpContextAccessor from singleton handler
    - AddPermissionAuthorization registers gRPC client + IHttpContextAccessor + singleton IAuthorizationHandler

key-files:
  created:
    - src/BuildingBlocks/Edvantix.Chassis/Security/Authorization/PermissionRequirement.cs
    - src/BuildingBlocks/Edvantix.Chassis/Security/Authorization/PermissionRequirementHandler.cs
    - src/BuildingBlocks/Edvantix.Chassis/Security/Protos/organizations/v1/permissions.proto
  modified:
    - src/BuildingBlocks/Edvantix.Chassis/Edvantix.Chassis.csproj
    - src/BuildingBlocks/Edvantix.Chassis/Security/Extensions/PolicyBuilderExtensions.cs
    - src/Services/Organizations/Edvantix.Organizations/Edvantix.Organizations.csproj
    - Directory.Packages.props

key-decisions:
  - "Proto-in-Chassis to avoid CS0436 duplicate type conflict: Organizations references Chassis via ServiceDefaults. Both generating types from the same csharp_namespace causes CS0436. Solution: proto lives in Chassis with GrpcServices=Both, Organizations removes its own Protobuf entry and uses Chassis-generated types."
  - "Google.Protobuf pinned to 3.33.0 in Directory.Packages.props: Aspire.Hosting.Yarp 13.1.2 requires >= 3.33.0. Earlier pick of 3.31.1 caused NU1109 downgrade error in AppHost."
  - "PermissionRequirementHandler registered as singleton with IHttpContextAccessor to access scoped ITenantContext: ITenantContext is scoped (per-request), so a singleton handler cannot receive it directly. IHttpContextAccessor is the canonical bridge to per-request services from singleton scope."
  - "AddPermissionAuthorization takes IHostApplicationBuilder (not IServiceCollection) to match the Aspire pattern used by other Chassis extension methods."

requirements-completed: [ORG-07]

duration: 8min
completed: 2026-03-21
---

# Phase 02 Plan 03: Chassis Permission Authorization Extension Summary

**PermissionRequirement + PermissionRequirementHandler + RequirePermission/AddPermissionAuthorization extensions enabling downstream services to guard endpoints via Organizations gRPC CheckPermission**

## Performance

- **Duration:** 8 min
- **Started:** 2026-03-21T06:48:29Z
- **Completed:** 2026-03-21T06:56:00Z
- **Tasks:** 1
- **Files modified:** 7

## Accomplishments

- `PermissionRequirement(string permission)` — carries the permission string required by an endpoint
- `PermissionRequirementHandler` — singleton `AuthorizationHandler<PermissionRequirement>` that resolves `profile_id` from JWT claims, reads `schoolId` from `ITenantContext` (via `IHttpContextAccessor`), calls Organizations gRPC `CheckPermission`, and calls `context.Succeed` only when `HasPermission = true`
- `RequirePermission(permission)` extension on `AuthorizationPolicyBuilder` — one-liner to add the requirement to any policy
- `AddPermissionAuthorization(builder, grpcBaseUrl)` extension on `IHostApplicationBuilder` — registers gRPC client, `IHttpContextAccessor`, and the singleton handler
- Chassis csproj updated with `Grpc.AspNetCore.Server.ClientFactory`, `Grpc.Tools`, `Google.Protobuf`
- `permissions.proto` moved to Chassis (`GrpcServices="Both"`) — Organizations' duplicate Protobuf entry removed to prevent CS0436 type conflict
- Chassis builds: 0 errors, 0 warnings
- Organizations builds: 0 errors, 0 warnings
- Architecture tests: 64/64 pass

## Task Commits

1. **Task 1: PermissionRequirement, PermissionRequirementHandler, AddPermissionAuthorization extension** — `cdbe710` (feat)

## Files Created/Modified

- `Security/Authorization/PermissionRequirement.cs` — `IAuthorizationRequirement` carrying `Permission` string
- `Security/Authorization/PermissionRequirementHandler.cs` — `AuthorizationHandler<PermissionRequirement>` calling gRPC CheckPermission
- `Security/Protos/organizations/v1/permissions.proto` — proto contract for Chassis gRPC code generation
- `Edvantix.Chassis.csproj` — added gRPC packages (Grpc.AspNetCore.Server.ClientFactory, Grpc.Tools, Google.Protobuf) and `<Protobuf>` entry with `GrpcServices="Both"`
- `Security/Extensions/PolicyBuilderExtensions.cs` — added `RequirePermission` and `AddPermissionAuthorization`
- `Organizations/Edvantix.Organizations.csproj` — removed `<Protobuf>` entry for permissions.proto (now in Chassis)
- `Directory.Packages.props` — added `Google.Protobuf` version 3.33.0

## Decisions Made

- **Proto-in-Chassis to avoid CS0436:** Organizations references Chassis via `Edvantix.ServiceDefaults`. Having both projects generate code from the same `csharp_namespace = "Edvantix.Organizations.Grpc.Generated"` proto causes `CS0436` type conflicts. The fix is to have Chassis generate `GrpcServices="Both"` (Server base class + Client stub) and have Organizations use the Chassis-generated types directly.
- **Google.Protobuf version 3.33.0:** `Aspire.Hosting.Yarp 13.1.2` requires `>= 3.33.0`. Initial pick of 3.31.1 caused `NU1109` downgrade error in AppHost. Updated `Directory.Packages.props` accordingly.
- **Singleton handler with IHttpContextAccessor:** `ITenantContext` is scoped per-request. The handler is registered as a singleton (consistent with `ProfileRegisteredRequirementHandler`). `IHttpContextAccessor` is the canonical bridge to access per-request services from singleton scope.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Proto duplicated in Chassis AND Organizations causes CS0436 type conflict**
- **Found during:** Task 1 (full solution build verification)
- **Issue:** Organizations references Chassis (via ServiceDefaults). Both projects generating types from the same `csharp_namespace` (`Edvantix.Organizations.Grpc.Generated`) causes `CS0436` "conflicting type" compiler errors in Organizations.
- **Fix:** Changed Chassis proto to `GrpcServices="Both"` and removed Organizations' `<Protobuf>` entry for permissions.proto. Organizations' concrete `PermissionsGrpcService` class already used the `Generated.PermissionsGrpcService.PermissionsGrpcServiceBase` base class — now it comes from Chassis instead of being locally generated.
- **Files modified:** Chassis.csproj (GrpcServices=Both), Organizations.csproj (removed proto entry)
- **Commit:** cdbe710

**2. [Rule 1 - Bug] Google.Protobuf version 3.31.1 caused NU1109 downgrade error in AppHost**
- **Found during:** Task 1 (full solution build verification)
- **Issue:** `Aspire.Hosting.Yarp 13.1.2` requires `Google.Protobuf >= 3.33.0`. Pinning to 3.31.1 in `Directory.Packages.props` triggered `NU1109` downgrade error.
- **Fix:** Updated `Directory.Packages.props` to use `Google.Protobuf` version 3.33.0.
- **Files modified:** Directory.Packages.props
- **Commit:** cdbe710

---

**Total deviations:** 2 auto-fixed (both build correctness bugs found during verification)
**Impact on plan:** Both fixes required for solution to compile. No behavioral scope changes.

## Known Stubs

None — all integration points are concrete implementations. The `AddPermissionAuthorization` extension requires a caller-provided gRPC URL; there is no hardcoded placeholder.

## Next Phase Readiness

- Downstream services (Scheduling, Payments) can call `builder.AddPermissionAuthorization("https+http://organizations")` during startup registration
- Endpoints use `policy.RequirePermission("service.action")` in `AddAuthorization` policy definitions
- The Organizations gRPC server (from plan 02-01) handles the `CheckPermission` RPC

---
*Phase: 02-organizations-permission-cache*
*Completed: 2026-03-21*

## Self-Check: PASSED

- `PermissionRequirement.cs` exists on disk (verified)
- `PermissionRequirementHandler.cs` exists on disk (verified)
- `permissions.proto` exists on disk (verified)
- Task commit `cdbe710` exists in git log (verified)
- Chassis builds: 0 errors, 0 warnings (verified)
- Organizations builds: 0 errors, 0 warnings (verified)
- Architecture tests: 64/64 pass (verified)
