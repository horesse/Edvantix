---
phase: 5
slug: payments-ledger-balance
status: draft
nyquist_compliant: false
wave_0_complete: false
created: 2026-03-22
---

# Phase 5 — Validation Strategy

> Per-phase validation contract for feedback sampling during execution.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | TUnit (.NET) + xUnit-style assertions |
| **Config file** | `tests/Edvantix.Payments.Tests/Edvantix.Payments.Tests.csproj` (Wave 0 creates) |
| **Quick run command** | `dotnet test tests/Edvantix.Payments.Tests/ --no-build -v q` |
| **Full suite command** | `dotnet test tests/Edvantix.Payments.Tests/ -v n` |
| **Estimated runtime** | ~30 seconds |

---

## Sampling Rate

- **After every task commit:** Run `dotnet test tests/Edvantix.Payments.Tests/ --no-build -v q`
- **After every plan wave:** Run `dotnet test tests/Edvantix.Payments.Tests/ -v n`
- **Before `/gsd:verify-work`:** Full suite must be green
- **Max feedback latency:** 60 seconds

---

## Per-Task Verification Map

| Task ID | Plan | Wave | Requirement | Test Type | Automated Command | File Exists | Status |
|---------|------|------|-------------|-----------|-------------------|-------------|--------|
| 05-01-01 | 01 | 1 | PAY-01 | unit | `dotnet test --filter "StudentBalance"` | ❌ W0 | ⬜ pending |
| 05-01-02 | 01 | 1 | PAY-01 | unit | `dotnet test --filter "LessonTransaction"` | ❌ W0 | ⬜ pending |
| 05-01-03 | 01 | 1 | PAY-01 | unit | `dotnet test --filter "PaymentsDbContext"` | ❌ W0 | ⬜ pending |
| 05-02-01 | 02 | 2 | PAY-02,PAY-05 | unit | `dotnet test --filter "AddCreditsCommand"` | ❌ W0 | ⬜ pending |
| 05-02-02 | 02 | 2 | PAY-03 | unit | `dotnet test --filter "GetStudentBalance"` | ❌ W0 | ⬜ pending |
| 05-03-01 | 03 | 3 | PAY-02 | unit | `dotnet test --filter "AttendanceRecordedConsumer"` | ❌ W0 | ⬜ pending |
| 05-03-02 | 03 | 3 | PAY-02 | unit | `dotnet test --filter "Idempotency"` | ❌ W0 | ⬜ pending |
| 05-03-03 | 03 | 3 | PAY-04 | unit | `dotnet test --filter "GetSlotPaymentStatus"` | ❌ W0 | ⬜ pending |
| 05-03-04 | 03 | 3 | PAY-06 | unit | `dotnet test --filter "PaymentStatus"` | ❌ W0 | ⬜ pending |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

---

## Wave 0 Requirements

- [ ] `tests/Edvantix.Payments.Tests/Edvantix.Payments.Tests.csproj` — test project referencing Payments service
- [ ] `tests/Edvantix.Payments.Tests/Domain/StudentBalanceTests.cs` — stubs for PAY-01 domain tests
- [ ] `tests/Edvantix.Payments.Tests/Application/AddCreditsCommandHandlerTests.cs` — stubs for PAY-02, PAY-05
- [ ] `tests/Edvantix.Payments.Tests/Application/GetStudentBalanceQueryHandlerTests.cs` — stubs for PAY-03
- [ ] `tests/Edvantix.Payments.Tests/Consumer/AttendanceRecordedConsumerTests.cs` — stubs for PAY-02 idempotency
- [ ] `tests/Edvantix.Payments.Tests/Grpc/PaymentStatusServiceTests.cs` — stubs for PAY-04, PAY-06

*Existing TUnit infrastructure in other test projects covers framework setup.*

---

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| Balance display in frontend UI | PAY-03 | UI not part of this phase | Verify via API endpoint response |
| End-to-end Kafka → consumer → debit flow | PAY-02 | Requires running Aspire stack | `just run` + trigger attendance via POST |
| gRPC call from Scheduling to Payments | PAY-04 | Requires Aspire + both services | `just run` + GET /slots/{id}/attendance |

---

## Validation Sign-Off

- [ ] All tasks have `<automated>` verify or Wave 0 dependencies
- [ ] Sampling continuity: no 3 consecutive tasks without automated verify
- [ ] Wave 0 covers all MISSING references
- [ ] No watch-mode flags
- [ ] Feedback latency < 60s
- [ ] `nyquist_compliant: true` set in frontmatter

**Approval:** pending
