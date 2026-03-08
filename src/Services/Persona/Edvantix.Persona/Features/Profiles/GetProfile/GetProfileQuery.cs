using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.GetProfile;

/// <summary>
/// Запрос краткого профиля. Приоритет идентификаторов:
/// <list type="number">
///   <item><see cref="ProfileId"/> — если задан.</item>
///   <item><see cref="AccountId"/> — если задан.</item>
///   <item>AccountId текущего аутентифицированного пользователя — иначе.</item>
/// </list>
/// </summary>
public sealed record GetProfileQuery(Guid? ProfileId = null, Guid? AccountId = null)
    : IQuery<ProfileViewModel>;

public sealed class GetProfileQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetProfileQuery, ProfileViewModel>
{
    public async ValueTask<ProfileViewModel> Handle(GetProfileQuery request, CancellationToken ct)
    {
        var profileRepo = provider.GetRequiredService<IProfileRepository>();
        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileViewModel>>();

        ISpecification<Profile> spec =
            request.ProfileId.HasValue ? new ProfileByIdSpec(request.ProfileId.Value)
            : request.AccountId.HasValue ? new ProfileByAccountIdSpec(request.AccountId.Value)
            : new ProfileByAccountIdSpec(provider.GetUserId());

        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        return mapper.Map(profile);
    }
}
