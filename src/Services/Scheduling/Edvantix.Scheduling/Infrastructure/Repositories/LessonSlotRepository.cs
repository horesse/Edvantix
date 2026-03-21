using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Scheduling.Domain.AggregatesModel.LessonSlotAggregate;

namespace Edvantix.Scheduling.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ILessonSlotRepository"/>.
/// Lesson slots are hard-deleted, so no soft-delete filtering is applied.
/// The tenant query filter is applied by <c>SchedulingDbContext</c> via HasQueryFilter.
/// Single-entity lookups use <see cref="SpecificationEvaluator"/> to stay consistent
/// with the project's Specification-over-ad-hoc-methods convention.
/// </summary>
public sealed class LessonSlotRepository(SchedulingDbContext context) : ILessonSlotRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<LessonSlot?> FirstOrDefaultAsync(
        Specification<LessonSlot> spec,
        CancellationToken ct
    ) => await Spec.GetQuery(context.LessonSlots, spec).FirstOrDefaultAsync(ct);

    /// <inheritdoc/>
    public void Add(LessonSlot slot) => context.LessonSlots.Add(slot);

    /// <inheritdoc/>
    public void Remove(LessonSlot slot) => context.LessonSlots.Remove(slot);

    /// <inheritdoc/>
    public async Task<bool> HasConflictAsync(
        Guid teacherId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        Guid? excludedSlotId,
        CancellationToken ct
    ) =>
        // IgnoreQueryFilters bypasses the tenant filter so we check for teacher conflicts across
        // ALL schools (D-04). A teacher must not be double-booked even if they teach in multiple schools.
        // Overlap predicate uses strict inequalities (D-05): two slots overlap when
        //   slotStart < proposedEnd AND slotEnd > proposedStart (no buffer).
        // Returns bool via AnyAsync — a Specification is not applicable here.
        await context
            .LessonSlots.IgnoreQueryFilters()
            .AnyAsync(
                s =>
                    s.TeacherId == teacherId
                    && s.Id != (excludedSlotId ?? Guid.Empty)
                    && s.StartTime < endTime
                    && s.EndTime > startTime,
                ct
            );
}
