using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Features.Directories.Levels;
using Edvantix.Organizational.Grpc.Services.Groups;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Levels.List;

/// <summary>Постраничный список уровней для страницы справочника.</summary>
/// <param name="Search">Фильтр по названию (частичное совпадение).</param>
/// <param name="IncludeArchived">Включать деактивированные уровни.</param>
/// <param name="PageIndex">Номер страницы (начиная с 1).</param>
/// <param name="PageSize">Размер страницы.</param>
[RequirePermission(LevelPermissions.View)]
public sealed record ListLevelsDirectoryQuery(
    [property: Description("Фильтр по названию")] string? Search = null,
    [property: Description("Включить деактивированные уровни")] bool IncludeArchived = false,
    [property: Description("Номер страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Размер страницы")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize
) : IQuery<PagedResult<LevelDirectoryListItemDto>>;

internal sealed class ListLevelsDirectoryQueryHandler(
    ITenantContext tenantContext,
    ILevelRepository repository,
    IGroupsUsageService groupsUsageService
) : IQueryHandler<ListLevelsDirectoryQuery, PagedResult<LevelDirectoryListItemDto>>
{
    public async ValueTask<PagedResult<LevelDirectoryListItemDto>> Handle(
        ListLevelsDirectoryQuery query,
        CancellationToken cancellationToken
    )
    {
        var pageIndex = Math.Max(query.PageIndex, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var (items, total) = await repository.ListForDirectoryAsync(
            tenantContext.OrganizationId,
            includeInactive: query.IncludeArchived,
            query.Search,
            pageIndex,
            pageSize,
            cancellationToken
        );

        var counts = await groupsUsageService.CountByLevelIdsAsync(
            items.Select(l => l.Id),
            cancellationToken
        );

        return new PagedResult<LevelDirectoryListItemDto>(
            [
                .. items.Select(l =>
                    LevelDirectoryMapper.ToListItemDto(l, BuildUsage(counts, l.Id))
                ),
            ],
            pageIndex,
            pageSize,
            total
        );
    }

    private static IReadOnlyList<DirectoryUsageDto> BuildUsage(
        IReadOnlyDictionary<Guid, int> counts,
        Guid id
    ) => [new DirectoryUsageDto("Группы", counts.GetValueOrDefault(id, 0))];
}
