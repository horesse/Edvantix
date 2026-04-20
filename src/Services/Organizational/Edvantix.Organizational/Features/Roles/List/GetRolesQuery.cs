using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.Roles.List;

[RequirePermission(OrganizationPermissions.ManageRoles)]
public sealed record GetRolesQuery(
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize
) : IQuery<PagedResult<RoleDto>>;

internal sealed class GetRolesQueryHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRoleRepository repository,
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

        var listSpec = new RoleListSpecification(organizationId, offset, clamped.PageSize);
        var countSpec = new RoleCountSpecification(organizationId);

        var roles = await repository.ListAsync(listSpec, cancellationToken);
        var totalCount = await repository.CountAsync(countSpec, cancellationToken);

        var items = roles.Select(mapper.Map).ToList();

        return new PagedResult<RoleDto>(items, clamped.PageIndex, clamped.PageSize, totalCount);
    }
}
