using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;

public sealed class ProfileByAccountSpecification : AttributeSpecification<Profile>
{
    public ProfileByAccountSpecification(Guid accountId, bool includeData = false)
    {
        Query.Where(x => x.AccountId == accountId);

        if (!includeData)
            return;

        Query.Include(x => x.Contacts);
        Query.Include(x => x.Educations);
        Query.Include(x => x.EmploymentHistories);
    }
}
