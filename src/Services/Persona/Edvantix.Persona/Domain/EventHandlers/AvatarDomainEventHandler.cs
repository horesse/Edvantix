using Edvantix.Persona.Domain.Events;
using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Domain.EventHandlers;

/// <summary>
/// Обрабатывает доменное событие удаления аватара профиля.
/// Удаляет файл из хранилища после успешного сохранения профиля.
/// Событие срабатывает ПОСЛЕ сохранения, поэтому файл удаляется только при успешной транзакции.
/// </summary>
public sealed class AvatarDomainEventHandler(
    IBlobService blobService,
    ILogger<AvatarDomainEventHandler> logger
) : INotificationHandler<AvatarDeletedDomainEvent>
{
    public async ValueTask Handle(
        AvatarDeletedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Удаление аватара из хранилища: {AvatarUrn}", notification.AvatarUrn);

        await blobService.DeleteFileAsync(notification.AvatarUrn, cancellationToken);
    }
}
