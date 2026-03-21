---
phase: 02-organizations-permission-cache
plan: "02"
subsystem: domain-events, cache, messaging
tags: [domain-events, integration-events, masstransit, hybrid-cache, cache-invalidation, outbox, mediator, grpc]

# Dependency graph
requires:
  - phase: 02-01
    provides: IHybridCache, GetUserPermissionGrantQuery, gRPC PermissionsGrpcService, permission cache infrastructure

provides:
  - UserRoleAssignedEvent, UserRoleRevokedEvent, RolePermissionsChangedEvent domain events
  - UserPermissionsInvalidatedIntegrationEvent in Edvantix.Contracts namespace
  - EventMapper mapping all three domain events to UserPermissionsInvalidated
  - Domain event handlers routing through IEventDispatcher to MassTransit outbox
  - Cache invalidation via IHybridCache.RemoveByTagAsync in all three mutation command handlers
  - UserRoleAssignment.Revoke() domain method
  - IUserRoleAssignmentRepository.GetAllByRoleIdAsync for cross-tenant role enumeration

affects:
  - 02-03 (plan 03 subscribes to UserPermissionsInvalidatedIntegrationEvent for cross-service cache invalidation)
  - future scheduling/payments services consuming UserPermissionsInvalidated

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Domain event handler dispatches to IEventDispatcher which routes via EventMapper to MassTransit outbox"
    - "Belt-and-suspenders cache invalidation: domain event (outbox) + immediate RemoveByTagAsync in handler"
    - "School-scoped invalidation tag: user:{profileId}:{schoolId} for per-user eviction"
    - "Null UserId in UserPermissionsInvalidatedIntegrationEvent signals school-wide invalidation"
    - "Revoke() domain method on aggregate before Remove() so domain event is dispatched by SaveEntitiesAsync"
    - "GetAllByRoleIdAsync uses IgnoreQueryFilters to enumerate across tenants for role-wide cache invalidation"

key-files:
  created:
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EventServices/Events/UserRoleAssignedEvent.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EventServices/Events/UserRoleRevokedEvent.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EventServices/Events/RolePermissionsChangedEvent.cs
    - src/Services/Organizations/Edvantix.Organizations/IntegrationEvents/UserPermissionsInvalidatedIntegrationEvent.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/EventHandlers/UserPermissionsDomainEventHandler.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Domain/UserRoleAssignmentDomainEventTests.cs
    - src/Services/Organizations/Edvantix.Organizations.UnitTests/Infrastructure/EventMapperTests.cs
  modified:
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/EventServices/EventMapper.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/UserRoleAssignmentAggregate/UserRoleAssignment.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/RoleAggregate/Role.cs
    - src/Services/Organizations/Edvantix.Organizations/Domain/AggregatesModel/UserRoleAssignmentAggregate/IUserRoleAssignmentRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Infrastructure/Repositories/UserRoleAssignmentRepository.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/AssignRole/AssignRoleCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/UserRoleAssignments/RevokeRole/RevokeRoleCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Features/Roles/AssignPermissions/AssignPermissionsCommand.cs
    - src/Services/Organizations/Edvantix.Organizations/Edvantix.Organizations.csproj
    - src/BuildingBlocks/Edvantix.Chassis/Edvantix.Chassis.csproj

key-decisions:
  - "Domain events placed in Infrastructure/EventServices/Events/ namespace matching plan specification — they have no infrastructure dependencies (only Guid fields), so the DomainEvent arch tests pass"
  - "Mediator source generator (MSG0005) requires all INotification types to have handlers — created UserRoleAssignedEventHandler, UserRoleRevokedEventHandler, RolePermissionsChangedEventHandler in Domain/EventHandlers/ following Persona pattern"
  - "InternalsVisibleTo added to Edvantix.Organizations.csproj to expose internal EventMapper class to unit tests — follows Blog/Scheduler service pattern"
  - "GetAllByRoleIdAsync uses IgnoreQueryFilters() to bypass tenant filter — required for role-wide cache invalidation across all schools, with roleId as the explicit isolation scope"
  - "AssignPermissionsCommandHandler extracts cache invalidation into private InvalidateCacheForRoleUsersAsync method for readability"
  - "Google.Protobuf package was missing from Edvantix.Chassis.csproj — Grpc.Tools generates code that imports Google.Protobuf but the package was not referenced; fixed as Rule 3 blocker"

