using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Enums;
using Edvantix.Organizational.Grpc.Services.Profiles;

namespace Edvantix.Organizational.Features.OrganizationMembers.List;

[RequirePermission(OrganizationPermissions.Read)]
public sealed record GetOrganizationMembersQuery(
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Фильтр по роли участника")] Guid? RoleId = null,
    [property: Description("Фильтр по статусу участника")] OrganizationStatus? Status = null
) : IQuery<PagedResult<OrganizationMemberDto>>;

internal sealed class GetOrganizationMembersQueryHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRepository repository,
    IMapper<OrganizationMember, OrganizationMemberDto> mapper,
    IProfileService profileService
) : IQueryHandler<GetOrganizationMembersQuery, PagedResult<OrganizationMemberDto>>
{
    public async ValueTask<PagedResult<OrganizationMemberDto>> Handle(
        GetOrganizationMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var pageIndex = Math.Max(request.PageIndex, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var offset = (pageIndex - 1) * pageSize;
        var organizationId = tenantContext.OrganizationId;

        var listSpec = new OrganizationMemberSpecification(
            organizationId,
            offset,
            pageSize,
            request.RoleId,
            request.Status
        );

        var countSpec = new OrganizationMemberSpecification(
            organizationId,
            request.RoleId,
            request.Status
        );

        var members = await repository.ListAsync(listSpec, cancellationToken);
        var totalCount = await repository.CountAsync(countSpec, cancellationToken);

        var items = members.Select(mapper.Map).ToList();
        await EnrichWithProfileDataAsync(items, cancellationToken);

        return new PagedResult<OrganizationMemberDto>(items, pageIndex, pageSize, totalCount);
    }

    private async Task EnrichWithProfileDataAsync(
        List<OrganizationMemberDto> items,
        CancellationToken cancellationToken
    )
    {
        if (items.Count == 0)
            return;

        var profileIds = items.Select(x => x.ProfileId.ToString()).ToArray();
        var response = await profileService.GetProfilesByIdsAsync(profileIds, cancellationToken);
        Guard.Against.Null(response, nameof(response));

        var profiles = response.Profiles.ToDictionary(p => p.Id);

        for (var i = 0; i < items.Count; i++)
        {
            var profileId = items[i].ProfileId.ToString();
            var profile = profiles.GetValueOrDefault(profileId);
            Guard.Against.NotFound(profile, items[i].ProfileId);

            items[i] = items[i] with { FullName = profile!.FullName };
        }
    }
}
