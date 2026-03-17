namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

public sealed class ProfileSpecification : Specification<Profile>
{
    public ProfileSpecification(Guid? profileId, bool withDetails = false, bool asTracking = true)
    {
        Query.Where(p => p.Id == profileId);
        ApplyIncludes(withDetails);

        if (asTracking)
            Query.AsTracking();
    }

    private void ApplyIncludes(bool withDetails)
    {
        Query.Include(p => p.FullName);

        if (!withDetails)
            return;

        Query.Include(p => p.Contacts);
        Query.Include(p => p.Educations);
        Query.Include(p => p.EmploymentHistories);
        Query.Include(p => p.Skills).ThenInclude(s => s.Skill);
    }
}
