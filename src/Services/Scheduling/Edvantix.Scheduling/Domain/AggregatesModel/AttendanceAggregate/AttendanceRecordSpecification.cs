namespace Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

/// <summary>
/// Finds a single attendance record by the composite natural key (LessonSlotId + StudentId).
/// Used by the upsert handler to determine whether to create or update.
/// </summary>
public sealed class AttendanceBySlotAndStudentSpecification : Specification<AttendanceRecord>
{
    /// <summary>Initializes the specification for the given slot and student.</summary>
    /// <param name="slotId">The lesson slot identifier.</param>
    /// <param name="studentId">The student identifier.</param>
    public AttendanceBySlotAndStudentSpecification(Guid slotId, Guid studentId)
    {
        Query.Where(a => a.LessonSlotId == slotId && a.StudentId == studentId);
    }
}

/// <summary>
/// Returns all attendance records for a given lesson slot.
/// Used by the GetSlotAttendance query handler.
/// </summary>
public sealed class AttendanceBySlotSpecification : Specification<AttendanceRecord>
{
    /// <summary>Initializes the specification for the given slot.</summary>
    /// <param name="slotId">The lesson slot identifier.</param>
    public AttendanceBySlotSpecification(Guid slotId)
    {
        Query.Where(a => a.LessonSlotId == slotId);
    }
}
