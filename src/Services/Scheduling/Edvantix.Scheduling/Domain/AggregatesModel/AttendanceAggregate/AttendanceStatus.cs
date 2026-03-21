namespace Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

/// <summary>
/// Represents the attendance status of a student for a lesson slot.
/// Exactly four values per D-03 domain decision.
/// </summary>
public enum AttendanceStatus
{
    /// <summary>The student was present at the lesson.</summary>
    Present = 0,

    /// <summary>The student was absent from the lesson.</summary>
    Absent = 1,

    /// <summary>The student's absence was excused.</summary>
    Excused = 2,

    /// <summary>The student arrived late to the lesson.</summary>
    Late = 3,
}
