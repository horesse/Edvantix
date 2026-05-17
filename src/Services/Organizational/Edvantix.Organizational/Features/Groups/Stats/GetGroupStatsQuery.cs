using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Groups.Stats;

[RequirePermission(GroupPermissions.View)]
public sealed record GetGroupStatsQuery : IQuery<GroupStatsDto>;

internal sealed class GetGroupStatsQueryHandler(
    ITenantContext tenantContext,
    IGroupRepository repository
) : IQueryHandler<GetGroupStatsQuery, GroupStatsDto>
{
    public async ValueTask<GroupStatsDto> Handle(
        GetGroupStatsQuery request,
        CancellationToken cancellationToken
    )
    {
        var organizationId = tenantContext.OrganizationId;

        // Один SQL-запрос: проекция статус + вместимость + активные участники на группу.
        var rows = await repository.GetStatsProjectionAsync(organizationId, cancellationToken);

        var total = rows.Count;
        var active = rows.Count(r => r.Status == GroupStatus.Active);
        var recruiting = rows.Count(r => r.Status == GroupStatus.Recruiting);
        var paused = rows.Count(r => r.Status == GroupStatus.Paused);
        var finished = rows.Count(r => r.Status == GroupStatus.Finished);
        var archived = rows.Count(r => r.Status == GroupStatus.Archived);

        var totalActiveStudents = rows.Where(r => r.Status == GroupStatus.Active)
            .Sum(r => r.ActiveMemberCount);

        var nonArchived = rows.Where(r => r.Status != GroupStatus.Archived).ToList();
        var totalCapacity = nonArchived.Sum(r => r.Capacity);
        var totalFilledSeats = nonArchived.Sum(r => r.ActiveMemberCount);

        var fillRatePercent =
            totalCapacity == 0
                ? 0
                : (int)Math.Round((double)totalFilledSeats * 100 / totalCapacity);

        return new GroupStatsDto(
            total,
            active,
            recruiting,
            paused,
            finished,
            archived,
            totalActiveStudents,
            totalCapacity,
            totalFilledSeats,
            fillRatePercent
        );
    }
}
