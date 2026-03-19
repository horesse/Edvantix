---
phase: 2
slug: organizations-permission-cache
status: draft
nyquist_compliant: false
wave_0_complete: false
created: 2026-03-19
---

# Phase 2 — Validation Strategy

> Per-phase validation contract for feedback sampling during execution.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | TUnit + xUnit (integration) |
| **Config file** | `tests/` — existing test projects |
| **Quick run command** | `dotnet test --filter "Category=Unit" --no-build` |
| **Full suite command** | `dotnet test --no-build` |
| **Estimated runtime** | ~60 seconds |

---

## Sampling Rate

- **After every task commit:** Run `dotnet test --filter "Category=Unit" --no-build`
- **After every plan wave:** Run `dotnet test --no-build`
- **Before `/gsd:verify-work`:** Full suite must be green
- **Max feedback latency:** 60 seconds

---

## Per-Task Verification Map

| Task ID | Plan | Wave | Requirement | Test Type | Automated Command | File Exists | Status |
|---------|------|------|-------------|-----------|-------------------|-------------|--------|
| 2-01-01 | 01 | 1 | ORG-07 | unit | `dotnet test --filter "CheckPermission"` | ❌ W0 | ⬜ pending |
| 2-01-02 | 01 | 1 | ORG-07 | integration | `dotnet test --filter "GrpcCheckPermission"` | ❌ W0 | ⬜ pending |
| 2-01-03 | 01 | 1 | ORG-07 | unit | `dotnet test --filter "GetPermissions"` | ❌ W0 | ⬜ pending |
| 2-02-01 | 02 | 2 | ORG-08 | unit | `dotnet test --filter "HybridCache"` | ❌ W0 | ⬜ pending |
| 2-02-02 | 02 | 2 | ORG-08 | integration | `dotnet test --filter "CacheEviction"` | ❌ W0 | ⬜ pending |
| 2-03-01 | 03 | 3 | ORG-09 | unit | `dotnet test --filter "UserPermissionsInvalidated"` | ❌ W0 | ⬜ pending |
| 2-03-02 | 03 | 3 | ORG-09 | integration | `dotnet test --filter "CacheInvalidation"` | ❌ W0 | ⬜ pending |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

---

## Wave 0 Requirements

- [ ] `tests/Organizations.Tests/Grpc/CheckPermissionServiceTests.cs` — stubs for ORG-07 gRPC endpoint
- [ ] `tests/Organizations.Tests/Cache/HybridCacheTests.cs` — stubs for ORG-08 cache behavior
- [ ] `tests/Organizations.Tests/Events/UserPermissionsInvalidatedTests.cs` — stubs for ORG-09 event dispatch
- [ ] `tests/Organizations.IntegrationTests/Permissions/PermissionCacheIntegrationTests.cs` — integration stubs

---

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| Cache eviction within 60s after role revocation | ORG-09 | Requires timing-dependent distributed system behavior | 1. Assign role to user. 2. Call CheckPermission — expect true. 3. Revoke role. 4. Wait ≤60s. 5. Call CheckPermission — expect false |
| L1→L2 cache fallback | ORG-08 | Requires restart of service instance to clear L1 | 1. Prime cache. 2. Restart Organizations pod. 3. Call CheckPermission — expect L2 Redis hit (no DB query in logs) |

---

## Validation Architecture

### gRPC CheckPermission (ORG-07)
- Unit: Mock `IPermissionRepository`, verify `CheckPermission` returns correct bool for known assignments
- Integration: Spin up Organizations service via Aspire test host, call gRPC endpoint, assert bool response
- REST GET /permissions: Unit test query handler returns correct permission string list

### HybridCache L1/L2 (ORG-08)
- Unit: Mock `IHybridCache`, verify `GetOrCreateAsync` called with correct key pattern (`perm:{userId}:{schoolId}:{permission}`)
- Unit: Verify `RemoveByTagAsync` called with tag `user:{userId}:{schoolId}` on role change
- Integration: Use TestContainers Redis, assert second call does not hit DB (verify via query count interceptor)

### Event Dispatch + Cache Invalidation (ORG-09)
- Unit: Verify `UserPermissionsInvalidatedIntegrationEvent` published via EventMapper on AssignRole/RevokeRole/AssignPermissions
- Unit: Verify cache `RemoveByTagAsync` called before outbox save in command handler
- Integration: Publish event via MassTransit test harness, assert consumer calls `RemoveByTagAsync`

---

## Validation Sign-Off

- [ ] All tasks have `<automated>` verify or Wave 0 dependencies
- [ ] Sampling continuity: no 3 consecutive tasks without automated verify
- [ ] Wave 0 covers all MISSING references
- [ ] No watch-mode flags
- [ ] Feedback latency < 60s
- [ ] `nyquist_compliant: true` set in frontmatter

**Approval:** pending
