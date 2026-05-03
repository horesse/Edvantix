using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Roles.List;

[RequirePermission(OrganizationPermissions.Roles)]
public sealed record GetRolesQuery(
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Поиск по названию или описанию")] string? Search = null
) : IQuery<PagedResult<RoleDto>>;

internal sealed class GetRolesQueryHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRoleRepository repository,
    IPermissionRepository permissionRepository,
    IOrganizationMemberRepository memberRepository,
    IMapper<OrganizationMemberRole, RoleDto> mapper
) : IQueryHandler<GetRolesQuery, PagedResult<RoleDto>>
{
    public async ValueTask<PagedResult<RoleDto>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken
    )
    {
        var clamped = (
            PageIndex: Math.Max(request.PageIndex, 1),
            PageSize: Math.Clamp(request.PageSize, 1, 100)
        );

        var offset = (clamped.PageIndex - 1) * clamped.PageSize;
        var organizationId = tenantContext.OrganizationId;
        var search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim();

        var listSpec = new RoleListSpecification(organizationId, offset, clamped.PageSize, search);
        var countSpec = new RoleCountSpecification(organizationId, search);

        var rolesTask = repository.ListAsync(listSpec, cancellationToken);
        var totalCountTask = repository.CountAsync(countSpec, cancellationToken);
        var totalPermissionsCountTask = permissionRepository.CountAsync(cancellationToken);

        var roles = await rolesTask;
        var totalCount = await totalCountTask;
        var totalPermissionsCount = await totalPermissionsCountTask;

        var roleIds = roles.Select(r => r.Id).ToList();
        var memberCounts =
            roleIds.Count > 0
                ? await memberRepository.GetMemberCountsByRolesAsync(roleIds, cancellationToken)
                : (IReadOnlyDictionary<Guid, int>)new Dictionary<Guid, int>();

        var items = roles
            .Select(r =>
                mapper.Map(r) with
                {
                    TotalPermissionsCount = totalPermissionsCount,
                    MembersCount = memberCounts.GetValueOrDefault(r.Id),
                }
            )
            .ToList();

        return new PagedResult<RoleDto>(items, clamped.PageIndex, clamped.PageSize, totalCount);
    }
}
