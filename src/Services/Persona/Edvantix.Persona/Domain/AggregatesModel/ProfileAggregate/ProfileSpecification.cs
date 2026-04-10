namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

public sealed class ProfileSpecification : Specification<Profile>
{
    public ProfileSpecification(IEnumerable<Guid> ids)
    {
        Query.Where(x => ids.Contains(x.Id));
    }

    private ProfileSpecification(Guid profileId)
    {
        Query.OrderBy(x => x.FullName.LastName).Where(p => p.Id == profileId);
    }

    public static ProfileSpecification ForRead(Guid profileId)
    {
        var spec = new ProfileSpecification(profileId);

        IncludeCollections(spec.Query).Include(p => p.Skills).ThenInclude(s => s.Skill);

        return spec;
    }

    public static ProfileSpecification ForWrite(Guid profileId)
    {
        var spec = new ProfileSpecification(profileId);

        IncludeCollections(spec.Query).AsTracking().Include(p => p.Skills);

        return spec;
    }

    public static ProfileSpecification Minimal(Guid profileId) => new(profileId);

    /// <summary>Минимальный профиль с отслеживанием изменений — для операций записи без загрузки коллекций.</summary>
    public static ProfileSpecification MinimalForWrite(Guid profileId)
    {
        var spec = new ProfileSpecification(profileId);
        spec.Query.AsTracking();
        return spec;
    }

    private static ISpecificationBuilder<Profile> IncludeCollections(
        ISpecificationBuilder<Profile> query
    ) =>
        query
            .Include(p => p.Contacts)
            .Include(p => p.Educations)
            .Include(p => p.EmploymentHistories);
}
