namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

/// <summary>
/// Спецификация профиля. Используй фабричные методы вместо прямого конструктора:
/// <list type="bullet">
///   <item><see cref="ForRead"/> — read-only, все навигации включая <c>Skills → Skill</c> (для запросов → <c>ProfileDetailsModel</c>).</item>
///   <item><see cref="ForWrite"/> — с трекингом, коллекции без <c>ThenInclude</c> (команды работают только с ID).</item>
///   <item><see cref="Minimal"/> — read-only, только <c>FullName</c> (для запросов → <c>ProfileViewModel</c>).</item>
/// </list>
/// </summary>
public sealed class ProfileSpecification : Specification<Profile>
{
    private ProfileSpecification(Guid profileId)
    {
        Query.OrderBy(x => x.FullName.LastName).Where(p => p.Id == profileId);
    }

    /// <summary>Read-only, все навигации — для запросов, возвращающих <c>ProfileDetailsModel</c>.</summary>
    public static ProfileSpecification ForRead(Guid profileId)
    {
        var spec = new ProfileSpecification(profileId);

        IncludeCollections(spec.Query).Include(p => p.Skills).ThenInclude(s => s.Skill);

        return spec;
    }

    /// <summary>
    /// С трекингом и коллекциями — для команд, изменяющих вложенные сущности.
    /// <c>Skills → Skill</c> не загружается: команды работают только с <c>SkillId</c>.
    /// </summary>
    public static ProfileSpecification ForWrite(Guid profileId)
    {
        var spec = new ProfileSpecification(profileId);

        IncludeCollections(spec.Query).AsTracking().Include(p => p.Skills);

        return spec;
    }

    /// <summary>Read-only, только <c>FullName</c> — для запросов, возвращающих <c>ProfileViewModel</c>.</summary>
    public static ProfileSpecification Minimal(Guid profileId) => new(profileId);

    private static ISpecificationBuilder<Profile> IncludeCollections(
        ISpecificationBuilder<Profile> query
    ) =>
        query
            .Include(p => p.Contacts)
            .Include(p => p.Educations)
            .Include(p => p.EmploymentHistories);
}
