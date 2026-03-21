namespace Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

/// <summary>
/// Repository for <see cref="AttendanceRecord"/> aggregate roots.
/// Use <see cref="IRepository{T}.UnitOfWork"/> to save changes.
/// AttendanceRecord is a separate aggregate from LessonSlot (D-01).
/// Queries accept <see cref="Specification{T}"/> to keep the interface generic
/// and avoid proliferating typed filter methods.
/// </summary>
public interface IAttendanceRecordRepository : IRepository<AttendanceRecord>
{
    /// <summary>
    /// Returns the first attendance record that satisfies <paramref name="spec"/>,
    /// or <see langword="null"/> if none exists.
    /// </summary>
    /// <param name="spec">The specification to apply.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<AttendanceRecord?> FirstOrDefaultAsync(
        Specification<AttendanceRecord> spec,
        CancellationToken ct
    );

    /// <summary>
    /// Returns all attendance records that satisfy <paramref name="spec"/>.
    /// </summary>
    /// <param name="spec">The specification to apply.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<IReadOnlyList<AttendanceRecord>> ListAsync(
        Specification<AttendanceRecord> spec,
        CancellationToken ct
    );

    /// <summary>Adds a new attendance record to the context. Call <see cref="IUnitOfWork.SaveEntitiesAsync"/> to persist.</summary>
    void Add(AttendanceRecord record);
}
