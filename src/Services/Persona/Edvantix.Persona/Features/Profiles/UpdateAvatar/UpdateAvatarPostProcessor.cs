using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.UpdateAvatar;

/// <summary>
/// Удаляет предыдущий аватар из хранилища после успешного обновления.
/// Вызывается только если аватар был заменён (<see cref="UpdateAvatarCommand.OldAvatarUrn"/> не null).
/// </summary>
public sealed class UpdateAvatarPostProcessor(IBlobService blobService)
    : MessagePostProcessor<UpdateAvatarCommand, ProfileDetailsModel>
{
    protected override async ValueTask Handle(
        UpdateAvatarCommand request,
        ProfileDetailsModel response,
        CancellationToken ct
    )
    {
        if (request.OldAvatarUrn is not null)
        {
            await blobService.DeleteFileAsync(request.OldAvatarUrn, ct);
        }
    }
}
