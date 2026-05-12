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
}
