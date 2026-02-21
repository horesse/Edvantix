namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

public sealed class OrganizationSpecification : Specification<Organization>
{
    private OrganizationSpecification(string name, string nameLatin)
    {
        Query.Where(x => x.Name.Contains(name));
        Query.Where(x => x.NameLatin.Contains(nameLatin));
    }
}
