# Phase 5: Payments — Ledger and Balance - Research

**Researched:** 2026-03-22
**Domain:** Financial ledger microservice — append-only transactions, MassTransit Kafka consumer with inbox, gRPC server
**Confidence:** HIGH

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions

- **D-01:** Single aggregate `StudentBalance` identified by `(StudentId, SchoolId)` — one balance per student per school
- **D-02:** No `LessonPackage` aggregate — package top-ups are plain `Credit` entries in the ledger
- **D-03:** Ledger entries are named `LessonTransaction`; transaction types: `ManualCredit`, `ManualDebit`, `AttendanceDebit`, `AttendanceReversal`
- **D-04:** `StudentBalance.Remaining` is computed dynamically at query time (sum of Credits minus sum of Debits) — not a stored field
- **D-05:** Balance CAN go negative — Payments never blocks an `AttendanceDebit` even when `Remaining = 0`; the student accumulates debt
- **D-06:** PAY-04 payment status per slot is computed dynamically from ledger at query time — not stored as a field
- **D-07:** `Paid` = student was marked `Present` AND balance was > 0 when the debit was applied
- **D-08:** `Unpaid` = no `AttendanceDebit` exists for this `CorrelationId`
- **D-09:** `Debt` = `AttendanceDebit` exists for this `CorrelationId` but balance went to zero or below when it was applied
- **D-10:** `ProcessedAttendanceEvents(CorrelationId, LastStatus)` table in Payments DB — consumer reads last known status before processing each event
- **D-11:** State machine logic for incoming events (see CONTEXT.md for full decision table)
- **D-12:** Every status cycle creates a new Debit/Credit
- **D-13:** MassTransit inbox handles duplicate message-id idempotency; `ProcessedAttendanceEvents` handles same-CorrelationId state machine logic
- **D-14:** Payment status shown in `GET /slots/{slotId}/attendance` response from Scheduling
- **D-15:** Scheduling calls Payments via gRPC `GetSlotPaymentStatus(LessonSlotId)` → returns list of `{ StudentId, PaymentStatus }`
- **D-16:** If Payments has no events for the slot, it returns an empty list — Scheduling maps missing students to `Unpaid`
- **D-17:** Payments gRPC server derives status from `ProcessedAttendanceEvents` joined with `LessonTransaction`

### Claude's Discretion

- EF Core configuration details for `StudentBalance` and `LessonTransaction` (index strategy, TPH vs. separate tables for transaction types)
- gRPC proto file placement (follow Phase 02-01 pattern: proto in Chassis with `GrpcServices=Both`)
- Whether `ProcessedAttendanceEvents` lives inside `StudentBalance` aggregate boundary or as a separate infrastructure table
- Exact Payments permissions (e.g., `payments.manage-balance`, `payments.view-balance`) — follow SchedulingPermissions pattern

### Deferred Ideas (OUT OF SCOPE)

- Transaction history for student (PAY-V2-03)
- Low balance notifications (PAY-V2-02)
- Reversal/cancellation when a slot is deleted
- Blocking debit at zero balance
- Package concept as a first-class entity
</user_constraints>

<phase_requirements>
## Phase Requirements

| ID | Description | Research Support |
|----|-------------|------------------|
| PAY-01 | Manager can manually add credits (lessons) to a student's balance within a school | `AddCredit` command → `LessonTransaction` with type `ManualCredit` in `StudentBalance` aggregate |
| PAY-02 | On receiving `AttendanceRecorded`, auto-debit 1 lesson (idempotent) | MassTransit inbox (message-id) + `ProcessedAttendanceEvents` state machine; `AttendanceDebit` transaction |
| PAY-03 | Student and manager see balance: purchased / used / remaining | Query sums Credits minus Debits from `LessonTransaction` at query time |
| PAY-04 | Per-slot payment status (paid / unpaid / debt) visible on lesson slot | gRPC `GetSlotPaymentStatus(LessonSlotId)` from Payments; Scheduling enriches `AttendanceRecordDto` |
| PAY-05 | Balance stored as append-only ledger (LessonTransaction with Credit/Debit) | `LessonTransaction` entity with no UPDATE path — inserts only |
| PAY-06 | Manager can manually apply credit or debit adjustment | `AddAdjustment` command → `ManualCredit` or `ManualDebit` transaction type |
</phase_requirements>

## Summary

Phase 5 creates a new `Edvantix.Payments` microservice from scratch, closely mirroring the Scheduling service pattern. The domain model is intentionally simple: a `StudentBalance` aggregate root owns an append-only collection of `LessonTransaction` entries. All balance and payment-status figures are computed at query time from the ledger — no denormalized counters.

The most non-trivial element is the MassTransit Kafka consumer (Plan 05-03). MassTransit's `AddEntityFrameworkInbox` handles duplicate message-id delivery. A separate `ProcessedAttendanceEvents` table handles the same-CorrelationId state machine — tracking whether the last known status was `Present` so the consumer knows whether to add a debit or a reversal on the next message for the same student-slot pairing.

