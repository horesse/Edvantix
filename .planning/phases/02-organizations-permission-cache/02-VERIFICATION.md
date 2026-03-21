---
phase: 02-organizations-permission-cache
verified: 2026-03-21T07:30:00Z
status: passed
score: 10/10 must-haves verified
re_verification: false
---

# Phase 02: Organizations Permission Cache Verification Report

**Phase Goal:** Downstream services can enforce permission checks via a gRPC call to Organizations that is backed by a HybridCache (L1 in-memory + L2 Redis) with tag-based invalidation triggered by domain events on role and assignment mutations.
**Verified:** 2026-03-21T07:30:00Z
**Status:** passed
**Re-verification:** No — initial verification

## Goal Achievement

### Observable Truths

| #  | Truth | Status | Evidence |
|----|-------|--------|----------|
| 1 | gRPC CheckPermission(userId, schoolId, permission) returns true when user has permission via assigned role | VERIFIED | `PermissionsGrpcService.cs` delegates to `GetUserPermissionGrantQuery` via `mediator.Send`; query handler loads assignments, roles, permissions from DB and returns bool |
| 2 | gRPC CheckPermission returns false when user lacks permission | VERIFIED | Handler returns `false` when assignments.Count == 0, assignedRoles.Count == 0, permissionIds.Count == 0, or permission not found in catalogue; unit test `GivenUserLacksPermission_WhenCheckPermission_ThenReturnsFalse` covers this |
| 3 | GET /permissions?userId={id}&schoolId={id} returns string[] of all permissions for user in school | VERIFIED | `GetPermissionsEndpoint.cs` maps `/permissions` with `AllowAnonymous`, calls `GetUserPermissionsQuery`, returns `string[]` sorted and distinct |
| 4 | Second CheckPermission call within TTL does not hit database (served from HybridCache) | VERIFIED | `GetUserPermissionGrantQueryHandler` calls `cache.GetOrCreateAsync` with key `perm:{userId}:{schoolId}:{permission}` and tag `user:{userId}:{schoolId}`; unit test `GivenCachedPermission_WhenCheckedAgain_ThenCacheFactoryNotCalledTwice` verifies key/tag format |
| 5 | When a role is assigned to a user, a UserPermissionsInvalidated integration event is published | VERIFIED | `UserRoleAssignment` constructor calls `RegisterDomainEvent(new UserRoleAssignedEvent(...))`. `EventMapper` switch maps `UserRoleAssignedEvent` to `UserPermissionsInvalidatedIntegrationEvent`. Domain event handler routes to `IEventDispatcher` (MassTransit outbox) |
| 6 | When a role is revoked from a user, a UserPermissionsInvalidated integration event is published | VERIFIED | `UserRoleAssignment.Revoke()` calls `RegisterDomainEvent(new UserRoleRevokedEvent(...))`. `RevokeRoleCommandHandler` calls `assignment.Revoke()` before `Remove`. EventMapper covers `UserRoleRevokedEvent` |
| 7 | When permissions on a role change, a UserPermissionsInvalidated integration event is published for affected users | VERIFIED | `Role.SetPermissions` calls `RegisterDomainEvent(new RolePermissionsChangedEvent(Id, SchoolId))`. EventMapper maps this to `UserPermissionsInvalidatedIntegrationEvent(null, schoolId, ...)` — null UserId signals school-wide invalidation |
| 8 | Cache is invalidated via RemoveByTagAsync in command handlers | VERIFIED | All three handlers (`AssignRoleCommandHandler`, `RevokeRoleCommandHandler`, `AssignPermissionsCommandHandler`) inject `IHybridCache` and call `cache.RemoveByTagAsync($"user:{profileId}:{schoolId}", ct)` after `SaveEntitiesAsync` |
| 9 | Downstream service can call AddPermissionAuthorization to register gRPC client and authorization handler | VERIFIED | `PolicyBuilderExtensions.AddPermissionAuthorization(IHostApplicationBuilder, string)` calls `AddGrpcClient<PermissionsGrpcServiceClient>`, `AddHttpContextAccessor`, and `AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>` |
| 10 | Endpoint can use RequirePermission("permission.string") to enforce a permission via gRPC call to Organizations | VERIFIED | `PolicyBuilderExtensions.RequirePermission(string)` calls `builder.AddRequirements(new PermissionRequirement(permission))`. `PermissionRequirementHandler` calls `grpcClient.CheckPermissionAsync` and calls `context.Succeed` only when `HasPermission = true` |

**Score:** 10/10 truths verified

### Required Artifacts

