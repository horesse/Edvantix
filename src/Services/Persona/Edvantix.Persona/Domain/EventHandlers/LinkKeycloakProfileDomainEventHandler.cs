using Edvantix.Persona.Domain.Events;
using Edvantix.Persona.Infrastructure.Keycloak;

namespace Edvantix.Persona.Domain.EventHandlers;

public sealed class LinkKeycloakProfileDomainEventHandler(
    IKeycloakAdminService service,
    ILogger<LinkKeycloakProfileDomainEventHandler> logger
) : INotificationHandler<LinkKeycloakProfileDomainEvent>
{
    public async ValueTask Handle(
        LinkKeycloakProfileDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        logger.LogWarning(
            "Профиль {ProfileId} будет принудительно привязан к {AccountId} в keycloak",
            notification.ProfileId,
            notification.AccountId
        );

        await service.SetProfileIdAsync(
            notification.AccountId,
            notification.ProfileId,
            cancellationToken
        );
    }
}
