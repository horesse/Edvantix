using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.UpdateOwnProfile;

/// <summary>
/// Удаляет предыдущий аватар из хранилища после успешного обновления профиля.
/// Вызывается только если аватар был заменён (<see cref="UpdateOwnProfileCommand.OldAvatarUrn"/> не null).
/// </summary>
public sealed class UpdateOwnProfilePostProcessor(IBlobService blobService)
    : MessagePostProcessor<UpdateOwnProfileCommand, ProfileViewModel>
{
    protected override async ValueTask Handle(
        UpdateOwnProfileCommand request,
        ProfileViewModel response,
        CancellationToken ct
    )
    {
        if (request.OldAvatarUrn is not null)
        {
            await blobService.DeleteFileAsync(request.OldAvatarUrn, ct);
        }
    }
}
