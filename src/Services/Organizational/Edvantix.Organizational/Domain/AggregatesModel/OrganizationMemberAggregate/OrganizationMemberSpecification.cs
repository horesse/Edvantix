using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Спецификация для получения участников организации.
/// </summary>
public sealed class OrganizationMemberSpecification : Specification<OrganizationMember>
{
    public OrganizationMemberSpecification(
        Guid organizationId,
        Guid? roleId = null,
        OrganizationStatus? status = null
    )
    {
        Query.Where(x => x.OrganizationId == organizationId);

        ApplyFilters(Query, roleId, status);
    }

    public OrganizationMemberSpecification(
        Guid organizationId,
        int offset,
        int limit,
        Guid? roleId = null,
        OrganizationStatus? status = null
    )
    {
        Query
            .Where(m => m.OrganizationId == organizationId)
            .OrderByDescending(m => m.StartDate)
            .Skip(offset)
            .Take(limit);

        ApplyFilters(Query, roleId, status);
    }

    internal static void ApplyFilters(
        ISpecificationBuilder<OrganizationMember> query,
        Guid? roleId,
        OrganizationStatus? status
    )
    {
        if (status.HasValue)
        {
            query.Where(m => m.Status == status.Value);
        }

        if (roleId.HasValue)
        {
            query.Where(m => m.OrganizationMemberRoleId == roleId.Value);
        }
    }
}
