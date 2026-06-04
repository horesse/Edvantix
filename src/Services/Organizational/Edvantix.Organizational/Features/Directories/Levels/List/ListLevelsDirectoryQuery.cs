using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate.Specifications;
using Edvantix.Organizational.Grpc.Services.Groups;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Levels.List;

/// <summary>Постраничный список уровней для страницы справочника.</summary>
/// <param name="Search">Фильтр по названию (частичное совпадение).</param>
/// <param name="IsArchive">Показать только деактивированные уровни.</param>
/// <param name="PageIndex">Номер страницы (начиная с 1).</param>
/// <param name="PageSize">Размер страницы.</param>
[RequirePermission(LevelPermissions.View)]
public sealed record ListLevelsDirectoryQuery(
    [property: Description("Фильтр по названию")] string? Search = null,
    [property: Description("Показать только деактивированные уровни")] bool IsArchive = false,
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
        var orgId = tenantContext.OrganizationId;

        var listSpec = new LevelListDirectorySpec(
            orgId,
            query.IsArchive,
            query.Search,
            pageIndex,
            pageSize
        );
        var countSpec = new LevelCountDirectorySpec(orgId, query.IsArchive, query.Search);

        var items = await repository.ListAsync(listSpec, cancellationToken);
        var total = await repository.CountAsync(countSpec, cancellationToken);

        var counts = await groupsUsageService.CountByLevelIdsAsync(
            items.Select(l => l.Id),
            cancellationToken
        );

        return new PagedResult<LevelDirectoryListItemDto>(
            [.. items.Select(l => LevelDirectoryMapper.ToListItemDto(l, BuildUsage(counts, l.Id)))],
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
