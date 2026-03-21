namespace Edvantix.Scheduling.Domain.AggregatesModel.LessonSlotAggregate;

/// <summary>
/// Repository for <see cref="LessonSlot"/> aggregate roots.
/// Use <see cref="IRepository{T}.UnitOfWork"/> to save changes.
/// </summary>
public interface ILessonSlotRepository : IRepository<LessonSlot>
{
    /// <summary>
    /// Returns the first lesson slot that satisfies <paramref name="spec"/>,
    /// or <see langword="null"/> if none exists.
    /// </summary>
    /// <param name="spec">The specification to apply.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<LessonSlot?> FirstOrDefaultAsync(Specification<LessonSlot> spec, CancellationToken ct);

    /// <summary>Adds a new lesson slot to the context. Call <see cref="IUnitOfWork.SaveEntitiesAsync"/> to persist.</summary>
    void Add(LessonSlot slot);

    /// <summary>Removes a lesson slot from the context. Lesson slots are hard-deleted.</summary>
    void Remove(LessonSlot slot);

    /// <summary>
    /// Checks whether a teacher has a conflicting lesson slot in the given time range.
    /// Uses <c>IgnoreQueryFilters</c> to check across all tenants (D-04) — a teacher cannot
    /// be double-booked even if they teach in multiple schools.
    /// Overlap predicate: StartTime &lt; endTime AND EndTime &gt; startTime (strict inequalities, no buffer).
    /// </summary>
    /// <param name="teacherId">The teacher to check for conflicts.</param>
    /// <param name="startTime">The proposed slot start time.</param>
    /// <param name="endTime">The proposed slot end time.</param>
    /// <param name="excludedSlotId">Optional slot ID to exclude (for reschedule operations).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if a conflicting slot exists; otherwise false.</returns>
    Task<bool> HasConflictAsync(
        Guid teacherId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        Guid? excludedSlotId,
        CancellationToken ct
    );
}
