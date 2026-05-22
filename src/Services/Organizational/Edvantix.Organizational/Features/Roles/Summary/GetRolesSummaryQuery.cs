using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Domain.Permissions;
using ZiggyCreatures.Caching.Fusion;

namespace Edvantix.Organizational.Features.Roles.Summary;

[RequirePermission(OrganizationPermissions.Roles)]
public sealed record GetRolesSummaryQuery : IQuery<RolesSummaryDto>;

internal sealed class GetRolesSummaryQueryHandler(
    ITenantContext tenantContext,
    IOrganizationRoleRepository roleRepository,
    IOrganizationMemberRepository memberRepository,
    IFusionCache cache
) : IQueryHandler<GetRolesSummaryQuery, RolesSummaryDto>
{
    public async ValueTask<RolesSummaryDto> Handle(
        GetRolesSummaryQuery query,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;
        var cacheKey = $"org:{orgId}:roles-summary";

        return await cache.GetOrSetAsync(
            cacheKey,
            async ct =>
            {
                var totalRoles = await roleRepository.CountAsync(
                    new RoleCountSpecification(orgId),
                    ct
                );

                var assignedMembers = await memberRepository.CountAsync(
                    new AssignedMemberCountSpecification(orgId),
                    ct
                );

                var previewRoles = await roleRepository.ListAsync(
                    new RoleNamePreviewSpecification(orgId),
                    ct
                );

                var roleNamesPreview = previewRoles.Select(r => r.Name).ToList();

                return new RolesSummaryDto(totalRoles, assignedMembers, roleNamesPreview);
            },
            options => options.SetDuration(TimeSpan.FromSeconds(60)),
            token: cancellationToken
        );
    }
}
