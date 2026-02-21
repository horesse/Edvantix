using Edvantix.Chassis.Specification.Builders;

namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

public sealed class ProfileSpecification : Specification<Profile>
{
    public ProfileSpecification(Guid accountId, bool includeData = false)
    {
        Query.Include(x => x.FullName);
        Query.Where(x => x.AccountId == accountId);

        if (!includeData)
            return;

        Query.Include(x => x.Contacts);
        Query.Include(x => x.Educations);
        Query.Include(x => x.EmploymentHistories);
    }
}
