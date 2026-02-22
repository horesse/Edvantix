using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.UpdateOwnProfile;

/// <summary>
/// Загружает новый аватар в хранилище до выполнения команды.
/// Устанавливает <see cref="UpdateOwnProfileCommand.AvatarUrn"/> для использования обработчиком.
/// </summary>
public sealed class UpdateOwnProfilePreProcessor(IBlobService blobService)
    : MessagePreProcessor<UpdateOwnProfileCommand, ProfileViewModel>
{
    protected override async ValueTask Handle(UpdateOwnProfileCommand request, CancellationToken ct)
    {
        if (request.Avatar is not null)
        {
            request.AvatarUrn = await blobService.UploadFileAsync(request.Avatar, ct);
        }
    }
}
