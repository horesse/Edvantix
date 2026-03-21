---
phase: 3
slug: scheduling-slots-and-views
status: draft
nyquist_compliant: true
wave_0_complete: true
created: 2026-03-21
---

# Phase 3 — Validation Strategy

> Per-phase validation contract for feedback sampling during execution.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | TUnit (TngTech.ArchUnitNET.TUnit) |
| **Config file** | `tests/Edvantix.ArchTests/Edvantix.ArchTests.csproj` |
| **Quick run command** | `dotnet test tests/Edvantix.ArchTests` |
| **Full suite command** | `dotnet test` |
| **Estimated runtime** | ~30 seconds (arch tests) / ~2 min (full suite) |

---

## Sampling Rate

- **After every task commit:** Run `dotnet test tests/Edvantix.ArchTests`
- **After every plan wave:** Run `dotnet test`
- **Before `/gsd:verify-work`:** Full suite must be green
- **Max feedback latency:** ~30 seconds

---

## Per-Task Verification Map

| Task ID | Plan | Wave | Requirement | Test Type | Automated Command | File Exists | Status |
|---------|------|------|-------------|-----------|-------------------|-------------|--------|
| 3-01-W0 | 01 | 0 | SCH-01, SCH-10 | Arch | `dotnet test tests/Edvantix.ArchTests` | ❌ W0 | ⬜ pending |
| 3-01-01 | 01 | 1 | SCH-01 | Arch/unit | `dotnet test tests/Edvantix.ArchTests` | ❌ W0 | ⬜ pending |
| 3-02-01 | 02 | 1 | SCH-02 | Unit | `dotnet test src/Services/Scheduling` | ❌ W0 | ⬜ pending |
| 3-03-01 | 03 | 1 | SCH-03 | Unit | `dotnet test src/Services/Scheduling` | ❌ W0 | ⬜ pending |
| 3-03-02 | 03 | 1 | SCH-04 | Manual | — | — | ⬜ pending |
| 3-03-03 | 03 | 1 | SCH-05 | Manual | — | — | ⬜ pending |
| 3-04-01 | 04 | 1 | SCH-06 | Unit | `dotnet test src/Services/Scheduling` | ❌ W0 | ⬜ pending |
| 3-04-02 | 04 | 1 | SCH-07 | Unit | `dotnet test src/Services/Scheduling` | ❌ W0 | ⬜ pending |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

---

## Wave 0 Requirements

- [ ] `tests/Edvantix.ArchTests/Abstractions/BaseTest.cs` — add `SchedulingAssembly` constant
- [ ] `tests/Edvantix.ArchTests/Abstractions/ArchUnitBaseTest.cs` — add `SchedulingAssembly` to `Architecture` loader and `SchedulingServiceTypes`
- [ ] `tests/Edvantix.ArchTests/Domain/SchedulingTenantIsolationTests.cs` — new arch test class parallel to `TenantIsolationTests` for `SchedulingDbContext`
- [ ] `tests/Edvantix.ArchTests/Edvantix.ArchTests.csproj` — add `<ProjectReference>` to `Edvantix.Scheduling.csproj`
- [ ] `src/Services/Scheduling/Edvantix.Scheduling.UnitTests/` — new unit test project following Organizations `.UnitTests` pattern (Group aggregate + LessonSlot aggregate tests)

---

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| Teacher sees only own slots | SCH-04 | No integration test project in v1 | Call `GET /schedule?dateFrom=X&dateTo=Y` with a teacher JWT; verify response contains only slots where `teacherId` matches caller |
| Student sees only group-membership slots | SCH-05 | No integration test project in v1 | Call `GET /schedule?dateFrom=X&dateTo=Y` with a student JWT; verify response contains only slots for groups the student belongs to |

---

## Validation Sign-Off

- [ ] All tasks have `<automated>` verify or Wave 0 dependencies
- [ ] Sampling continuity: no 3 consecutive tasks without automated verify
- [ ] Wave 0 covers all MISSING references
- [ ] No watch-mode flags
- [ ] Feedback latency < 30s
- [ ] `nyquist_compliant: true` set in frontmatter

**Approval:** pending
