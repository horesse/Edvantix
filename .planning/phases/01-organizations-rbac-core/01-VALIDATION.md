---
phase: 1
slug: organizations-rbac-core
status: draft
nyquist_compliant: false
wave_0_complete: false
created: 2026-03-18
---

# Phase 1 — Validation Strategy

> Per-phase validation contract for feedback sampling during execution.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | TUnit 1.19.74 + TngTech.ArchUnitNET.TUnit 0.13.3 |
| **Config file** | `global.json` → `"test": { "runner": "Microsoft.Testing.Platform" }` |
| **Quick run command** | `dotnet test tests/Edvantix.Organizations.UnitTests/ -x` |
| **Full suite command** | `dotnet test --filter "FullyQualifiedName~Organizations"` |
| **Estimated runtime** | ~15 seconds |

---

## Sampling Rate

- **After every task commit:** Run `dotnet test tests/Edvantix.Organizations.UnitTests/ -x`
- **After every plan wave:** Run `dotnet test --filter "FullyQualifiedName~Organizations"`
- **Before `/gsd:verify-work`:** Full suite must be green
- **Max feedback latency:** 15 seconds

---

## Per-Task Verification Map

| Task ID | Plan | Wave | Requirement | Test Type | Automated Command | File Exists | Status |
|---------|------|------|-------------|-----------|-------------------|-------------|--------|
| 1-01-01 | 01-01 | 1 | ORG-10 | arch | `dotnet test tests/Edvantix.ArchTests/` | ❌ W0 | ⬜ pending |
| 1-01-02 | 01-02 | 2 | ORG-01, ORG-02 | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "Role"` | ❌ W0 | ⬜ pending |
| 1-01-03 | 01-02 | 2 | ORG-03 | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "AssignPermissions"` | ❌ W0 | ⬜ pending |
| 1-01-04 | 01-03 | 3 | ORG-01, ORG-02 | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "Role"` | ❌ W0 | ⬜ pending |
| 1-01-05 | 01-04 | 4 | ORG-04 | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "RegisterPermissions"` | ❌ W0 | ⬜ pending |
| 1-01-06 | 01-04 | 4 | ORG-05 | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "AssignRole"` | ❌ W0 | ⬜ pending |
| 1-01-07 | 01-04 | 4 | ORG-06 | unit | `dotnet test tests/Edvantix.Organizations.UnitTests/ --filter "RevokeRole"` | ❌ W0 | ⬜ pending |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

---

## Wave 0 Requirements

- [ ] `tests/Edvantix.Organizations.UnitTests/` — new TUnit test project, mirrors `Edvantix.Persona.UnitTests` structure
- [ ] `tests/Edvantix.Organizations.UnitTests/GlobalUsings.cs` — standard global usings (Moq, TUnit, Shouldly, Bogus)
- [ ] `tests/Edvantix.ArchTests/Domain/TenantIsolationTests.cs` — EF Core model query filter test for ITenanted entities (covers ORG-10)
- [ ] `tests/Edvantix.ArchTests/Abstractions/BaseTest.cs` — add `OrganizationsAssembly` reference

*Framework already present — no new packages needed.*

---

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| gRPC call to Persona validates profileId on role assignment | ORG-05 | Requires live Persona gRPC endpoint | Run Aspire app, assign role with valid/invalid profileId, verify 404 vs 201 |
| Permission upsert on startup registers strings in Organizations DB | ORG-04 | Requires startup integration | Run Aspire app, inspect `Permissions` table for `organizations.*` strings |

---

## Validation Sign-Off

- [ ] All tasks have `<automated>` verify or Wave 0 dependencies
- [ ] Sampling continuity: no 3 consecutive tasks without automated verify
- [ ] Wave 0 covers all MISSING references
- [ ] No watch-mode flags
- [ ] Feedback latency < 15s
- [ ] `nyquist_compliant: true` set in frontmatter

**Approval:** pending
