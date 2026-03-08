using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.UpdateAvatar;

/// <summary>
/// Загружает аватар в хранилище до выполнения команды.
/// Устанавливает <see cref="UpdateAvatarCommand.AvatarUrn"/> для использования обработчиком.
/// </summary>
public sealed class UpdateAvatarPreProcessor(IBlobService blobService)
    : MessagePreProcessor<UpdateAvatarCommand, ProfileDetailsModel>
{
    protected override async ValueTask Handle(UpdateAvatarCommand request, CancellationToken ct)
    {
        request.AvatarUrn = await blobService.UploadFileAsync(request.Avatar, ct);
    }
}
