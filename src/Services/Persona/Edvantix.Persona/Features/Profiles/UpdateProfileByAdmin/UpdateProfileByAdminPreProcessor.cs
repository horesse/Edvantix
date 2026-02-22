using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.UpdateProfileByAdmin;

/// <summary>
/// Загружает новый аватар в хранилище до выполнения команды.
/// Устанавливает <see cref="UpdateProfileByAdminCommand.AvatarUrn"/> для использования обработчиком.
/// </summary>
public sealed class UpdateProfileByAdminPreProcessor(IBlobService blobService)
    : MessagePreProcessor<UpdateProfileByAdminCommand, ProfileViewModel>
{
    protected override async ValueTask Handle(
        UpdateProfileByAdminCommand request,
        CancellationToken ct
    )
    {
        if (request.Avatar is not null)
        {
            request.AvatarUrn = await blobService.UploadFileAsync(request.Avatar, ct);
        }
    }
}
