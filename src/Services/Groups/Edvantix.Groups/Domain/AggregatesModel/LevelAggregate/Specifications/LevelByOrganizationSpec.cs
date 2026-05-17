namespace Edvantix.Groups.Domain.AggregatesModel.LevelAggregate.Specifications;

/// <summary>
/// Спецификация для выборки уровней организации, отсортированных по порядку отображения.
/// </summary>
public sealed class LevelByOrganizationSpec : Specification<Level>
{
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="includeInactive">Включать ли деактивированные уровни.</param>
    public LevelByOrganizationSpec(Guid organizationId, bool includeInactive = false)
    {
        Query.Where(l => l.OrganizationId == organizationId && !l.IsDeleted);

        if (!includeInactive)
            Query.Where(l => l.IsActive);

        Query.OrderBy(l => l.SortOrder);
    }
}
