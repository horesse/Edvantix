namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

public sealed class ProfileAccountSpecification : Specification<Profile>
{
    public ProfileAccountSpecification(Guid accountId)
    {
        Query.Where(p => p.AccountId == accountId);
    }
}
