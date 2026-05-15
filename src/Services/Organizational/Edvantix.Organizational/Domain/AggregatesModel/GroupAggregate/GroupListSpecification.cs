namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Спецификация для получения постраничного списка групп с фильтрацией.
/// </summary>
public sealed class GroupListSpecification : Specification<Group>
{
    /// <summary>Конструктор для постраничного списка.</summary>
    public GroupListSpecification(
        Guid organizationId,
        int offset,
        int limit,
        string? search = null,
        IReadOnlyCollection<Guid>? levelIds = null,
        IReadOnlyCollection<GroupStatus>? statuses = null,
        IReadOnlyCollection<GroupFormat>? formats = null
    )
    {
        Query
            .Where(g => g.OrganizationId == organizationId && !g.IsDeleted)
            .OrderByDescending(g => g.Id)
            .Skip(offset)
            .Take(limit);

        ApplyFilters(Query, search, levelIds, statuses, formats);
    }

    /// <summary>Конструктор для подсчёта (без пагинации).</summary>
    public GroupListSpecification(
        Guid organizationId,
        string? search = null,
        IReadOnlyCollection<Guid>? levelIds = null,
        IReadOnlyCollection<GroupStatus>? statuses = null,
        IReadOnlyCollection<GroupFormat>? formats = null
    )
    {
        Query.Where(g => g.OrganizationId == organizationId && !g.IsDeleted);
        ApplyFilters(Query, search, levelIds, statuses, formats);
    }

    private static void ApplyFilters(
        ISpecificationBuilder<Group> query,
        string? search,
        IReadOnlyCollection<Guid>? levelIds,
        IReadOnlyCollection<GroupStatus>? statuses,
        IReadOnlyCollection<GroupFormat>? formats
    )
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            query.Where(g => g.Name.ToLower().Contains(term));
        }

        if (levelIds?.Count > 0)
            query.Where(g => levelIds.Contains(g.LevelId));

        if (statuses?.Count > 0)
            query.Where(g => statuses.Contains(g.Status));

        if (formats?.Count > 0)
            query.Where(g => formats.Contains(g.Format));
    }
}
