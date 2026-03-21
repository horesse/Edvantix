namespace Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

/// <summary>
/// Repository for <see cref="AttendanceRecord"/> aggregate roots.
/// Use <see cref="IRepository{T}.UnitOfWork"/> to save changes.
/// AttendanceRecord is a separate aggregate from LessonSlot (D-01).
/// </summary>
public interface IAttendanceRecordRepository : IRepository<AttendanceRecord>
{
    /// <summary>
    /// Finds an attendance record by the combination of lesson slot and student.
    /// Used to determine whether to create a new record or update an existing one.
    /// Returns null if no record exists for this combination.
    /// </summary>
    /// <param name="slotId">The lesson slot identifier.</param>
    /// <param name="studentId">The student identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<AttendanceRecord?> FindBySlotAndStudentAsync(
        Guid slotId,
        Guid studentId,
        CancellationToken ct
    );

    /// <summary>
    /// Returns all attendance records for a given lesson slot.
    /// Used by Plan 02 GetSlotAttendance query.
    /// </summary>
    /// <param name="slotId">The lesson slot identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<IReadOnlyList<AttendanceRecord>> GetBySlotAsync(Guid slotId, CancellationToken ct);

    /// <summary>Adds a new attendance record to the context. Call <see cref="IUnitOfWork.SaveEntitiesAsync"/> to persist.</summary>
    void Add(AttendanceRecord record);
}
