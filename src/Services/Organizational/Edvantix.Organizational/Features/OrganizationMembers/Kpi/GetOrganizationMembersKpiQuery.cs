using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.OrganizationMembers.Kpi;

[RequirePermission(OrganizationPermissions.View)]
public sealed record GetOrganizationMembersKpiQuery : IQuery<OrganizationMembersKpiDto>;

internal sealed class GetOrganizationMembersKpiQueryHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRepository repository
) : IQueryHandler<GetOrganizationMembersKpiQuery, OrganizationMembersKpiDto>
{
    public async ValueTask<OrganizationMembersKpiDto> Handle(
        GetOrganizationMembersKpiQuery request,
        CancellationToken cancellationToken
    )
    {
        var organizationId = tenantContext.OrganizationId;

        var totalSpec = new OrganizationMembersKpiSpecification(organizationId);
        var activeSpec = new OrganizationMembersKpiSpecification(
            organizationId,
            OrganizationStatus.Active
        );
        var archivedSpec = new OrganizationMembersKpiSpecification(
            organizationId,
            OrganizationStatus.Archived
        );
        var deletedSpec = new OrganizationMembersKpiSpecification(
            organizationId,
            OrganizationStatus.Deleted
        );

        // Последовательное выполнение: DbContext не поддерживает параллельные запросы в одном скоупе.
        var total = await repository.CountAsync(totalSpec, cancellationToken);
        var active = await repository.CountAsync(activeSpec, cancellationToken);
        var archived = await repository.CountAsync(archivedSpec, cancellationToken);
        var deleted = await repository.CountAsync(deletedSpec, cancellationToken);

        return new OrganizationMembersKpiDto(total, active, archived, deleted);
    }
}
