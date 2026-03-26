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

        if (profileId is null || profileId == Guid.Empty)
        {
            var accountId = claims.GetUserId();
            var userByAccountSpec = new ProfileAccountSpecification(accountId);
            var profileByAccount = await repository.FindAsync(userByAccountSpec, cancellationToken);

            if (profileByAccount is null)
                throw new NotFoundException("Account not found.");

            profileId = profileByAccount.Id;
        }

        var spec = ProfileSpecification.Minimal(profileId.Value);

        var profile = await repository.FindAsync(spec, cancellationToken);

        Guard.Against.NotFound(profile, profileId.Value);

        return mapper.Map(profile);
    }
}
