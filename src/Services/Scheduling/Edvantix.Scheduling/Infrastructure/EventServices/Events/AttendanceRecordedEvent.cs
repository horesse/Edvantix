using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Scheduling.Infrastructure.EventServices.Events;

/// <summary>
/// Domain event raised when an attendance record is created or its status is updated.
/// Published on both new record creation and existing record update via <see cref="AttendanceRecord.UpdateStatus"/>.
/// <para>
/// Status is stored as a string (not enum) per D-12 so the integration event contract is decoupled
/// from the Scheduling service's internal enum definition.
/// </para>
/// </summary>
public sealed class AttendanceRecordedEvent(
    Guid correlationId,
    Guid studentId,
    Guid lessonSlotId,
    Guid schoolId,
    string status,
    DateTimeOffset markedAt
) : DomainEvent
{
    /// <summary>Gets the idempotency correlation identifier, preserved from the AttendanceRecord.</summary>
    public Guid CorrelationId { get; } = correlationId;

    /// <summary>Gets the student whose attendance was recorded.</summary>
    public Guid StudentId { get; } = studentId;

    /// <summary>Gets the lesson slot this attendance belongs to.</summary>
    public Guid LessonSlotId { get; } = lessonSlotId;

    /// <summary>Gets the school (tenant) that owns this record.</summary>
    public Guid SchoolId { get; } = schoolId;

    /// <summary>
    /// Gets the attendance status as a string (e.g. "Present", "Absent").
    /// The aggregate passes <c>Status.ToString()</c> to decouple the event from the enum type.
    /// </summary>
    public string Status { get; } = status;

    /// <summary>Gets the timestamp when the attendance was marked or updated.</summary>
    public DateTimeOffset MarkedAt { get; } = markedAt;
}
