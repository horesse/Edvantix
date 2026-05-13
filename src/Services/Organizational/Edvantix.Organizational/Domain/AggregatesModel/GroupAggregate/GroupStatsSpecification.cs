namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Спецификация для подсчёта групп по статусу.
/// Используется для формирования KPI-карточек на странице групп.
/// </summary>
public sealed class GroupStatsSpecification : Specification<Group>
{
    /// <summary>Подсчитывает все активные (не удалённые) группы организации.</summary>
    public GroupStatsSpecification(Guid organizationId)
    {
        Query.Where(g => g.OrganizationId == organizationId && !g.IsDeleted);
    }

    /// <summary>Подсчитывает группы организации с заданным статусом.</summary>
    public GroupStatsSpecification(Guid organizationId, GroupStatus status)
    {
        Query
            .Where(g => g.OrganizationId == organizationId && !g.IsDeleted)
            .Where(g => g.Status == status);
    }
}
