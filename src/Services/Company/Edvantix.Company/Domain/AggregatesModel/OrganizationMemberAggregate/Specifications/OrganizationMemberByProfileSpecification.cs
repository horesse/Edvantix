using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate.Specifications;

/// <summary>
/// Спецификация для поиска участника по идентификатору профиля (опционально в конкретной организации).
/// </summary>
public sealed class OrganizationMemberByProfileSpecification : CommonSpecification<OrganizationMember>
{
    public OrganizationMemberByProfileSpecification(long profileId, long? organizationId = null)
    {
        Query.Where(x => x.ProfileId == profileId);

        if (organizationId.HasValue)
            Query.Where(x => x.OrganizationId == organizationId.Value);
    }
}
