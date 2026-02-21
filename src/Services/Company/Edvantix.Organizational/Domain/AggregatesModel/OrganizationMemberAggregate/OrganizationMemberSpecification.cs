namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Спецификация для поиска участников организации по идентификатору организации.
/// </summary>
public sealed class OrganizationMemberSpecification : Specification<OrganizationMember>
{
    public OrganizationMemberSpecification(ulong organizationId)
    {
        Query.Where(x => x.OrganizationId == organizationId);
    }

    public OrganizationMemberSpecification(ulong profileId, ulong? organizationId = null)
    {
        Query.Where(x => x.ProfileId == profileId);

        if (organizationId.HasValue)
            Query.Where(x => x.OrganizationId == organizationId.Value);
    }
}
