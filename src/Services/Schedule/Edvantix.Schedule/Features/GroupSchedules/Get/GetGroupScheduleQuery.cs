using Edvantix.Chassis.CQRS;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;

namespace Edvantix.Schedule.Features.GroupSchedules.Get;

public sealed record GetGroupScheduleQuery(Guid GroupId) : IQuery<GroupScheduleDto>;

internal sealed class GetGroupScheduleQueryHandler(
    IGroupScheduleRepository repository,
    IMapper<GroupSchedule, GroupScheduleDto> mapper
) : IQueryHandler<GetGroupScheduleQuery, GroupScheduleDto>
{
    public async ValueTask<GroupScheduleDto> Handle(
        GetGroupScheduleQuery query,
        CancellationToken cancellationToken
    )
    {
        var schedule =
            await repository.GetByGroupIdAsync(query.GroupId, cancellationToken)
            ?? throw NotFoundException.For<GroupSchedule>(query.GroupId);

        return mapper.Map(schedule);
    }
}
