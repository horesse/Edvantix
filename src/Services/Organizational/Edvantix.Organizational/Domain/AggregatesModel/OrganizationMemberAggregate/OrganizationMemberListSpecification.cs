using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Спецификация для постраничного получения участников организации.
/// Поддерживает фильтрацию по статусу.
/// </summary>
public sealed class OrganizationMemberListSpecification : Specification<OrganizationMember>
{
    public OrganizationMemberListSpecification(
        Guid organizationId,
        int offset,
        int limit,
        OrganizationStatus? status = null
    )
    {
        Query
            .AsNoTracking()
            .Where(m => m.OrganizationId == organizationId && !m.IsDeleted)
            .OrderByDescending(m => m.StartDate)
            .Skip(offset)
            .Take(limit);

        ApplyFilters(Query, status);
    }

    internal static void ApplyFilters(
        ISpecificationBuilder<OrganizationMember> query,
        OrganizationStatus? status
    )
    {
        if (status.HasValue)
        {
            query.Where(m => m.Status == status.Value);
        }
    }
}
