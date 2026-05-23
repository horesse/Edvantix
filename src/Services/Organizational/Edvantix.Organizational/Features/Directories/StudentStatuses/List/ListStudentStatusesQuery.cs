using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Domain.Permissions;
using Edvantix.Organizational.Features.Directories.StudentStatuses.Specifications;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses.List;

/// <summary>Запрос постраничного списка статусов студентов организации.</summary>
/// <param name="Search">Строка поиска по имени или коду (опционально).</param>
/// <param name="IncludeArchived">Включать ли архивные записи.</param>
/// <param name="Page">Номер страницы (от 1).</param>
/// <param name="PageSize">Размер страницы.</param>
[RequirePermission(OrganizationPermissions.View)]
public sealed record ListStudentStatusesQuery(
    [property: Description("Строка поиска по имени или коду")] string? Search = null,
    [property: Description("Включать архивные записи")] bool IncludeArchived = false,
    [property: Description("Номер страницы (от 1)")] int Page = 1,
    [property: Description("Размер страницы")] int PageSize = 50
) : IQuery<PagedResult<StudentStatusListItemDto>>;

internal sealed class ListStudentStatusesQueryHandler(
    ITenantContext tenantContext,
    IStudentStatusRepository repository,
    IMapper<StudentStatus, StudentStatusListItemDto> mapper
) : IQueryHandler<ListStudentStatusesQuery, PagedResult<StudentStatusListItemDto>>
{
    public async ValueTask<PagedResult<StudentStatusListItemDto>> Handle(
        ListStudentStatusesQuery query,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;

        // isArchived=null означает «все» (includeArchived=true), false — только активные
        var isArchivedFilter = query.IncludeArchived ? (bool?)null : false;

        var listSpec = new StudentStatusListSpecification(
            orgId,
            query.IncludeArchived,
            query.Search,
            query.Page,
            query.PageSize
        );

        var countSpec = new StudentStatusCountSpecification(orgId, isArchivedFilter, query.Search);

        var items = await repository.ListAsync(listSpec, cancellationToken);
        var total = await repository.CountAsync(countSpec, cancellationToken);

        var dtos = mapper.Map(items);

        return new PagedResult<StudentStatusListItemDto>(
            dtos.ToList(),
            query.Page,
            query.PageSize,
            total
        );
    }
}
