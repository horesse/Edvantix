using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.Avatar.UpdateAvatar;

public sealed class UpdateAvatarCommand : ICommand<Guid>
{
    public required IFormFile Avatar { get; init; }
}

public sealed class UpdateAvatarCommandHandler(
    IProfileRepository repository,
    IBlobService blobService,
    ClaimsPrincipal claims
) : ICommandHandler<UpdateAvatarCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        UpdateAvatarCommand command,
        CancellationToken cancellationToken
    )
    {
        var profileId = claims.GetProfileIdOrError();

        var spec = ProfileSpecification.ForWrite(profileId);
        var profile = await repository.FindAsync(spec, cancellationToken);

        Guard.Against.NotFound(profile, profileId);

        var newAvatarUrn = await blobService.UploadFileAsync(command.Avatar, cancellationToken);

        try
        {
            profile.UploadAvatar(newAvatarUrn);

            await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        catch
        {
            await blobService.DeleteFileAsync(newAvatarUrn, cancellationToken);
            throw;
        }

        return profile.Id;
    }
}
