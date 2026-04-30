using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Спецификация для подсчёта участников организации по статусу.
/// Используется для формирования KPI-карточек на странице участников.
/// </summary>
public sealed class OrganizationMembersKpiSpecification : Specification<OrganizationMember>
{
    /// <summary>Подсчитывает всех участников организации (без фильтра по статусу).</summary>
    public OrganizationMembersKpiSpecification(Guid organizationId)
    {
        Query.Where(x => x.OrganizationId == organizationId);
    }

    /// <summary>Подсчитывает участников организации с заданным статусом.</summary>
    public OrganizationMembersKpiSpecification(Guid organizationId, OrganizationStatus status)
    {
        Query.Where(x => x.OrganizationId == organizationId).Where(x => x.Status == status);
    }
}
