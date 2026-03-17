using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.Details;

/// <summary>
/// Запрос полного профиля с контактами, образованием и историей занятости.
/// Приоритет идентификаторов:
/// <list type="number">
///   <item><see cref="ProfileId"/> — если задан.</item>
///   <item>AccountId текущего аутентифицированного пользователя — иначе.</item>
/// </list>
/// </summary>
public sealed record GetProfileDetailsQuery(Guid? ProfileId = null) : IQuery<ProfileDetailsModel>;

public sealed class GetProfileDetailsQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetProfileDetailsQuery, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        GetProfileDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileId = request.ProfileId ?? provider.GetProfileIdOrError();

        var profileRepo = provider.GetRequiredService<IProfileRepository>();
        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileDetailsModel>>();

        ISpecification<Profile> spec = new ProfileSpecification(profileId, withDetails: true);

        var profile =
            await profileRepo.FindAsync(spec, cancellationToken)
            ?? throw new NotFoundException("Профиль не найден.");

        return mapper.Map(profile);
    }
}