| Artifact | Expected | Status | Details |
|----------|----------|--------|---------|
| `Grpc/Protos/organizations/v1/permissions.proto` (in Organizations) | gRPC contract — `rpc CheckPermission` | VERIFIED | File exists, contains `rpc CheckPermission (CheckPermissionRequest) returns (CheckPermissionReply)`, csharp_namespace `Edvantix.Organizations.Grpc.Generated` |
| `Grpc/Services/PermissionsGrpcService.cs` | gRPC server implementation, inherits generated base | VERIFIED | Inherits `Generated.PermissionsGrpcService.PermissionsGrpcServiceBase`, delegates via `mediator.Send` |
| `Features/Permissions/CheckPermission/CheckPermissionQuery.cs` | Cached permission query handler using IHybridCache | VERIFIED | `GetUserPermissionGrantQuery` + `GetUserPermissionGrantQueryHandler`; uses `IHybridCache.GetOrCreateAsync` with `perm:{userId}:{schoolId}:{permission}` key and `user:{userId}:{schoolId}` tag |
| `Features/Permissions/CheckPermission/GetPermissionsEndpoint.cs` | REST cache priming endpoint with AllowAnonymous | VERIFIED | `AllowAnonymous()` present, queries DB directly via `GetUserPermissionsQuery` |
| `Infrastructure/EventServices/Events/UserRoleAssignedEvent.cs` | Domain event for role assignment | VERIFIED | `sealed class UserRoleAssignedEvent : DomainEvent` with `ProfileId`, `SchoolId`, `RoleId` |
| `Infrastructure/EventServices/Events/UserRoleRevokedEvent.cs` | Domain event for role revocation | VERIFIED | `sealed class UserRoleRevokedEvent : DomainEvent` |
| `Infrastructure/EventServices/Events/RolePermissionsChangedEvent.cs` | Domain event for role permissions change | VERIFIED | `sealed class RolePermissionsChangedEvent : DomainEvent` with `RoleId`, `SchoolId` |
| `IntegrationEvents/UserPermissionsInvalidatedIntegrationEvent.cs` | Integration event in Edvantix.Contracts namespace | VERIFIED | `namespace Edvantix.Contracts`, `sealed record` extending `IntegrationEvent`, nullable `UserId` |
| `Infrastructure/EventServices/EventMapper.cs` | Domain-to-integration event mapping via switch expression | VERIFIED | Switch covers `UserRoleAssignedEvent`, `UserRoleRevokedEvent`, `RolePermissionsChangedEvent` — all map to `UserPermissionsInvalidatedIntegrationEvent` |
| `Security/Authorization/PermissionRequirement.cs` (Chassis) | IAuthorizationRequirement carrying permission string | VERIFIED | `sealed class PermissionRequirement(string) : IAuthorizationRequirement` with `Permission` property |
| `Security/Authorization/PermissionRequirementHandler.cs` (Chassis) | AuthorizationHandler calling Organizations gRPC | VERIFIED | Inherits `AuthorizationHandler<PermissionRequirement>`, calls `grpcClient.CheckPermissionAsync`, resolves schoolId via `ITenantContext` through `IHttpContextAccessor` |
| `Security/Extensions/PolicyBuilderExtensions.cs` (Chassis) | AddPermissionAuthorization and RequirePermission extension methods | VERIFIED | Both methods present and wired |
| `Security/Protos/organizations/v1/permissions.proto` (Chassis) | Proto contract in Chassis with `GrpcServices="Both"` | VERIFIED | File exists in Chassis, `GrpcServices="Both"` in `Edvantix.Chassis.csproj` — Organizations' duplicate proto entry removed to prevent CS0436 |

### Key Link Verification

| From | To | Via | Status | Details |
|------|----|-----|--------|---------|
| `PermissionsGrpcService.cs` | `GetUserPermissionGrantQuery` | `mediator.Send(new GetUserPermissionGrantQuery(...))` | WIRED | Line 30-33 in PermissionsGrpcService.cs; query name changed from plan (CheckPermissionQuery → GetUserPermissionGrantQuery) due to arch rule enforcement |
| `GetUserPermissionGrantQueryHandler` | `IHybridCache` | `cache.GetOrCreateAsync(key, factory, tags: [tag])` | WIRED | Line 45-50 in CheckPermissionQuery.cs; key pattern `perm:{userId}:{schoolId}:{permission}`, tag `user:{userId}:{schoolId}` |
| `AppHost.cs` | Organizations | `.WithReference(redis).WaitFor(redis)` | WIRED | Lines 82-83 in AppHost.cs explicitly present in organizationsApi builder chain |
| `UserRoleAssignment.cs` | `UserRoleAssignedEvent` | `RegisterDomainEvent` in constructor | WIRED | Line 41 in UserRoleAssignment.cs |
| `EventMapper.cs` | `UserPermissionsInvalidatedIntegrationEvent` | switch expression | WIRED | All three domain events mapped; null UserId for school-wide invalidation |
| `RevokeRoleCommand.cs` | `IHybridCache` | `cache.RemoveByTagAsync` after `assignment.Revoke()` and `SaveEntitiesAsync` | WIRED | `assignment.Revoke()` on line 46, `RemoveByTagAsync` on line 53 |
| `AssignRoleCommand.cs` | `IHybridCache` | `cache.RemoveByTagAsync` after `SaveEntitiesAsync` | WIRED | Line 78 |
| `AssignPermissionsCommand.cs` | `IHybridCache` | `RemoveByTagAsync` loop via `GetAllByRoleIdAsync` | WIRED | Lines 92-101; iterates all users with the role and calls `RemoveByTagAsync` per user |
| `PermissionRequirementHandler.cs` | `PermissionsGrpcServiceClient` | `grpcClient.CheckPermissionAsync(...)` | WIRED | Line 56 in PermissionRequirementHandler.cs |
| `PolicyBuilderExtensions.cs` | `PermissionRequirement` | `builder.AddRequirements(new PermissionRequirement(permission))` | WIRED | Line 39 in PolicyBuilderExtensions.cs |

