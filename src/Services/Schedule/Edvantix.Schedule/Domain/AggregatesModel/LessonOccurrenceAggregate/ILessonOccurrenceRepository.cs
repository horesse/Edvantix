using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;

namespace Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;

/// <summary>Репозиторий агрегата <see cref="LessonOccurrence"/>.</summary>
public interface ILessonOccurrenceRepository : IRepository<LessonOccurrence>
{
    Task<IReadOnlyList<LessonOccurrence>> ListAsync(
        ISpecification<LessonOccurrence> specification,
        CancellationToken cancellationToken = default
    );

    Task AddRangeAsync(
        IEnumerable<LessonOccurrence> occurrences,
        CancellationToken cancellationToken = default
    );

    Task DeleteByScheduleIdAsync(Guid scheduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает агрегированные счётчики занятий для нескольких групп одним запросом.
    /// Группы без занятий в результирующий словарь не включаются.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, OccurrenceSummary>> GetSummariesByGroupIdsAsync(
        IEnumerable<Guid> groupIds,
        DateOnly today,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Возвращает первые <paramref name="count"/> предстоящих занятий группы начиная с <paramref name="from"/>,
    /// отсортированных по дате и времени начала.
    /// </summary>
    Task<IReadOnlyList<LessonOccurrence>> GetUpcomingByGroupIdAsync(
        Guid groupId,
        DateOnly from,
        int count,
        CancellationToken cancellationToken = default
    );
}
