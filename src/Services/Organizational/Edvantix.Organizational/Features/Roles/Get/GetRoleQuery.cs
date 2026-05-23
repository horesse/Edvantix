using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Roles.Get;

[RequirePermission(OrganizationPermissions.Roles)]
public sealed record GetRoleQuery(Guid Id) : IQuery<RoleDetailDto>;

internal sealed class GetRoleQueryHandler(
    ITenantContext tenantContext,
    IOrganizationRoleRepository repository,
    IPermissionRepository permissionRepository,
    IOrganizationMemberRepository memberRepository,
    IMapper<OrganizationRole, RoleDetailDto> mapper
) : IQueryHandler<GetRoleQuery, RoleDetailDto>
{
    public async ValueTask<RoleDetailDto> Handle(
        GetRoleQuery query,
        CancellationToken cancellationToken
    )
    {
        var role = await repository.GetByIdWithPermissionsAsync(query.Id, cancellationToken);

        if (role is null || role.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<OrganizationRole>(query.Id);

        var dto = mapper.Map(role);

        var allPermissions = await permissionRepository.GetAllWithFeaturesAsync(cancellationToken);
        var membersCount = await memberRepository.CountByRoleAsync(query.Id, cancellationToken);

        var rolePermissionIds = role.Permissions.Select(p => p.Id).ToHashSet();

        var features = allPermissions
            .GroupBy(p => p.FeatureCode)
            .OrderBy(g => g.Key)
            .Select(g => new FeatureDto(
                g.Key,
                g.First().Feature?.Name ?? g.Key,
                g.OrderBy(p => p.Code)
                    .Select(p => new PermissionDto(
                        p.Id,
                        p.Code,
                        p.Name,
                        rolePermissionIds.Contains(p.Id)
                    ))
                    .ToList()
            ))
            .ToList();

        return dto with
        {
            Features = features,
            TotalPermissionsCount = allPermissions.Count,
            MembersCount = membersCount,
        };
    }
}
