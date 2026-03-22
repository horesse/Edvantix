using Edvantix.Persona.Domain.Events;
using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Domain.EventHandlers;

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
