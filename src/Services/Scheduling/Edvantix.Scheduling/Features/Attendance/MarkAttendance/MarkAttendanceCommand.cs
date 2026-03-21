using Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

namespace Edvantix.Scheduling.Features.Attendance.MarkAttendance;

/// <summary>
/// Command to mark or update a student's attendance for a specific lesson slot.
/// <para>
/// This command implements upsert semantics (D-06): if no record exists for the
/// (slotId, studentId) pair, a new <see cref="AttendanceRecord"/> is created.
/// If a record already exists, its status is updated via <see cref="AttendanceRecord.UpdateStatus"/>.
/// </para>
/// </summary>
/// <param name="SlotId">The lesson slot to mark attendance for.</param>
/// <param name="StudentId">The student whose attendance is being marked.</param>
/// <param name="Status">The attendance status to record.</param>
public sealed record MarkAttendanceCommand(
    Guid SlotId,
    Guid StudentId,
    AttendanceStatus Status
) : ICommand<Unit>;
