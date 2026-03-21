namespace Edvantix.Scheduling.Features.Attendance.GetSlotAttendance;

/// <summary>
/// Data transfer object representing a single attendance record for a lesson slot.
/// <para>
/// <c>Status</c> is exposed as a string (not enum) per D-07 response shape,
/// so API consumers receive a human-readable value (e.g., "Present", "Absent")
/// without needing to know the enum's numeric representation.
/// </para>
/// </summary>
/// <param name="StudentId">The student whose attendance is recorded.</param>
/// <param name="Status">The attendance status as a string (e.g., "Present", "Absent", "Late").</param>
/// <param name="CorrelationId">Idempotency identifier for the attendance event chain (D-04).</param>
/// <param name="MarkedAt">Timestamp when the attendance was last recorded or updated.</param>
public sealed record AttendanceRecordDto(
    Guid StudentId,
    string Status,
    Guid CorrelationId,
    DateTimeOffset MarkedAt
);
