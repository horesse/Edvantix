using Edvantix.Chassis.Repository;

namespace Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;

/// <summary>Репозиторий агрегата <see cref="GroupSchedule"/>.</summary>
public interface IGroupScheduleRepository : IRepository<GroupSchedule>
{
    Task<GroupSchedule?> GetByGroupIdAsync(
        Guid groupId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Загружает расписания (со слотами) для нескольких групп одним запросом.
    /// </summary>
    Task<IReadOnlyList<GroupSchedule>> GetByGroupIdsAsync(
        IEnumerable<Guid> groupIds,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(GroupSchedule schedule, CancellationToken cancellationToken = default);
}