The gRPC server (`GetSlotPaymentStatus`) is called by Scheduling when serving `GET /slots/{slotId}/attendance`. The proto lives in Chassis following the established `GrpcServices=Both` pattern from Phase 02-01. Scheduling adds a gRPC client; Payments exposes a gRPC server.

**Primary recommendation:** Build the Payments service as a strict structural copy of Scheduling (Extensions.cs, DbContext, EF config, PermissionSeeder, Aspire wiring) and focus implementation effort on the two novel pieces: the `ProcessedAttendanceEvents` state machine and the per-slot payment status gRPC derivation.

## Standard Stack

### Core

| Library | Version | Purpose | Why Standard |
|---------|---------|---------|--------------|
| MassTransit | Same as Scheduling | Kafka consumer + inbox | Already wired in all services via `AddEventBus` in Chassis |
| MassTransit.EntityFrameworkCore | Same as Scheduling | `AddEntityFrameworkInbox`, `InboxState` entity | Required for MassTransit inbox idempotency |
| Grpc.AspNetCore | Same as Organizations | gRPC server hosting | Established pattern from Phase 02-01 |
| Google.Protobuf / Grpc.Tools | Same as Chassis | Proto compilation | Already in Chassis; proto added there |
| Mediator (Mediator.SourceGenerator) | Same as all services | CQRS dispatch | Project-wide standard |
| Entity Framework Core (Npgsql) | Same as Scheduling | Persistence | PostgreSQL, same as all services |
| Ardalis.GuardClauses / Chassis Guard | Same as all services | Input validation in aggregates | `Guard.Against.Default`, `Guard.Against.NegativeOrZero` |

### Supporting

| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| TUnit | Same as test projects | Unit tests | New `Edvantix.Payments.UnitTests` project |
| Moq | Same as test projects | Mocking in unit tests | Test project dependency |
| ArchUnitNET | Same as `Edvantix.ArchTests` | Architecture tests | Payments tenant isolation test (new test class in existing project) |
| Microsoft.EntityFrameworkCore.InMemory | Same as test projects | In-memory DB for arch tests | Payments tenant isolation test |

**Installation:** No new packages required — all dependencies are already present in `Directory.Packages.props`. Payments `.csproj` mirrors Scheduling `.csproj` package references.

**Version verification:** All versions inherit from `Directory.Packages.props` — central package management enforced. Do not specify versions in individual `.csproj` files.

## Architecture Patterns

### Recommended Project Structure

```
src/Services/Payments/
└── Edvantix.Payments/
    ├── IPaymentsApiMarker.cs
    ├── GlobalUsings.cs
    ├── Program.cs
    ├── Configurations/
    │   └── PaymentsAppSettings.cs
    ├── Domain/
    │   └── AggregatesModel/
    │       └── StudentBalanceAggregate/
    │           ├── StudentBalance.cs            # Aggregate root
    │           ├── LessonTransaction.cs         # Child entity (append-only)
    │           ├── LessonTransactionType.cs     # Enum: ManualCredit, ManualDebit, AttendanceDebit, AttendanceReversal
    │           └── IStudentBalanceRepository.cs
    ├── Extensions/
    │   └── Extensions.cs                        # AddApplicationServices
    ├── Features/
    │   ├── Balance/
    │   │   ├── AddCredit/
    │   │   │   ├── AddCreditCommand.cs
    │   │   │   ├── AddCreditCommandHandler.cs
    │   │   │   ├── AddCreditCommandValidator.cs
    │   │   │   └── AddCreditEndpoint.cs
    │   │   ├── AddAdjustment/
    │   │   │   ├── AddAdjustmentCommand.cs
    │   │   │   ├── AddAdjustmentCommandHandler.cs
    │   │   │   ├── AddAdjustmentCommandValidator.cs
    │   │   │   └── AddAdjustmentEndpoint.cs
    │   │   └── GetBalance/
    │   │       ├── GetBalanceQuery.cs
    │   │       ├── GetBalanceQueryHandler.cs
    │   │       ├── GetBalanceDto.cs
    │   │       └── GetBalanceEndpoint.cs
    │   └── SlotPaymentStatus/
    │       └── GetSlotPaymentStatus/
    │           ├── GetSlotPaymentStatusQuery.cs   # consumed by gRPC service
    │           └── GetSlotPaymentStatusQueryHandler.cs
    ├── Infrastructure/
    │   ├── PaymentsDbContext.cs
    │   ├── PaymentsDbContextFactory.cs
    │   ├── Extensions.cs                          # AddPersistenceServices
    │   ├── EntityConfigurations/
    │   │   ├── StudentBalanceConfiguration.cs
    │   │   ├── LessonTransactionConfiguration.cs
    │   │   └── ProcessedAttendanceEventConfiguration.cs
    │   ├── Repositories/
    │   │   └── StudentBalanceRepository.cs
    │   ├── Seeding/
    │   │   └── PermissionSeeder.cs
    │   └── Migrations/
    ├── IntegrationEvents/
    │   └── EventHandlers/
    │       ├── AttendanceRecordedIntegrationEventHandler.cs
    │       └── AttendanceRecordedIntegrationEventHandlerDefinition.cs
    └── Grpc/
        └── Services/
            └── PaymentsGrpcService.cs

tests/
└── Edvantix.Payments.UnitTests/
    ├── Edvantix.Payments.UnitTests.csproj
    ├── GlobalUsings.cs
    └── Domain/
        └── StudentBalanceAggregateTests.cs

# Proto in Chassis (GrpcServices=Both):
src/BuildingBlocks/Edvantix.Chassis/Security/Protos/payments/v1/payments.proto
```

