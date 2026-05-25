namespace Edvantix.Organizational.Domain.LessonTypeAggregate.Specifications;

/// <summary>
/// Спецификация для постраничного списка типов занятий с фильтрацией и поиском.
/// </summary>
public sealed class LessonTypeListSpec : Specification<LessonType>
{
    /// <summary>Конструктор для постраничной выборки.</summary>
    public LessonTypeListSpec(
        Guid organizationId,
        bool includeArchived,
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

        ApplyFilters(Query, includeArchived, search);
    }

    /// <summary>Конструктор для подсчёта (без пагинации).</summary>
    public LessonTypeListSpec(Guid organizationId, bool includeArchived, string? search)
    {
        Query.Where(lt => lt.OrganizationId == organizationId);

        ApplyFilters(Query, includeArchived, search);
    }

    private static void ApplyFilters(
        ISpecificationBuilder<LessonType> query,
        bool includeArchived,
        string? search
    )
    {
        if (!includeArchived)
            query.Where(lt => !lt.IsArchived);

        if (!string.IsNullOrWhiteSpace(search))
            query.Where(lt => lt.Name.Contains(search));
    }
}
