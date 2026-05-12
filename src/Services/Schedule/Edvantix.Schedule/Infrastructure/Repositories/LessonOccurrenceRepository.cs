using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;

namespace Edvantix.Schedule.Infrastructure.Repositories;

internal sealed class LessonOccurrenceRepository(ScheduleDbContext context)
    : ILessonOccurrenceRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<IReadOnlyList<LessonOccurrence>> ListAsync(
        ISpecification<LessonOccurrence> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.LessonOccurrences.AsNoTracking(), specification)
            .ToListAsync(cancellationToken);

    public async Task AddRangeAsync(
        IEnumerable<LessonOccurrence> occurrences,
        CancellationToken cancellationToken = default
    ) =>
        await context.LessonOccurrences.AddRangeAsync(occurrences, cancellationToken);

    public async Task DeleteByScheduleIdAsync(
        Guid scheduleId,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .LessonOccurrences.Where(o => o.ScheduleId == scheduleId)
            .ExecuteDeleteAsync(cancellationToken);
}
