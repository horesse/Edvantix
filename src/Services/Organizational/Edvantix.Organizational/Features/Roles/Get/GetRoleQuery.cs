using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.Roles.Get;

[RequirePermission(OrganizationPermissions.ManageRoles)]
public sealed record GetRoleQuery(Guid Id) : IQuery<RoleDetailDto>;

internal sealed class GetRoleQueryHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRoleRepository repository,
    IMapper<OrganizationMemberRole, RoleDetailDto> mapper
) : IQueryHandler<GetRoleQuery, RoleDetailDto>
{
    public async ValueTask<RoleDetailDto> Handle(
        GetRoleQuery query,
        CancellationToken cancellationToken
    )
    {
        var role = await repository.GetByIdWithPermissionsAsync(query.Id, cancellationToken);

        if (role is null || role.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<OrganizationMemberRole>(query.Id);

        return mapper.Map(role);
    }
}
