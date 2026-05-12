using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;

namespace Edvantix.Schedule.Infrastructure.Repositories;

internal sealed class GroupScheduleRepository(ScheduleDbContext context) : IGroupScheduleRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<GroupSchedule?> GetByGroupIdAsync(
        Guid groupId,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .GroupSchedules.Include(s => s.Slots)
            .Include(s => s.Exceptions)
            .FirstOrDefaultAsync(s => s.GroupId == groupId, cancellationToken);

    public async Task AddAsync(
        GroupSchedule schedule,
        CancellationToken cancellationToken = default
    ) => await context.GroupSchedules.AddAsync(schedule, cancellationToken);
}