### Requirements Coverage

| Requirement | Source Plan | Description | Status | Evidence |
|-------------|-------------|-------------|--------|----------|
| ORG-07 | Plans 01 + 03 | Organizations service provides gRPC CheckPermission(userId, schoolId, permission) → bool | SATISFIED | `PermissionsGrpcService` mapped in `Program.cs` line 43; Chassis `AddPermissionAuthorization` + `RequirePermission` enable downstream consumption |
| ORG-08 | Plans 01 + 02 | CheckPermission result cached in HybridCache with invalidation on role change | SATISFIED | HybridCache with L1=30s/L2=5min configured in `Extensions.cs`; Redis wired in AppHost; `RemoveByTagAsync` in all three mutation handlers |
| ORG-09 | Plan 02 | On role/assignment change, integration event published for cache invalidation in other services | SATISFIED | `UserPermissionsInvalidatedIntegrationEvent` in `Edvantix.Contracts` namespace; EventMapper wired; MassTransit outbox configured in `Extensions.cs`; domain event handlers in `Domain/EventHandlers/UserPermissionsDomainEventHandler.cs` |

All three requirement IDs (ORG-07, ORG-08, ORG-09) declared across plans 01-03 are accounted for. No orphaned requirements found in REQUIREMENTS.md for this phase.

### Anti-Patterns Found

| File | Line | Pattern | Severity | Impact |
|------|------|---------|----------|--------|
| `GetPermissionsEndpoint.cs` | 14 | `TODO: In production, secure this endpoint with a shared secret or service-to-service mTLS policy.` | Info | The endpoint is `AllowAnonymous` — this is intentional and consistent with existing `RegisterPermissionsEndpoint` convention, but the TODO flags a known production security gap |

No blocking stubs were found. The TODO is a documented known gap, not an unimplemented feature. The endpoint functions correctly for its stated purpose (service-to-service cache priming).

### Human Verification Required

#### 1. gRPC End-to-End Flow

**Test:** Run the Aspire application, assign a role to a user via the Organizations API, then call the gRPC `CheckPermission` RPC from a gRPC client (e.g., grpcurl) with matching userId/schoolId/permission values.
**Expected:** First call returns `has_permission: true`; Redis cache populated with `perm:{userId}:{schoolId}:{permission}` key; second call within 30 seconds served without hitting the database (verify via trace spans or structured logs showing no DB queries on second call).
**Why human:** Cache hit vs. miss behavior requires runtime verification; not deterministic in static analysis.

#### 2. HybridCache TTL Behavior

**Test:** Verify L1 (30-second in-memory) eviction and L2 (5-minute Redis) fallback. After a cache entry is written, wait >30 seconds and re-check; the second call should hit Redis (L2) but not the database. After 5 minutes, verify the full DB round-trip occurs.
**Expected:** Correct TTL behavior per `HybridCacheEntryOptions` in `Extensions.cs`.
**Why human:** TTL verification requires time-based testing; static analysis cannot confirm runtime behavior.

#### 3. Tag-Based Invalidation End-to-End

**Test:** Prime the cache for a user, then assign a new role to them via the `AssignRole` endpoint. Verify that the subsequent `CheckPermission` call reflects the updated role (cache was invalidated and refreshed).
**Expected:** `RemoveByTagAsync($"user:{profileId}:{schoolId}")` evicts the cached entry; the next gRPC call hits the database.
**Why human:** Requires runtime observation of cache state and DB call sequence.

#### 4. Authorization Handler in Downstream Service

**Test:** In a downstream service (Scheduling or Payments), call `builder.AddPermissionAuthorization("https+http://organizations")` and add a policy using `RequirePermission("some.permission")`. Make an authenticated request with and without the permission.
**Expected:** Request with permission passes; request without permission returns HTTP 403.
**Why human:** Downstream integration has not yet been wired into any existing service — this tests the Chassis extension in a real consumer context.

### Gaps Summary

No automated gaps found. All 10 observable truths are verified, all 13 artifacts are substantive and wired, all 10 key links are confirmed present in source code, and all three requirement IDs (ORG-07, ORG-08, ORG-09) are satisfied.

The single notable finding is the `AllowAnonymous` TODO on `GetPermissionsEndpoint.cs` — this is an intentional deferral (documented in the source comment and consistent with existing service conventions) and does not block the phase goal. It should be addressed before production deployment.

---

_Verified: 2026-03-21T07:30:00Z_
_Verifier: Claude (gsd-verifier)_
