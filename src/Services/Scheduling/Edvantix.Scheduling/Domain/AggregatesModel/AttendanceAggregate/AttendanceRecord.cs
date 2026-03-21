namespace Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

/// <summary>
/// Aggregate root representing a student's attendance record for a specific lesson slot.
/// Separate aggregate from <c>LessonSlot</c> per D-01 — each has its own repository and lifecycle.
/// CorrelationId is generated once at creation (Guid v7, D-04) and preserved across status updates.
/// Tenant isolation is enforced via <see cref="ITenanted"/> and a HasQueryFilter in <c>SchedulingDbContext</c>.
/// </summary>
public sealed class AttendanceRecord : Entity, IAggregateRoot, ITenanted
{
    /// <summary>Gets the school (tenant) that owns this attendance record.</summary>
    public Guid SchoolId { get; private set; }

    /// <summary>Gets the lesson slot this attendance record belongs to.</summary>
    public Guid LessonSlotId { get; private set; }

    /// <summary>Gets the student whose attendance is recorded.</summary>
    public Guid StudentId { get; private set; }

    /// <summary>Gets the current attendance status of the student.</summary>
    public AttendanceStatus Status { get; private set; }

    /// <summary>
    /// Gets the idempotency correlation identifier for this record.
    /// Generated once at creation using Guid v7 (D-04) and never changed,
    /// even when <see cref="UpdateStatus"/> is called.
    /// </summary>
    public Guid CorrelationId { get; private set; }

    /// <summary>Gets the timestamp when the attendance was last marked or updated.</summary>
    public DateTimeOffset MarkedAt { get; private set; }

    // EF Core parameterless constructor
    private AttendanceRecord() { }

    /// <summary>
    /// Creates a new <see cref="AttendanceRecord"/> for the given school, slot, and student.
    /// </summary>
    /// <param name="schoolId">The tenant school. Must not be empty.</param>
    /// <param name="lessonSlotId">The lesson slot being attended. Must not be empty.</param>
    /// <param name="studentId">The student being marked. Must not be empty.</param>
    /// <param name="status">The initial attendance status.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="schoolId"/>, <paramref name="lessonSlotId"/>,
    /// or <paramref name="studentId"/> is the default <see cref="Guid"/>.
    /// </exception>
    public AttendanceRecord(
        Guid schoolId,
        Guid lessonSlotId,
        Guid studentId,
        AttendanceStatus status
    )
    {
        Guard.Against.Default(schoolId, nameof(schoolId));
        Guard.Against.Default(lessonSlotId, nameof(lessonSlotId));
        Guard.Against.Default(studentId, nameof(studentId));

        SchoolId = schoolId;
        LessonSlotId = lessonSlotId;
        StudentId = studentId;
        Status = status;

        // CorrelationId is generated once at creation using Guid v7 (D-04).
        // Monotonically increasing UUIDs improve DB index locality.
        CorrelationId = Guid.CreateVersion7();
        MarkedAt = DateTimeOffset.UtcNow;

        // TODO: Plan 02 — RegisterDomainEvent(new AttendanceRecordedEvent(Id, SchoolId, LessonSlotId, StudentId, Status, CorrelationId))
    }

    /// <summary>
    /// Updates the attendance status and refreshes the <see cref="MarkedAt"/> timestamp.
    /// <see cref="CorrelationId"/> is intentionally preserved (D-04) — the event chain stays consistent.
    /// </summary>
    /// <param name="newStatus">The updated attendance status.</param>
    public void UpdateStatus(AttendanceStatus newStatus)
    {
        Status = newStatus;
        MarkedAt = DateTimeOffset.UtcNow;

        // TODO: Plan 02 — RegisterDomainEvent(new AttendanceStatusUpdatedEvent(Id, SchoolId, LessonSlotId, StudentId, newStatus, CorrelationId))
    }
}