patterns-established:
  - "Domain event handler pattern: INotificationHandler<TEvent> delegates to IEventDispatcher.DispatchAsync"
  - "EventMapper switch expression: each domain event maps to UserPermissionsInvalidatedIntegrationEvent"
  - "Cache invalidation pattern: domain event via outbox + immediate RemoveByTagAsync (belt-and-suspenders)"
  - "Tag format: user:{profileId}:{schoolId} for per-user permission cache lookup"

requirements-completed: [ORG-08, ORG-09]

# Metrics
duration: 8min
completed: 2026-03-21
---

# Phase 02 Plan 02: Domain Events and Cache Invalidation Summary

**Domain events on UserRoleAssignment and Role aggregates, EventMapper switch expression, UserPermissionsInvalidatedIntegrationEvent, and RemoveByTagAsync cache invalidation in all three mutation command handlers**

## Performance

- **Duration:** 8 min
- **Started:** 2026-03-21T06:55:25Z
- **Completed:** 2026-03-21T06:57:14Z
- **Tasks:** 2
- **Files modified:** 17

## Accomplishments

- Domain events raised on all three mutation paths: `UserRoleAssignedEvent` (in constructor), `UserRoleRevokedEvent` (via new `Revoke()` method), `RolePermissionsChangedEvent` (in `SetPermissions`)
- `UserPermissionsInvalidatedIntegrationEvent` in `Edvantix.Contracts` namespace with nullable `UserId` (null = school-wide invalidation)
- `EventMapper` switch covers all three domain events; domain event handlers route through `IEventDispatcher` to MassTransit outbox
- All three mutation command handlers (`AssignRole`, `RevokeRole`, `AssignPermissions`) call `RemoveByTagAsync` with `user:{profileId}:{schoolId}` tag
- `GetAllByRoleIdAsync` added to enumerate all users with a role for permission-change cache invalidation
- 57 unit tests and 64 arch tests pass

## Task Commits

Each task was committed atomically:

1. **Task 1: Domain events on aggregates and EventMapper extension with integration event** - `70e4156` (feat)
2. **Task 2: Cache invalidation in mutation command handlers via RemoveByTagAsync** - `4d21217` (feat)

## Files Created/Modified

- `Infrastructure/EventServices/Events/UserRoleAssignedEvent.cs` - Domain event for role assignment with ProfileId, SchoolId, RoleId
- `Infrastructure/EventServices/Events/UserRoleRevokedEvent.cs` - Domain event for role revocation
- `Infrastructure/EventServices/Events/RolePermissionsChangedEvent.cs` - Domain event for role permission changes (schoolId scope)
- `IntegrationEvents/UserPermissionsInvalidatedIntegrationEvent.cs` - Integration event in Edvantix.Contracts, nullable UserId
- `Domain/EventHandlers/UserPermissionsDomainEventHandler.cs` - Three handler classes for Mediator source generator compliance
- `Infrastructure/EventServices/EventMapper.cs` - Switch expression mapping all three domain events
- `Domain/AggregatesModel/UserRoleAssignmentAggregate/UserRoleAssignment.cs` - Added RegisterDomainEvent in constructor, added Revoke() method
- `Domain/AggregatesModel/RoleAggregate/Role.cs` - Added RegisterDomainEvent in SetPermissions
- `Domain/AggregatesModel/UserRoleAssignmentAggregate/IUserRoleAssignmentRepository.cs` - Added GetAllByRoleIdAsync
- `Infrastructure/Repositories/UserRoleAssignmentRepository.cs` - Implemented GetAllByRoleIdAsync with IgnoreQueryFilters
- `Features/UserRoleAssignments/AssignRole/AssignRoleCommand.cs` - Added IHybridCache, RemoveByTagAsync after SaveEntitiesAsync
- `Features/UserRoleAssignments/RevokeRole/RevokeRoleCommand.cs` - Added IHybridCache, Revoke() before Remove, RemoveByTagAsync
- `Features/Roles/AssignPermissions/AssignPermissionsCommand.cs` - Added IHybridCache + IUserRoleAssignmentRepository, loop invalidation

