using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.LessonTypeAggregate;
using Edvantix.Groups.Domain.LessonTypeAggregate.Specifications;
using Edvantix.Groups.Features.Directories.LessonTypes;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Directories.LessonTypes.List;

/// <summary>
/// Возвращает постраничный список типов занятий организации.
/// </summary>
/// <param name="PageIndex">Номер страницы (начинается с 1).</param>
/// <param name="PageSize">Размер страницы.</param>
/// <param name="Search">Текстовый поиск по имени.</param>
/// <param name="IncludeArchived">Включить архивные записи.</param>
[RequirePermission(LessonTypePermissions.View)]
public sealed record ListLessonTypesQuery(
    [property: Description("Номер страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Текстовый поиск по имени")] string? Search = null,
    [property: Description("Включить архивные записи")] bool IncludeArchived = false
) : IQuery<PagedResult<LessonTypeListItemDto>>;

internal sealed class ListLessonTypesQueryHandler(
    ITenantContext tenantContext,
    ILessonTypeRepository repository
) : IQueryHandler<ListLessonTypesQuery, PagedResult<LessonTypeListItemDto>>
{
    public async ValueTask<PagedResult<LessonTypeListItemDto>> Handle(
        ListLessonTypesQuery request,
        CancellationToken cancellationToken
    )
    {
        var pageIndex = Math.Max(request.PageIndex, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var offset = (pageIndex - 1) * pageSize;
        var orgId = tenantContext.OrganizationId;

        var listSpec = new LessonTypeListSpec(
            orgId,
            request.IncludeArchived,
            request.Search,
            offset,
            pageSize
        );
        var countSpec = new LessonTypeListSpec(orgId, request.IncludeArchived, request.Search);

        var items = await repository.ListAsync(listSpec, cancellationToken);
        var totalCount = await repository.CountAsync(countSpec, cancellationToken);

        var dtos = items.Select(lt => lt.ToListItemDto()).ToList();

        return new PagedResult<LessonTypeListItemDto>(dtos, pageIndex, pageSize, totalCount);
    }
}
