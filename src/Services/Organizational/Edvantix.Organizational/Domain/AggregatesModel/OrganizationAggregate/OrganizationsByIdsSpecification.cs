namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

/// <summary>
/// Спецификация для получения организаций по набору идентификаторов.
/// </summary>
public sealed class OrganizationsByIdsSpecification : Specification<Organization>
{
    public OrganizationsByIdsSpecification(IReadOnlyCollection<Guid> ids)
    {
        Query.Where(o => ids.Contains(o.Id) && !o.IsDeleted);
        Query.OrderBy(o => o.FullLegalName);
    }
}
