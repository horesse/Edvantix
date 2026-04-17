using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Features.OrganizationMembers.List;

public sealed record GetOrganizationMembersQuery(
    [property: Description("Идентификатор организации")] Guid OrganizationId,
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Фильтр по статусу участника")] OrganizationStatus? Status = null
) : IQuery<PagedResult<OrganizationMemberDto>>;

internal sealed class GetOrganizationMembersQueryHandler(
    IOrganizationMemberRepository repository,
    IMapper<OrganizationMember, OrganizationMemberDto> mapper
) : IQueryHandler<GetOrganizationMembersQuery, PagedResult<OrganizationMemberDto>>
{
    public async ValueTask<PagedResult<OrganizationMemberDto>> Handle(
        GetOrganizationMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var clamped = (
            PageIndex: Math.Max(request.PageIndex, 1),
            PageSize: Math.Clamp(request.PageSize, 1, 100)
        );

        var offset = (clamped.PageIndex - 1) * clamped.PageSize;

        var listSpec = new OrganizationMemberListSpecification(
            request.OrganizationId,
            offset,
            clamped.PageSize,
            request.Status
        );

        var countSpec = new OrganizationMemberCountSpecification(
            request.OrganizationId,
            request.Status
        );

        var members = await repository.ListAsync(listSpec, cancellationToken);
        var totalCount = await repository.CountAsync(countSpec, cancellationToken);

        var items = members.Select(mapper.Map).ToList();

        return new PagedResult<OrganizationMemberDto>(
            items,
            clamped.PageIndex,
            clamped.PageSize,
            totalCount
        );
    }
}
