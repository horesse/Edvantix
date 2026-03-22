using Edvantix.Chassis.Utilities;
using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.UpdateAvatar;

public sealed class UpdateAvatarCommand : ICommand<Guid>
{
    public required IFormFile Avatar { get; init; }
}

public sealed class UpdateAvatarCommandHandler(IServiceProvider provider)
    : ICommandHandler<UpdateAvatarCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        UpdateAvatarCommand command,
        CancellationToken cancellationToken
    )
    {
        var profileId = provider.GetProfileIdOrError();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();
        var blobService = provider.GetRequiredService<IBlobService>();

        var spec = ProfileSpecification.ForWrite(profileId);
        var profile =
            await profileRepo.FindAsync(spec, cancellationToken)
            ?? throw new NotFoundException("Профиль не найден.");

        var newAvatarUrn = await blobService.UploadFileAsync(command.Avatar, cancellationToken);

        try
        {
            profile.UploadAvatar(newAvatarUrn);

            await profileRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        catch
        {
            await blobService.DeleteFileAsync(newAvatarUrn, cancellationToken);
            throw;
        }

        return profile.Id;
    }
}
