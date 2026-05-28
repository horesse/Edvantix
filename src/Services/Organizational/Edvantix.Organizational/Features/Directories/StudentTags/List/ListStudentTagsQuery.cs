using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentTags.List;

/// <summary>Запрос постраничного списка тегов студентов организации.</summary>
/// <param name="Search">Строка поиска по названию (опционально).</param>
/// <param name="IncludeArchived">Включать ли архивные записи.</param>
/// <param name="Page">Номер страницы (от 1).</param>
/// <param name="PageSize">Размер страницы.</param>
[RequirePermission(OrganizationPermissions.View)]
public sealed record ListStudentTagsQuery(
    [property: Description("Строка поиска по названию")] string? Search = null,
    [property: Description("Включать архивные записи")] bool IncludeArchived = false,
    [property: Description("Номер страницы (от 1)")] int Page = 1,
    [property: Description("Размер страницы")] int PageSize = 50
) : IQuery<PagedResult<StudentTagListItemDto>>;

internal sealed class ListStudentTagsQueryHandler(
    ITenantContext tenantContext,
    IStudentTagRepository repository,
    IMapper<StudentTag, StudentTagListItemDto> mapper
) : IQueryHandler<ListStudentTagsQuery, PagedResult<StudentTagListItemDto>>
{
    public async ValueTask<PagedResult<StudentTagListItemDto>> Handle(
        ListStudentTagsQuery query,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;

        // isArchived=null означает «все» (includeArchived=true), false — только активные
        var isArchivedFilter = query.IncludeArchived ? (bool?)null : false;

        var listSpec = new StudentTagListSpecification(
            orgId,
            query.IncludeArchived,
            query.Search,
            query.Page,
            query.PageSize
        );

        var countSpec = new StudentTagCountSpecification(orgId, isArchivedFilter, query.Search);

        var items = await repository.ListAsync(listSpec, cancellationToken);
        var total = await repository.CountAsync(countSpec, cancellationToken);

        var dtos = mapper.Map(items);

        return new PagedResult<StudentTagListItemDto>(
            dtos.ToList(),
            query.Page,
            query.PageSize,
            total
        );
    }
}
