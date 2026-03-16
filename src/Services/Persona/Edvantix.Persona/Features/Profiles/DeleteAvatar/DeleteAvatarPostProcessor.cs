using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.DeleteAvatar;

/// <summary>
/// Удаляет аватар из хранилища после успешного выполнения команды.
/// Вызывается только если у профиля был аватар (<see cref="DeleteAvatarCommand.AvatarUrn"/> не null).
/// </summary>
public sealed class DeleteAvatarPostProcessor(IBlobService blobService)
    : MessagePostProcessor<DeleteAvatarCommand, ProfileDetailsModel>
{
    protected override async ValueTask Handle(
        DeleteAvatarCommand request,
        ProfileDetailsModel response,
        CancellationToken ct
    )
    {
        if (request.AvatarUrn is not null)
        {
            await blobService.DeleteFileAsync(request.AvatarUrn, ct);
        }
    }
}
