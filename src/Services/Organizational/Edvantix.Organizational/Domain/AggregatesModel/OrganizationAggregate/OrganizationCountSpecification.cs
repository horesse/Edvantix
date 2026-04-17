using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

/// <summary>
/// Спецификация для подсчёта организаций (без пагинации).
/// </summary>
public sealed class OrganizationCountSpecification : Specification<Organization>
{
    public OrganizationCountSpecification(
        string? search = null,
        OrganizationStatus? status = null,
        OrganizationType? organizationType = null
    )
    {
        Query.AsNoTracking().Where(o => !o.IsDeleted);
        OrganizationListSpecification.ApplyFilters(Query, search, status, organizationType);
    }
}
