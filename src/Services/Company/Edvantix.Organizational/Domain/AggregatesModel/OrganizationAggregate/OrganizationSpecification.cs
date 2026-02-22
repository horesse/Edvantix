namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

public sealed class OrganizationSpecification : Specification<Organization>
{
    /// <summary>Найти организацию по идентификатору с опциональной загрузкой навигационных свойств.</summary>
    public OrganizationSpecification(
        Guid id,
        bool includeMembers = false,
        bool includeGroups = false,
        bool includeContacts = false
    )
    {
        Query.Where(x => x.Id == id);

        if (includeMembers)
            Query.Include(x => x.Members);

        if (includeGroups)
            Query.Include(x => x.Groups);

        if (includeContacts)
            Query.Include(x => x.Contacts);
    }
}
