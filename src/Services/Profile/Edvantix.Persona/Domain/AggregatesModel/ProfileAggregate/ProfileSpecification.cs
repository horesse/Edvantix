namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

/// <summary>Спецификация поиска профиля по внутреннему ID.</summary>
public sealed class ProfileByIdSpec : Specification<Profile>
{
    /// <param name="id">Внутренний ID профиля.</param>
    /// <param name="withDetails">
    /// Загружать контакты, историю занятости и образование.
    /// По умолчанию загружается только <see cref="Profile.FullName"/>.
    /// </param>
    public ProfileByIdSpec(Guid id, bool withDetails = false)
    {
        Query.Where(p => p.Id == id);
        ApplyIncludes(withDetails);
    }

    private void ApplyIncludes(bool withDetails)
    {
        Query.Include(p => p.FullName);

        if (!withDetails)
            return;

        Query.Include(p => p.Contacts);
        Query.Include(p => p.Educations);
        Query.Include(p => p.EmploymentHistories);
    }
}

/// <summary>Спецификация поиска профиля по AccountId (GUID аккаунта Keycloak).</summary>
public sealed class ProfileByAccountIdSpec : Specification<Profile>
{
    /// <param name="accountId">GUID аккаунта пользователя в Keycloak.</param>
    /// <param name="withDetails">
    /// Загружать контакты, историю занятости и образование.
    /// По умолчанию загружается только <see cref="Profile.FullName"/>.
    /// </param>
    public ProfileByAccountIdSpec(Guid accountId, bool withDetails = false)
    {
        Query.Where(p => p.AccountId == accountId);
        ApplyIncludes(withDetails);
    }

    private void ApplyIncludes(bool withDetails)
    {
        Query.Include(p => p.FullName);

        if (!withDetails)
            return;

        Query.Include(p => p.Contacts);
        Query.Include(p => p.Educations);
        Query.Include(p => p.EmploymentHistories);
    }
}
