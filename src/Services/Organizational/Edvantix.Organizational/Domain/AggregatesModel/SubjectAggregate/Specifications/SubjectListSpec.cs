namespace Edvantix.Organizational.Domain.AggregatesModel.SubjectAggregate.Specifications;

/// <summary>
/// Спецификация для постраничного списка предметов организации с поиском и фильтрацией.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — активные записи.
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные (удалённые).
/// </para>
/// </summary>
public sealed class SubjectListSpec : Specification<Subject>
{
    /// <summary>Конструктор для постраничного списка.</summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="offset">Смещение (skip).</param>
    /// <param name="size">Размер страницы (take).</param>
    /// <param name="search">Текстовый поиск по названию (опционально).</param>
    /// <param name="isArchive">Показать только архивные записи.</param>
    public SubjectListSpec(
        Guid organizationId,
        int offset,
        int size,
        string? search = null,
        bool isArchive = false
    )
    {
        Query
            .Where(s => s.OrganizationId == organizationId)
            .OrderBy(s => s.Order)
            .ThenBy(s => s.Name)
            .Skip(offset)
            .Take(size);

        ApplyFilters(Query, search, isArchive);
    }

    /// <summary>Конструктор для подсчёта (без пагинации).</summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="search">Текстовый поиск по названию (опционально).</param>
    /// <param name="isArchive">Показать только архивные записи.</param>
    public SubjectListSpec(Guid organizationId, string? search = null, bool isArchive = false)
    {
        Query.Where(s => s.OrganizationId == organizationId);
        ApplyFilters(Query, search, isArchive);
    }

    private static void ApplyFilters(
        ISpecificationBuilder<Subject> query,
        string? search,
        bool isArchive
    )
    {
        if (isArchive)
            query.IgnoreQueryFilters().Where(s => s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query.Where(s => s.Name.Contains(search));
    }
}
