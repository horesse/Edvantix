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

        var totalSpec = new GroupStatsSpecification(organizationId);
        var activeSpec = new GroupStatsSpecification(organizationId, GroupStatus.Active);
        var recruitingSpec = new GroupStatsSpecification(organizationId, GroupStatus.Recruiting);
        var pausedSpec = new GroupStatsSpecification(organizationId, GroupStatus.Paused);
        var finishedSpec = new GroupStatsSpecification(organizationId, GroupStatus.Finished);
        var archivedSpec = new GroupStatsSpecification(organizationId, GroupStatus.Archived);

        // Последовательное выполнение: DbContext не поддерживает параллельные запросы в одном скоупе.
        var total = await repository.CountAsync(totalSpec, cancellationToken);
        var active = await repository.CountAsync(activeSpec, cancellationToken);
        var recruiting = await repository.CountAsync(recruitingSpec, cancellationToken);
        var paused = await repository.CountAsync(pausedSpec, cancellationToken);
        var finished = await repository.CountAsync(finishedSpec, cancellationToken);
        var archived = await repository.CountAsync(archivedSpec, cancellationToken);

        return new GroupStatsDto(total, active, recruiting, paused, finished, archived);
    }
}
