using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Features.Organizations.List;

public sealed record GetOrganizationsQuery(
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Поиск по полному или краткому наименованию")] string? Search = null,
    [property: Description("Фильтр по статусу организации")] OrganizationStatus? Status = null,
    [property: Description("Фильтр по типу организации")] OrganizationType? OrganizationType = null
) : IQuery<PagedResult<OrganizationDto>>;

internal sealed class GetOrganizationsQueryHandler(
    IOrganizationRepository repository,
    IMapper<Organization, OrganizationDto> mapper
) : IQueryHandler<GetOrganizationsQuery, PagedResult<OrganizationDto>>
{
    public async ValueTask<PagedResult<OrganizationDto>> Handle(
        GetOrganizationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var clamped = (
            PageIndex: Math.Max(request.PageIndex, 1),
            PageSize: Math.Clamp(request.PageSize, 1, 100)
        );

        var offset = (clamped.PageIndex - 1) * clamped.PageSize;

        var listSpec = new OrganizationListSpecification(
            offset,
            clamped.PageSize,
            request.Search,
            request.Status,
            request.OrganizationType
        );

        var countSpec = new OrganizationCountSpecification(
            request.Search,
            request.Status,
            request.OrganizationType
        );

        var organizations = await repository.ListAsync(listSpec, cancellationToken);
        var totalCount = await repository.CountAsync(countSpec, cancellationToken);

        var items = organizations.Select(mapper.Map).ToList();

        return new PagedResult<OrganizationDto>(
            items,
            clamped.PageIndex,
            clamped.PageSize,
            totalCount
        );
    }
}
