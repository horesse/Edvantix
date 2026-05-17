namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Базовый фильтр групп организации для спецификаций.
/// Используется как основа для подсчётов без материализации сущностей.
/// Агрегаты по участникам (MemberCount, Capacity) вычисляются через
/// <see cref="IGroupRepository.GetStatsProjectionAsync"/> единым SQL-запросом.
/// </summary>
public sealed class GroupStatsSpecification : Specification<Group>
{
    /// <summary>Все активные (не удалённые) группы организации.</summary>
    public GroupStatsSpecification(Guid organizationId)
    {
        Query.Where(g => g.OrganizationId == organizationId && !g.IsDeleted);
    }
}
