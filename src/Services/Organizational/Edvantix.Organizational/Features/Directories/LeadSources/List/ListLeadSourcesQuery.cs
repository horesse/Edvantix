using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate.Specifications;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.LeadSources.List;

/// <summary>Запрос постраничного списка источников привлечения организации.</summary>
/// <param name="Search">Строка поиска по названию (опционально).</param>
/// <param name="IncludeArchived">Включать ли архивные записи.</param>
/// <param name="Page">Номер страницы (от 1).</param>
/// <param name="PageSize">Размер страницы.</param>
[RequirePermission(OrganizationPermissions.View)]
public sealed record ListLeadSourcesQuery(
    [property: Description("Строка поиска по названию")] string? Search = null,
    [property: Description("Включать архивные записи")] bool IncludeArchived = false,
    [property: Description("Номер страницы (от 1)")] int Page = 1,
    [property: Description("Размер страницы")] int PageSize = 50
) : IQuery<PagedResult<LeadSourceListItemDto>>;

internal sealed class ListLeadSourcesQueryHandler(
    ITenantContext tenantContext,
    ILeadSourceRepository repository,
    IMapper<LeadSource, LeadSourceListItemDto> mapper
) : IQueryHandler<ListLeadSourcesQuery, PagedResult<LeadSourceListItemDto>>
{
    public async ValueTask<PagedResult<LeadSourceListItemDto>> Handle(
        ListLeadSourcesQuery query,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;

        // isArchived=null означает «все» (includeArchived=true), false — только активные
        var isArchivedFilter = query.IncludeArchived ? (bool?)null : false;

        var listSpec = new LeadSourceListSpecification(
            orgId,
            query.IncludeArchived,
            query.Search,
            query.Page,
            query.PageSize
        );

        var countSpec = new LeadSourceCountSpecification(orgId, isArchivedFilter, query.Search);

        var items = await repository.ListAsync(listSpec, cancellationToken);
        var total = await repository.CountAsync(countSpec, cancellationToken);

        var dtos = mapper.Map(items);

        return new PagedResult<LeadSourceListItemDto>(
            dtos.ToList(),
            query.Page,
            query.PageSize,
            total
        );
    }
}
