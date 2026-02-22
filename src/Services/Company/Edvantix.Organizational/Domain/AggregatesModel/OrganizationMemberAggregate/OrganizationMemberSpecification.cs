namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Спецификация для поиска участников организации по идентификатору организации.
/// </summary>
public sealed class OrganizationMemberSpecification : Specification<OrganizationMember>
{
    public OrganizationMemberSpecification(Guid organizationId)
    {
        Query.Where(x => x.OrganizationId == organizationId);
    }

    public OrganizationMemberSpecification(Guid profileId, Guid? organizationId = null)
    {
        Query.Where(x => x.ProfileId == profileId);

        if (organizationId.HasValue)
            Query.Where(x => x.OrganizationId == organizationId.Value);
    }
}