## Decisions Made

- Domain events placed in `Infrastructure/EventServices/Events/` per plan spec; they pass arch tests because they contain only `Guid` fields with no infrastructure imports
- Mediator source generator `MSG0005` error required adding three domain event handlers in `Domain/EventHandlers/` — follows Persona service pattern
- `InternalsVisibleTo` added to `Edvantix.Organizations.csproj` to let unit tests access internal `EventMapper` — follows Blog/Scheduler pattern
- `GetAllByRoleIdAsync` uses `IgnoreQueryFilters()` so cross-school role enumeration is possible without a tenant context
- `AssignPermissionsCommandHandler` extracted cache invalidation into `InvalidateCacheForRoleUsersAsync` private method for readability

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Added Google.Protobuf explicit reference to Edvantix.Chassis.csproj**
- **Found during:** Task 1 (domain event build verification)
- **Issue:** `Grpc.Tools` generates proto stubs that import `Google.Protobuf` types, but `Google.Protobuf` was not explicitly referenced in `Edvantix.Chassis.csproj`, causing 13 compile errors (`CS0400`, `CS0538`)
- **Fix:** Added `<PackageReference Include="Google.Protobuf" />` to the gRPC ItemGroup in `Edvantix.Chassis.csproj`
- **Files modified:** `src/BuildingBlocks/Edvantix.Chassis/Edvantix.Chassis.csproj`
- **Verification:** `dotnet build src/BuildingBlocks/Edvantix.Chassis/` exits 0
- **Committed in:** `70e4156` (Task 1 commit)

**2. [Rule 2 - Missing Critical] Added three domain event handlers for Mediator source generator compliance**
- **Found during:** Task 1 (Organizations service build)
- **Issue:** Mediator source generator error `MSG0005` — all `INotification` implementations (including `DomainEvent`) must have registered handlers; without them the Organizations service fails to compile
- **Fix:** Created `Domain/EventHandlers/UserPermissionsDomainEventHandler.cs` with three handler classes, each delegating to `IEventDispatcher.DispatchAsync` following the Persona service pattern
- **Files modified:** `src/Services/Organizations/Edvantix.Organizations/Domain/EventHandlers/UserPermissionsDomainEventHandler.cs`
- **Verification:** `dotnet build src/Services/Organizations/Edvantix.Organizations/` exits 0
- **Committed in:** `70e4156` (Task 1 commit)

**3. [Rule 2 - Missing Critical] Added InternalsVisibleTo for unit test access to internal EventMapper**
- **Found during:** Task 1 (EventMapperTests build)
- **Issue:** `EventMapper` is `internal sealed` (by design) but the test project can't access it; `CS0122` error in `EventMapperTests.cs`
- **Fix:** Added `<InternalsVisibleTo Include="DynamicProxyGenAssembly2" />` and `<InternalsVisibleTo Include="$(AssemblyName).UnitTests" />` to `Edvantix.Organizations.csproj` following Blog/Scheduler pattern
- **Files modified:** `src/Services/Organizations/Edvantix.Organizations/Edvantix.Organizations.csproj`
- **Verification:** All 57 unit tests pass
- **Committed in:** `70e4156` (Task 1 commit)

---

**Total deviations:** 3 auto-fixed (all Rule 2/3 — blocking or missing critical)
**Impact on plan:** All fixes were prerequisites for the plan's goals. No scope creep.

## Issues Encountered

None beyond the auto-fixed deviations above.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- Domain events, EventMapper, and integration event are fully wired for outbox publication
- All three mutation handlers invalidate local cache via `RemoveByTagAsync` immediately
- `UserPermissionsInvalidatedIntegrationEvent` is available in `Edvantix.Contracts` for Phase 02-03 consumer subscription
- `GetAllByRoleIdAsync` provides the enumeration needed by AssignPermissionsCommand for cross-user invalidation

---
*Phase: 02-organizations-permission-cache*
*Completed: 2026-03-21*
