namespace Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate.Specifications;

/// <summary>
/// Спецификация постраничного списка уровней для страницы справочника.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — только активные уровни (<see cref="Level.IsActive"/> = true).
/// <paramref name="isArchive"/> = <see langword="true"/> — только деактивированные уровни (<see cref="Level.IsActive"/> = false).
/// Удалённые уровни (<see cref="Level.IsDeleted"/> = true) всегда исключены глобальным query-фильтром.
/// </para>
/// </summary>
public sealed class LevelListDirectorySpec : Specification<Level>
{
    public LevelListDirectorySpec(
        Guid organizationId,
        bool isArchive,
        string? search,
        int pageIndex,
        int pageSize
    )
    {
        var offset = (pageIndex - 1) * pageSize;

        Query
            .AsNoTracking()
            .Where(l => l.OrganizationId == organizationId)
            .Where(l => l.IsActive == !isArchive)
            .OrderBy(l => l.SortOrder)
            .Skip(offset)
            .Take(pageSize);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(l => l.Name.Contains(search.Trim()));
    }
}
