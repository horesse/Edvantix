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
    ) => await context.LessonOccurrences.AddRangeAsync(occurrences, cancellationToken);

    public async Task DeleteByScheduleIdAsync(
        Guid scheduleId,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .LessonOccurrences.Where(o => o.ScheduleId == scheduleId)
            .ExecuteDeleteAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<Guid, OccurrenceSummary>> GetSummariesByGroupIdsAsync(
        IEnumerable<Guid> groupIds,
        DateOnly today,
        CancellationToken cancellationToken = default
    )
    {
        var ids = groupIds.ToList();

        // Один запрос: для каждой группы считаем total, remaining и минимальную будущую дату.
        var rows = await context
            .LessonOccurrences.Where(o => ids.Contains(o.GroupId))
            .GroupBy(o => o.GroupId)
            .Select(g => new
            {
                GroupId = g.Key,
                Total = g.Count(),
                Remaining = g.Count(o => o.LessonDate >= today),
                NextLessonDate = g
                    .Where(o => o.LessonDate >= today)
                    .OrderBy(o => o.LessonDate)
                    .Select(o => (DateOnly?)o.LessonDate)
                    .FirstOrDefault(),
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rows.ToDictionary(
            r => r.GroupId,
            r => new OccurrenceSummary(r.Total, r.Remaining, r.NextLessonDate)
        );
    }
}
