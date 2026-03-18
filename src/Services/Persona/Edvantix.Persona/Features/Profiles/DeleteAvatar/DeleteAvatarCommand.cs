using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.DeleteAvatar;

/// <summary>DELETE /v1/profile/avatar — удалить аватар профиля.</summary>
public sealed class DeleteAvatarCommand : ICommand<ProfileDetailsModel>;

public sealed class DeleteAvatarCommandHandler(IServiceProvider provider)
    : ICommandHandler<DeleteAvatarCommand, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        DeleteAvatarCommand command,
        CancellationToken cancellationToken
    )
    {
        var profileId = provider.GetProfileIdOrError();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = ProfileSpecification.ForWrite(profileId);
        var profile =
            await profileRepo.FindAsync(spec, cancellationToken)
            ?? throw new NotFoundException("Профиль не найден.");

        profile.DeleteAvatar();

        await profileRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileDetailsModel>>();
        return mapper.Map(profile);
    }
}
