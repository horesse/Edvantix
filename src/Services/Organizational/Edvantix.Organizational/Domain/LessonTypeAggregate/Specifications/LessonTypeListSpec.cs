namespace Edvantix.Organizational.Domain.LessonTypeAggregate.Specifications;

/// <summary>
/// Спецификация для постраничного списка типов занятий с фильтрацией и поиском.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — активные записи.
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные (удалённые).
/// </para>
/// </summary>
public sealed class LessonTypeListSpec : Specification<LessonType>
{
    /// <summary>Конструктор для постраничной выборки.</summary>
    public LessonTypeListSpec(
        Guid organizationId,
        bool isArchive,
        string? search,
        int offset,
        int limit
    )
    {
        Query
            .Where(lt => lt.OrganizationId == organizationId)
            .OrderBy(lt => lt.Order)
            .ThenBy(lt => lt.Name)
            .Skip(offset)
            .Take(limit);

        ApplyFilters(Query, isArchive, search);
    }

    /// <summary>Конструктор для подсчёта (без пагинации).</summary>
    public LessonTypeListSpec(Guid organizationId, bool isArchive, string? search)
    {
        Query.Where(lt => lt.OrganizationId == organizationId);

        ApplyFilters(Query, isArchive, search);
    }

    private static void ApplyFilters(
        ISpecificationBuilder<LessonType> query,
        bool isArchive,
        string? search
    )
    {
        if (isArchive)
            query.IgnoreQueryFilters().Where(lt => lt.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query.Where(lt => lt.Name.Contains(search));
    }
}
