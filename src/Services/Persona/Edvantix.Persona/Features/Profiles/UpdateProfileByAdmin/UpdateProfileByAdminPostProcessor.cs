using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.UpdateProfileByAdmin;

/// <summary>
/// Удаляет предыдущий аватар из хранилища после успешного обновления профиля.
/// Вызывается только если аватар был заменён (<see cref="UpdateProfileByAdminCommand.OldAvatarUrn"/> не null).
/// </summary>
public sealed class UpdateProfileByAdminPostProcessor(IBlobService blobService)
    : MessagePostProcessor<UpdateProfileByAdminCommand, ProfileViewModel>
{
    protected override async ValueTask Handle(
        UpdateProfileByAdminCommand request,
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
