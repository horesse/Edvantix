using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.DeleteAvatar;

/// <summary>DELETE /v1/profile/avatar — удалить аватар профиля.</summary>
public sealed class DeleteAvatarCommand : ICommand<Guid>;

public sealed class DeleteAvatarCommandHandler(IServiceProvider provider)
    : ICommandHandler<DeleteAvatarCommand, Guid>
{
    public async ValueTask<Guid> Handle(
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

        return profileId;
    }
}
