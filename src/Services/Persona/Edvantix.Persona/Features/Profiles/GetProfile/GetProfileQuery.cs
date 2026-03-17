using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.GetProfile;

/// <summary>
/// Запрос краткого профиля. Приоритет идентификаторов:
/// <list type="number">
///   <item><see cref="ProfileId"/> — если задан.</item>
///   <item>AccountId текущего аутентифицированного пользователя — иначе.</item>
/// </list>
/// </summary>
public sealed record GetProfileQuery(Guid? ProfileId = null) : IQuery<ProfileViewModel>;

public sealed class GetProfileQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetProfileQuery, ProfileViewModel>
{
    public async ValueTask<ProfileViewModel> Handle(
        GetProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileRepo = provider.GetRequiredService<IProfileRepository>();
        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileViewModel>>();

        var profileId = request.ProfileId ?? provider.TryGetProfileId();

        if (!profileId.HasValue || profileId == Guid.Empty)
            throw new NotFoundException("Профиль не найден.");

        var spec = new ProfileSpecification(profileId);

        var profile =
            await profileRepo.FindAsync(spec, cancellationToken)
            ?? throw new NotFoundException("Профиль не найден.");

        return mapper.Map(profile);
    }
}
