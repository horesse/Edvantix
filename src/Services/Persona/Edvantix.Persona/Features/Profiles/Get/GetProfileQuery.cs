namespace Edvantix.Persona.Features.Profiles.Get;

/// <summary>
/// Запрос краткого профиля. Приоритет идентификаторов:
/// <list type="number">
///   <item><see cref="ProfileId"/> — если задан.</item>
///   <item>AccountId текущего аутентифицированного пользователя — иначе.</item>
/// </list>
/// </summary>
public sealed record GetProfileQuery : IQuery<ProfileDto>;

public sealed class GetProfileQueryHandler(
    IProfileRepository repository,
    ClaimsPrincipal claims,
    IMapper<Profile, ProfileDto> mapper
) : IQueryHandler<GetProfileQuery, ProfileDto>
{
    public async ValueTask<ProfileDto> Handle(
        GetProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileId = claims.TryGetProfileId();

        if (!profileId.HasValue || profileId == Guid.Empty)
            throw new NotFoundException("Профиль не найден.");

        var spec = ProfileSpecification.Minimal(profileId.Value);

        var profile = await repository.FindAsync(spec, cancellationToken);

        Guard.Against.NotFound(profile, profileId.Value);

        return mapper.Map(profile);
    }
}
