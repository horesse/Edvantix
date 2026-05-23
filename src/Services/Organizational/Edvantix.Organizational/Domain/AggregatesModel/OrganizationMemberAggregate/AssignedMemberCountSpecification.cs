namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Спецификация для подсчёта участников организации с назначенной ролью.
/// Учитываются только не удалённые участники (через глобальный фильтр запросов),
/// у которых явно задан идентификатор роли.
/// </summary>
public sealed class AssignedMemberCountSpecification : Specification<OrganizationMember>
{
    /// <param name="organizationId">Идентификатор организации.</param>
    public AssignedMemberCountSpecification(Guid organizationId)
    {
        Query
            .AsNoTracking()
            .Where(m => m.OrganizationId == organizationId && m.OrganizationRoleId != Guid.Empty);
    }
}
