using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.Details;

/// <summary>
/// Запрос полного профиля с контактами, образованием и историей занятости.
/// Приоритет идентификаторов:
/// <list type="number">
///   <item><see cref="ProfileId"/> — если задан.</item>
///   <item><see cref="AccountId"/> — если задан.</item>
///   <item>AccountId текущего аутентифицированного пользователя — иначе.</item>
/// </list>
/// </summary>
public sealed record GetProfileDetailsQuery(Guid? ProfileId = null, Guid? AccountId = null)
    : IQuery<ProfileDetailsModel>;

public sealed class GetProfileDetailsQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetProfileDetailsQuery, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        GetProfileDetailsQuery request,
        CancellationToken ct
    )
    {
        var profileRepo = provider.GetRequiredService<IProfileRepository>();
        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileDetailsModel>>();

        ISpecification<Profile> spec =
            request.ProfileId.HasValue
                ? new ProfileByIdSpec(request.ProfileId.Value, withDetails: true)
            : request.AccountId.HasValue
                ? new ProfileByAccountIdSpec(request.AccountId.Value, withDetails: true)
            : new ProfileByAccountIdSpec(provider.GetUserId(), withDetails: true);

        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        return mapper.Map(profile);
    }
}
