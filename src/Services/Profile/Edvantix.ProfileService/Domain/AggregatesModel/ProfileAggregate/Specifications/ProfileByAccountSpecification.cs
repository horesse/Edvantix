using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;

public sealed class ProfileByAccountSpecification : AttributeSpecification<Profile>
{
    public ProfileByAccountSpecification(Guid accountId)
    {
        Query.Where(x => x.AccountId == accountId);
    }
}
