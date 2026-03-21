namespace Edvantix.Scheduling.Features.Attendance.GetSlotAttendance;

/// <summary>
/// Query that returns all attendance records for a given lesson slot.
/// <para>
/// Used by managers and teachers to review who was present, absent, or late for a specific slot.
/// Authorization is enforced at the endpoint level via <c>scheduling.view-schedule</c> (D-09).
/// </para>
/// </summary>
/// <param name="SlotId">The identifier of the lesson slot whose attendance records are requested.</param>
public sealed record GetSlotAttendanceQuery(Guid SlotId) : IQuery<IReadOnlyList<AttendanceRecordDto>>;
