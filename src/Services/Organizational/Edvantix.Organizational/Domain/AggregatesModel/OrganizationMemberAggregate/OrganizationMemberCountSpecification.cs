using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Спецификация для подсчёта участников организации (без пагинации).
/// </summary>
public sealed class OrganizationMemberCountSpecification : Specification<OrganizationMember>
{
    public OrganizationMemberCountSpecification(
        Guid organizationId,
        OrganizationStatus? status = null
    )
    {
        Query.AsNoTracking().Where(m => m.OrganizationId == organizationId && !m.IsDeleted);
        OrganizationMemberListSpecification.ApplyFilters(Query, status);
    }
}