### Pattern 1: StudentBalance Aggregate with Append-Only Ledger

**What:** `StudentBalance` is the aggregate root (keyed on `StudentId + SchoolId`). `LessonTransaction` entries are child entities — they are always inserted, never updated or deleted.

**When to use:** Any debit or credit operation must go through `StudentBalance.AddTransaction(...)`.

**Key design choice (Claude's Discretion):** Use a single `LessonTransaction` table (not TPH inheritance) — all transaction types share the same columns (`Id`, `StudentBalanceId`, `Type`, `Amount`, `CorrelationId`, `CreatedAt`). The `CorrelationId` column is nullable — it is populated only for `AttendanceDebit` and `AttendanceReversal` entries (to link back to the attendance event chain).

**Example:**
```csharp
// Source: mirrors AttendanceRecord aggregate pattern from Phase 04-01
public sealed class StudentBalance : Entity, IAggregateRoot, ITenanted
{
    public Guid SchoolId { get; private set; }
    public Guid StudentId { get; private set; }

    // EF Core navigation — loaded when needed, not always
    private readonly List<LessonTransaction> _transactions = [];
    public IReadOnlyList<LessonTransaction> Transactions => _transactions.AsReadOnly();

    private StudentBalance() { }

    public StudentBalance(Guid schoolId, Guid studentId)
    {
        Guard.Against.Default(schoolId, nameof(schoolId));
        Guard.Against.Default(studentId, nameof(studentId));
        SchoolId = schoolId;
        StudentId = studentId;
    }

    public void AddTransaction(LessonTransactionType type, int amount, Guid? correlationId = null)
    {
        Guard.Against.NegativeOrZero(amount, nameof(amount));
        _transactions.Add(new LessonTransaction(type, amount, correlationId));
    }
}
```

### Pattern 2: ProcessedAttendanceEvents — Infrastructure Table Outside Aggregate

**What:** `ProcessedAttendanceEvents(CorrelationId PK, LastStatus string, UpdatedAt DateTimeOffset)` is an infrastructure-layer idempotency table. It does NOT implement `ITenanted` and has no `HasQueryFilter` (similar to the `Permission` entity in Organizations — a global infrastructure record).

**Reasoning (Claude's Discretion):** Placing it outside the `StudentBalance` aggregate boundary avoids polluting the domain model with infrastructure concerns. The consumer handler loads and upserts it directly via `PaymentsDbContext` without going through a domain aggregate. This matches the "infrastructure table" precedent set by MassTransit's own `InboxState` / `OutboxMessage` tables.

**When to use:** The `AttendanceRecordedIntegrationEventHandler` reads `ProcessedAttendanceEvents` before deciding what `LessonTransaction` to create.

### Pattern 3: MassTransit Inbox — Consumer with ConsumerDefinition

**What:** MassTransit's `AddEntityFrameworkInbox` deduplicates by message-id before the consumer body runs. The consumer implementation is `IConsumer<AttendanceRecordedIntegrationEvent>` with a paired `ConsumerDefinition`.

**Example (mirrors Notification service pattern):**
```csharp
// Source: Notification/IntegrationEvents/EventHandlers/SendInAppNotificationIntegrationEventHandler.cs
public sealed class AttendanceRecordedIntegrationEventHandler(
    PaymentsDbContext dbContext,
    ILogger<AttendanceRecordedIntegrationEventHandler> logger
) : IConsumer<AttendanceRecordedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<AttendanceRecordedIntegrationEvent> context)
    {
        var msg = context.Message;
        var processed = await dbContext.ProcessedAttendanceEvents
            .FirstOrDefaultAsync(p => p.CorrelationId == msg.CorrelationId);

        var lastStatus = processed?.LastStatus;
        var newStatus = msg.Status; // "Present", "Absent", "Excused"

        // State machine (D-11):
        // lastStatus null AND new = "Present"   → AttendanceDebit
        // lastStatus = "Present" AND new ≠ "Present" → AttendanceReversal
        // lastStatus ≠ "Present" AND new = "Present" → AttendanceDebit
        // lastStatus = "Present" AND new = "Present" → no-op

        var balance = await dbContext.StudentBalances
            .Include(b => b.Transactions)
            .FirstOrDefaultAsync(b => b.StudentId == msg.StudentId && b.SchoolId == msg.SchoolId);

        // ... apply transaction, upsert ProcessedAttendanceEvents

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}

[ExcludeFromCodeCoverage]
public sealed class AttendanceRecordedIntegrationEventHandlerDefinition
    : ConsumerDefinition<AttendanceRecordedIntegrationEventHandler>
{
    public AttendanceRecordedIntegrationEventHandlerDefinition()
    {
        Endpoint(x => x.Name = "payments-attendance-recorded");
        ConcurrentMessageLimit = 5;
    }
}
```

**CRITICAL:** The `AddEventBus` call in Payments `Extensions.cs` must include:
```csharp
cfg.AddEntityFrameworkInbox<PaymentsDbContext>(o =>
{
    o.QueryDelay = TimeSpan.FromSeconds(1);
    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
    o.UsePostgres();
});
```

And `PaymentsDbContext.OnModelCreating` must call `modelBuilder.AddInboxStateEntity()`.

The Payments service does NOT need `AddEntityFrameworkOutbox` — it only consumes, it does not produce integration events.

### Pattern 4: gRPC Server for GetSlotPaymentStatus

**What:** Proto in Chassis (`src/BuildingBlocks/Edvantix.Chassis/Security/Protos/payments/v1/payments.proto`) with `GrpcServices=Both` and `csharp_namespace = "Edvantix.Payments.Grpc.Generated"`. Payments hosts the server. Scheduling adds a gRPC client.

**Proto design:**
```protobuf
edition = "2023";
option csharp_namespace = "Edvantix.Payments.Grpc.Generated";
package payments.v1;

service PaymentsGrpcService {
  rpc GetSlotPaymentStatus (GetSlotPaymentStatusRequest)
      returns (GetSlotPaymentStatusReply);
}

message GetSlotPaymentStatusRequest {
  string lesson_slot_id = 1;
}

message StudentPaymentStatus {
  string student_id = 1;
  string payment_status = 2;  // "Paid", "Debt"
}

message GetSlotPaymentStatusReply {
  repeated StudentPaymentStatus statuses = 1;
}
```

**Server implementation (Payments):**
```csharp
// Source: mirrors PermissionsGrpcService.cs in Organizations
public sealed class PaymentsGrpcService(IMediator mediator)
    : Generated.PaymentsGrpcService.PaymentsGrpcServiceBase
{
    public override async Task<GetSlotPaymentStatusReply> GetSlotPaymentStatus(
        GetSlotPaymentStatusRequest request,
        ServerCallContext context)
    {
        var slotId = Guid.Parse(request.LessonSlotId);
        var statuses = await mediator.Send(
            new GetSlotPaymentStatusQuery(slotId),
            context?.CancellationToken ?? CancellationToken.None);

        var reply = new GetSlotPaymentStatusReply();
        reply.Statuses.AddRange(statuses.Select(s => new StudentPaymentStatus
        {
            StudentId = s.StudentId.ToString(),
            PaymentStatus = s.PaymentStatus
        }));
        return reply;
    }
}
```

**Scheduling-side client:** Follows `OrganizationsGroupService.cs` pattern — wrap generated client in an interface `IPaymentsGrpcService`, inject into `GetSlotAttendanceQueryHandler`.

**D-16 mapping in Scheduling:** After gRPC call returns `{ StudentId, PaymentStatus }` list, any student in `AttendanceRecordDto` whose `StudentId` is NOT in the gRPC response is mapped to `Unpaid`.

### Pattern 5: EF Core — PaymentsDbContext

**What:** Mirrors `SchedulingDbContext` exactly. Tenant filter on `StudentBalance`. `ProcessedAttendanceEvents` has NO filter (infrastructure table, not tenanted).

```csharp
// Source: mirrors SchedulingDbContext.cs
public sealed class PaymentsDbContext(
    DbContextOptions<PaymentsDbContext> options,
    ITenantContext tenantContext
) : DbContext(options), IUnitOfWork
{
    public DbSet<StudentBalance> StudentBalances => Set<StudentBalance>();
    public DbSet<ProcessedAttendanceEvent> ProcessedAttendanceEvents
        => Set<ProcessedAttendanceEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();     // MassTransit inbox
        // No OutboxMessageEntity — Payments does not publish
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsDbContext).Assembly);

        // Tenant isolation on StudentBalance (and its owned LessonTransaction)
        modelBuilder
            .Entity<StudentBalance>()
            .HasQueryFilter(b => b.SchoolId == tenantContext.SchoolId);

        // ProcessedAttendanceEvents: NO filter — infrastructure table
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}
```

**LessonTransaction EF config:**
```csharp
// LessonTransaction is a child entity owned by StudentBalance
// Use separate table (not owned type) because collection-owned types
// have limited query support in EF Core
builder.HasMany(b => b.Transactions)
    .WithOne()
    .HasForeignKey("StudentBalanceId")
    .IsRequired()
    .OnDelete(DeleteBehavior.Cascade);

// Index for balance queries (sum by type)
builder.HasIndex(t => new { t.StudentBalanceId, t.Type });

// Index for CorrelationId lookup in payment status queries
builder.HasIndex(t => t.CorrelationId)
    .HasFilter("\"CorrelationId\" IS NOT NULL");
```

### Pattern 6: Payments Permissions

**What:** New static class `PaymentsPermissions` in `Edvantix.Constants/Permissions/`, following `SchedulingPermissions.cs` exactly.

```csharp
public static class PaymentsPermissions
{
    public const string ManageBalance = "payments.manage-balance";
    public const string ViewBalance   = "payments.view-balance";

    public static IReadOnlyList<string> All => [ManageBalance, ViewBalance];
}
```

`ManageBalance` — required for `AddCredit` and `AddAdjustment` commands (manager only).
`ViewBalance` — required for `GetBalance` query (student and manager).

### Pattern 7: Aspire Wiring

**What:** Add Payments to `AppHost.cs`, following the Scheduling pattern exactly.

```csharp
// In AppHost.cs — add after schedulingDb declaration
var paymentsDb = postgres.AddDatabase(Components.Database.Payments);

// Add Payments constant to Components.Database
public static readonly string Payments = $"{nameof(Payments).ToLowerInvariant()}db";

// Add Payments constant to Services
public static readonly string Payments = nameof(Payments).ToLowerInvariant();

var paymentsApi = builder
    .AddProject<Edvantix_Payments>(Services.Payments)
    .WithReference(paymentsDb)
    .WaitFor(paymentsDb)
    .WithKeycloak(keycloak)
    .WaitFor(keycloak)
    .WithReference(organizationsApi)
    .WaitFor(organizationsApi)
    .WithReference(queue)
    .WaitFor(queue)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls();

// Add to gateway
gateway = builder
    .AddApiGatewayProxy()
    // ... existing services ...
    .WithService(paymentsApi, true)
    .Build();

// Add to Scalar (if in RunMode)
.WithOpenAPI(paymentsApi)
```

**Note:** Payments does NOT reference `schedulingApi` or `personaApi` directly — it only consumes from Kafka and exposes a gRPC server that Scheduling calls. Scheduling must reference `paymentsApi`.

### Anti-Patterns to Avoid

- **Stored balance counter:** Never store `Remaining` as a column — it becomes stale. Compute at query time from `LessonTransaction` sums.
- **Updating LessonTransaction rows:** The ledger is append-only (PAY-05). Corrections use new `AttendanceReversal` or `ManualDebit` entries.
- **Blocking on zero balance:** D-05 is explicit — allow negative. No `Guard.Against.Negative` on the running balance.
- **Skipping `AddInboxStateEntity`:** MassTransit inbox requires it in `OnModelCreating` — without it, the inbox table is missing and duplicate messages will be processed twice.
- **Using `ExecuteUpdate` for state changes:** Follow the Scheduling precedent — use EF change tracking for any domain-event-bearing operation.
- **Placing `ProcessedAttendanceEvents` inside `StudentBalance` navigation:** Keep it as a standalone entity accessed via `PaymentsDbContext` directly in the consumer. Adding it to the aggregate creates a problematic owned collection with poor query semantics.

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Kafka consumer duplicate detection | Custom dedup table keyed on message-id | `AddEntityFrameworkInbox` + `modelBuilder.AddInboxStateEntity()` | MassTransit inbox handles message-id dedup with EF atomic write in same transaction |
| Kafka consumer registration | Manual `IKafkaReceiveEndpoint` config | `AddEventBus(typeof(IPaymentsApiMarker))` in Chassis Extensions | Auto-discovers `IConsumer<T>` types by reflection — zero manual registration |
| gRPC proto compilation | Manually write generated C# | Add `.proto` to Chassis with `GrpcServices=Both` — compiler generates client and server stubs | Established pattern, avoids CS0436 duplicate type conflicts |
| Tenant isolation | Per-query `WHERE SchoolId = ?` | `HasQueryFilter` in `PaymentsDbContext.OnModelCreating` + arch test | Single filter applied globally; arch test catches regressions |
| Permission seeding | Direct DB insert | HTTP POST to `/v1/permissions/register` via named `HttpClient` with `AddStandardResilienceHandler` | Scheduling owns the Permission aggregate; same IHostedService pattern as Scheduling |
| Migrations on startup | Manual migration invocation | `services.AddMigration<PaymentsDbContext>()` from Chassis `MigrateDbContextExtensions` | Idempotent, retried on startup race |

**Key insight:** The Chassis `AddEventBus` extension discovers consumers by scanning the assembly for `IConsumer<T>` implementations and producers by scanning for `IntegrationEvent` subtypes. Payments only consumes — place `AttendanceRecordedIntegrationEvent` in the consumer assembly under `Edvantix.Contracts` namespace (no duplicate definition needed since it is already in Scheduling; Payments must reference the Scheduling assembly or a shared contracts project that contains it).

**CRITICAL — Contracts sharing:** `AttendanceRecordedIntegrationEvent` is declared in `Edvantix.Scheduling` with `namespace Edvantix.Contracts`. `DiscoverMessageTypes` in Chassis `Extensions.cs` scans the assembly passed via `typeof(IPaymentsApiMarker)`. Since the event lives in the Scheduling assembly, Payments must either (a) add a `ProjectReference` to `Edvantix.Scheduling` or (b) redeclare the record in the Payments assembly under the same `Edvantix.Contracts` namespace. Option (b) is preferred — it keeps services decoupled and avoids a circular-ish reference. The consumer-side record just needs to match the wire format exactly (same field names, same order).

## Common Pitfalls

### Pitfall 1: Inbox Not Wired — Double-Debit on Retry

**What goes wrong:** If `AddEntityFrameworkInbox` is called without `modelBuilder.AddInboxStateEntity()`, EF migrations will not create the inbox tables. MassTransit will still run but silently skip inbox deduplication, causing double-debits on Kafka retries.

**Why it happens:** `AddInboxStateEntity` is a separate call that must appear in `OnModelCreating`. The MassTransit configuration call and the model call are independent.

**How to avoid:** Check `SchedulingDbContext.OnModelCreating` — it calls both `AddInboxStateEntity()` and `AddOutboxMessageEntity()`. Payments only needs `AddInboxStateEntity()` (no outbox). Verify with a migration inspection that `inbox_state` table appears.

**Warning signs:** Log line "Inbox state entity not found" or duplicate `AttendanceDebit` transactions for same `CorrelationId`.

### Pitfall 2: HasQueryFilter on LessonTransaction — Broken Balance Calculation

**What goes wrong:** If `HasQueryFilter` is applied to `LessonTransaction` (the child entity) independently, EF Core may silently filter out some transaction rows when loading via navigation property, producing incorrect balance sums.

**Why it happens:** EF Core applies query filters to entity types independently. If `LessonTransaction` has its own filter, loading via `StudentBalance.Transactions` still applies it. Since `LessonTransaction` is not `ITenanted` (it is owned by a tenanted aggregate root), it must NOT have its own `HasQueryFilter`. Tenant isolation is guaranteed through the parent's filter.

**How to avoid:** Only register `HasQueryFilter` on `StudentBalance`. `LessonTransaction` has no filter — rely on the parent FK constraint for data isolation.

### Pitfall 3: ProcessedAttendanceEvents Race Condition on Concurrent Delivery

**What goes wrong:** If two Kafka partitions deliver the same `CorrelationId` simultaneously to different consumer instances, both could read `LastStatus = null` and both create an `AttendanceDebit`.

**Why it happens:** `AttendanceRecordedIntegrationEvent` is published per-student per-slot. If Kafka has multiple partitions and the student's messages are routed to different partitions, concurrent delivery is possible.

**How to avoid:** Use a unique index on `ProcessedAttendanceEvents.CorrelationId` (it is the PK) with a PostgreSQL `INSERT ... ON CONFLICT DO UPDATE` (upsert). EF Core does not support native upsert — use `ExecuteUpdate` or a raw SQL call for the upsert of `ProcessedAttendanceEvents`. Alternatively, set `ConcurrentMessageLimit = 1` in `ConsumerDefinition` to force sequential processing per consumer instance (simplest option for v1).

**Warning signs:** Duplicate `AttendanceDebit` rows with the same `CorrelationId` in `LessonTransaction`.

### Pitfall 4: gRPC Proto Namespace Collision (CS0436)

**What goes wrong:** If the proto's `csharp_namespace` matches an existing class in the service (e.g., `Edvantix.Payments`), the compiler emits CS0436 for conflicting type names.

**Why it happens:** Protoc generates classes in the specified namespace. If another file in the same namespace has a class with the same name, the compiler sees a duplicate.

**How to avoid:** Use `csharp_namespace = "Edvantix.Payments.Grpc.Generated"` — the `.Grpc.Generated` suffix is the established project convention (see `organizations.proto` → `Edvantix.Organizations.Grpc.Generated`). The Payments service itself removes its own `Protobuf` entry and references the Chassis proto only.

### Pitfall 5: CorrelationId as String in Proto vs Guid in C#

**What goes wrong:** Proto3 does not have a native UUID type. If the CorrelationId is passed as `string` but parsed with `Guid.Parse`, any mis-formatted value causes a runtime exception.

**Why it happens:** `AttendanceRecordedIntegrationEvent.CorrelationId` is `Guid` in C#. When passed through gRPC, it becomes a string in proto. The server receives a string and must parse it.

**How to avoid:** Pass `lesson_slot_id` as `string` in the proto (same as the Organizations proto uses `string user_id`). Parse with `Guid.Parse` in the gRPC server implementation. The error case (invalid GUID string) should be caught and translated to `StatusCode.InvalidArgument`.

### Pitfall 6: Arch Test Does Not Cover Payments Until Test Class Added

**What goes wrong:** The existing `SchedulingTenantIsolationTests` and `TenantIsolationTests` only cover Organizations and Scheduling. A new `PaymentsTenantIsolationTests` class must be added to `Edvantix.ArchTests` — otherwise the `StudentBalance` entity has no automated guard against missing `HasQueryFilter`.

**How to avoid:** Wave 0 of Plan 05-01 must include the arch test stub. The pattern is identical to `SchedulingTenantIsolationTests` — instantiate `PaymentsDbContext` with in-memory options and a `TenantContext`, find all `ITenanted` entity types, assert each has a query filter.

## Code Examples

### Balance Query (PAY-03)

```csharp
// Source: pattern derived from append-only ledger sum
public sealed class GetBalanceQueryHandler(IStudentBalanceRepository repository)
    : IQueryHandler<GetBalanceQuery, GetBalanceDto>
{
    public async ValueTask<GetBalanceDto> Handle(
        GetBalanceQuery query,
        CancellationToken cancellationToken)
    {
        var balance = await repository.GetWithTransactionsAsync(
            query.StudentId, cancellationToken);

        if (balance is null)
            return new GetBalanceDto(query.StudentId, 0, 0, 0);

        var purchased = balance.Transactions
            .Where(t => t.Type is LessonTransactionType.ManualCredit)
            .Sum(t => t.Amount);

        var used = balance.Transactions
            .Where(t => t.Type is LessonTransactionType.AttendanceDebit)
            .Sum(t => t.Amount);

        var remaining = balance.Transactions
            .Where(t => t.Type is LessonTransactionType.ManualCredit
                     or LessonTransactionType.AttendanceReversal)
            .Sum(t => t.Amount)
            - balance.Transactions
            .Where(t => t.Type is LessonTransactionType.AttendanceDebit
                     or LessonTransactionType.ManualDebit)
            .Sum(t => t.Amount);

        return new GetBalanceDto(query.StudentId, purchased, used, remaining);
    }
}
```

### Payment Status Derivation (D-07 / D-08 / D-09)

```csharp
// GetSlotPaymentStatusQueryHandler — reads ProcessedAttendanceEvents + LessonTransaction
// For each student that has an AttendanceDebit for the slot:
//   1. Find the LessonTransaction row with CorrelationId = processedEvent.CorrelationId
//      and Type = AttendanceDebit
//   2. Determine balance at the time of that debit by summing all prior transactions
//      (all transactions with Id <= debit.Id, chronological order via UUIDv7)
//   3. If running balance before debit was > 0 → "Paid"; else → "Debt"
// Only return students with AttendanceDebit; Scheduling maps absences to "Unpaid"
```

### EF Core Migration for Append-Only Ledger

```csharp
// LessonTransactionConfiguration.cs
internal sealed class LessonTransactionConfiguration : IEntityTypeConfiguration<LessonTransaction>
{
    public void Configure(EntityTypeBuilder<LessonTransaction> builder)
    {
        builder.UseDefaultConfiguration();  // HasKey(Id), UUIDv7 default

        builder.Property(t => t.Type).IsRequired().HasConversion<string>();
        builder.Property(t => t.Amount).IsRequired();
        builder.Property(t => t.CorrelationId);  // nullable
        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        // Composite index: load all transactions for a balance quickly
        builder.HasIndex(t => t.StudentBalanceId);

        // Sparse index: CorrelationId lookups in GetSlotPaymentStatus
        builder.HasIndex(t => t.CorrelationId)
            .HasFilter("\"CorrelationId\" IS NOT NULL");
    }
}
```

## State of the Art

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| Manual inbox dedup table | `AddEntityFrameworkInbox` + `InboxState` | MassTransit 8.x | MassTransit handles message-id dedup atomically with consumer transaction |
| gRPC proto per-service | Proto in shared Chassis (`GrpcServices=Both`) | Phase 02-03 decision | Avoids CS0436 duplicate type conflict in same csharp_namespace |
| `HasQueryFilter` in `IEntityTypeConfiguration<T>` | `HasQueryFilter` in `DbContext.OnModelCreating` | Phase 01-01 decision | Required because ITenantContext injection is not available inside ApplyConfigurationsFromAssembly |

## Open Questions

1. **Balance snapshot at debit time (D-07/D-09 implementation)**
   - What we know: D-07 says "Paid" if balance was > 0 when the debit was applied
   - What's unclear: Does the ledger store a snapshot of the running balance at debit time, or must the query reconstruct it by ordering all prior transactions?
   - Recommendation: Reconstruct at query time by summing transactions with `Id <= debitId` using UUIDv7 ordering (monotonically increasing). This avoids stored state and keeps the ledger truly append-only. Store `CreatedAt` as secondary sort key if needed.

2. **`AttendanceRecordedIntegrationEvent` — own copy or cross-reference**
   - What we know: The event lives in `Edvantix.Scheduling` assembly under `Edvantix.Contracts` namespace
   - What's unclear: Whether Chassis auto-discovery will find the consumer's matching type if the record is redeclared in Payments
   - Recommendation: Redeclare the record verbatim in `Edvantix.Payments` under `Edvantix.Contracts` namespace. MassTransit matches by message type name on the wire (CloudEvents), not by CLR type identity. This preserves service decoupling (Payments does not reference Scheduling).

3. **Scheduling enrichment of `AttendanceRecordDto` with PaymentStatus**
   - What we know: D-14/D-15 require Scheduling to call Payments gRPC and return payment status in the attendance response
   - What's unclear: The gRPC call is network-bound; if Payments is down, `GET /slots/{slotId}/attendance` fails
   - Recommendation: In `GetSlotAttendanceQueryHandler`, wrap the gRPC call in try/catch — on failure, return `PaymentStatus = "Unknown"` for all students and log a warning. This is a v1 resilience decision left to the planner.

## Validation Architecture

### Test Framework

| Property | Value |
|----------|-------|
| Framework | TUnit (same as all services) |
| Config file | None — uses `OutputType=Exe` pattern |
| Quick run command | `dotnet test tests/Edvantix.Payments.UnitTests` |
| Full suite command | `dotnet test` (all test projects) |

### Phase Requirements → Test Map

| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|-------------|
| PAY-01 | AddCredit adds ManualCredit transaction to StudentBalance | unit | `dotnet test tests/Edvantix.Payments.UnitTests --filter "GivenValidCredit"` | ❌ Wave 0 |
| PAY-02 | Consumer debits on Present, reversal on status change, no-op on same | unit | `dotnet test tests/Edvantix.Payments.UnitTests --filter "GivenAttendance"` | ❌ Wave 0 |
| PAY-03 | GetBalance sums purchased/used/remaining correctly | unit | `dotnet test tests/Edvantix.Payments.UnitTests --filter "GivenBalance"` | ❌ Wave 0 |
| PAY-04 | Paid/Unpaid/Debt status derived correctly from ledger | unit | `dotnet test tests/Edvantix.Payments.UnitTests --filter "GivenSlotStatus"` | ❌ Wave 0 |
| PAY-05 | Ledger is append-only — no Update path exists | arch/unit | `dotnet test tests/Edvantix.ArchTests` | partial (arch test class ❌ Wave 0) |
| PAY-06 | AddAdjustment adds ManualCredit or ManualDebit | unit | `dotnet test tests/Edvantix.Payments.UnitTests --filter "GivenAdjustment"` | ❌ Wave 0 |

### Sampling Rate

- **Per task commit:** `dotnet test tests/Edvantix.Payments.UnitTests`
- **Per wave merge:** `dotnet test` (full suite)
- **Phase gate:** Full suite green before `/gsd:verify-work`

### Wave 0 Gaps

- [ ] `tests/Edvantix.Payments.UnitTests/` — new test project with `.csproj` mirroring `Edvantix.Scheduling.UnitTests.csproj`
- [ ] `tests/Edvantix.Payments.UnitTests/GlobalUsings.cs` — shared usings
- [ ] `tests/Edvantix.Payments.UnitTests/Domain/StudentBalanceAggregateTests.cs` — Wave 0 stubs for PAY-01, PAY-02, PAY-03, PAY-04, PAY-06
- [ ] `tests/Edvantix.ArchTests/Domain/PaymentsTenantIsolationTests.cs` — new arch test class for PAY-05 isolation guarantee
- [ ] Add `Edvantix.Payments` to `Edvantix.ArchTests` project references

## Sources

### Primary (HIGH confidence)

- `src/Services/Scheduling/Edvantix.Scheduling/Infrastructure/SchedulingDbContext.cs` — DbContext pattern with inbox/outbox and HasQueryFilter
- `src/Services/Scheduling/Edvantix.Scheduling/Domain/AggregatesModel/AttendanceAggregate/AttendanceRecord.cs` — aggregate pattern
- `src/BuildingBlocks/Edvantix.Chassis/EventBus/Extensions.cs` — AddEventBus auto-discovery mechanism
- `src/Services/Notification/Edvantix.Notification/IntegrationEvents/EventHandlers/SendInAppNotificationIntegrationEventHandler.cs` — IConsumer + ConsumerDefinition pattern
- `src/BuildingBlocks/Edvantix.Chassis/Security/Protos/organizations/v1/permissions.proto` — proto-in-Chassis pattern
- `src/Services/Organizations/Edvantix.Organizations/Grpc/Services/PermissionsGrpcService.cs` — gRPC server implementation pattern
- `src/Aspire/Edvantix.AppHost/AppHost.cs` — Aspire wiring pattern for new services
- `src/BuildingBlocks/Edvantix.Constants/Aspire/Components.cs` — database constant pattern
- `tests/Edvantix.ArchTests/Domain/SchedulingTenantIsolationTests.cs` — tenant isolation arch test pattern
- `tests/Edvantix.Scheduling.UnitTests/Domain/AttendanceRecordAggregateTests.cs` — unit test style and naming convention

### Secondary (MEDIUM confidence)

- STATE.md accumulated decisions — Phase 02-03 proto-in-Chassis decision, Phase 04 inbox/outbox patterns
- CONTEXT.md decisions D-01 through D-17 — authoritative design decisions from /gsd:discuss-phase

### Tertiary (LOW confidence)

- None — all findings verified from existing codebase or locked decisions

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH — all packages already in Directory.Packages.props; verified from existing service csproj files
- Architecture: HIGH — Payments is structurally identical to Scheduling; all patterns verified from source
- Pitfalls: HIGH — pitfalls derived from existing codebase decisions documented in STATE.md (Phase 01-01, 02-03, 04 entries)
- gRPC design: HIGH — proto pattern verified from `permissions.proto`; namespace convention verified from STATE.md [Phase 02-01]
- Consumer idempotency: HIGH — MassTransit inbox pattern verified from `SchedulingDbContext` + `Extensions.cs`

**Research date:** 2026-03-22
**Valid until:** 2026-04-22 (stable stack; no fast-moving dependencies)
