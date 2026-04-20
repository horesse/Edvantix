namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Спецификация для постраничного получения ролей организации.
/// </summary>
public sealed class RoleListSpecification : Specification<OrganizationMemberRole>
{
    public RoleListSpecification(Guid organizationId, int offset, int limit)
    {
        Query
            .AsNoTracking()
            .Where(r => r.OrganizationId == organizationId && !r.IsDeleted)
            .OrderBy(r => r.Code)
            .Skip(offset)
            .Take(limit);
    }
}

/// <summary>
/// Спецификация для подсчёта ролей организации (без пагинации).
/// </summary>
public sealed class RoleCountSpecification : Specification<OrganizationMemberRole>
{
    public RoleCountSpecification(Guid organizationId)
    {
        Query.AsNoTracking().Where(r => r.OrganizationId == organizationId && !r.IsDeleted);
    }
}
