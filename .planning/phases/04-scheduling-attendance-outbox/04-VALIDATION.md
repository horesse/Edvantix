---
phase: 4
slug: scheduling-attendance-outbox
status: draft
nyquist_compliant: false
wave_0_complete: false
created: 2026-03-21
---

# Phase 4 — Validation Strategy

> Per-phase validation contract for feedback sampling during execution.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | TUnit (all existing test projects) |
| **Config file** | None — test projects use `Sdk="Microsoft.NET.Sdk"` with `OutputType=Exe` |
| **Quick run command** | `dotnet test tests/Edvantix.Scheduling.UnitTests --no-build -x` |
| **Full suite command** | `just test` |
| **Estimated runtime** | ~30 seconds |

---

## Sampling Rate

- **After every task commit:** Run `dotnet test tests/Edvantix.Scheduling.UnitTests --no-build -x`
- **After every plan wave:** Run `just test`
- **Before `/gsd:verify-work`:** Full suite must be green
- **Max feedback latency:** 30 seconds

---

## Per-Task Verification Map

| Task ID | Plan | Wave | Requirement | Test Type | Automated Command | File Exists | Status |
|---------|------|------|-------------|-----------|-------------------|-------------|--------|
| 4-01-01 | 01 | 1 | SCH-08 | unit | `dotnet test tests/Edvantix.Scheduling.UnitTests --filter "AttendanceRecord" -x` | ❌ Wave 0 | ⬜ pending |
| 4-01-02 | 01 | 1 | SCH-08 | unit | `dotnet test tests/Edvantix.Scheduling.UnitTests --filter "AttendanceRecord" -x` | ❌ Wave 0 | ⬜ pending |
| 4-01-03 | 01 | 2 | SCH-08 | unit | `dotnet test tests/Edvantix.Scheduling.UnitTests --filter "MarkAttendance" -x` | ❌ Wave 0 | ⬜ pending |
| 4-01-04 | 01 | 2 | SCH-08 | unit | `dotnet test tests/Edvantix.Scheduling.UnitTests --filter "MarkAttendance" -x` | ❌ Wave 0 | ⬜ pending |
| 4-02-01 | 02 | 3 | SCH-09 | unit | `dotnet test tests/Edvantix.Scheduling.UnitTests --filter "EventMapper" -x` | ❌ Wave 0 | ⬜ pending |
| 4-02-02 | 02 | 3 | SCH-09 | arch | `dotnet test tests/Edvantix.ArchTests -x` | ✅ existing | ⬜ pending |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

---

## Wave 0 Requirements

- [ ] `tests/Edvantix.Scheduling.UnitTests/Domain/AttendanceRecordAggregateTests.cs` — stubs for SCH-08 aggregate behavior (construction, UpdateStatus, CorrelationId stability, Guard throws)
- [ ] `tests/Edvantix.Scheduling.UnitTests/Features/MarkAttendanceCommandHandlerTests.cs` — stubs for SCH-08 upsert logic (new record adds, existing record updates, no re-add)
- [ ] `tests/Edvantix.Scheduling.UnitTests/Infrastructure/EventMapperTests.cs` — stubs for SCH-09 mapping (all fields, CorrelationId preserved)

---

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| Kafka message appears on `edvantix.scheduling.attendance-recorded` topic after attendance mark | SCH-09 | Requires running Aspire stack with Kafka | Run `just run`, mark attendance via API, inspect Kafka UI or consumer logs |
| Application restart between DB commit and outbox poll still delivers event | SCH-09 | Requires process kill + restart timing | Mark attendance, kill process before outbox relay fires, restart, verify event appears |

---

## Validation Sign-Off

- [ ] All tasks have `<automated>` verify or Wave 0 dependencies
- [ ] Sampling continuity: no 3 consecutive tasks without automated verify
- [ ] Wave 0 covers all MISSING references
- [ ] No watch-mode flags
- [ ] Feedback latency < 30s
- [ ] `nyquist_compliant: true` set in frontmatter

**Approval:** pending
