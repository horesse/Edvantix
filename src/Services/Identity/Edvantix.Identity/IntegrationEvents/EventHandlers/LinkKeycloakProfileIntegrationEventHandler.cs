using Edvantix.Contracts;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает событие <see cref="LinkKeycloakProfileIntegrationEvent"/>:
/// привязывает profileId к учётной записи Keycloak.
/// </summary>
public static class LinkKeycloakProfileIntegrationEventHandler
{
    public static async Task Handle(
        LinkKeycloakProfileIntegrationEvent @event,
        IKeycloakAdminService keycloakAdminService,
        CancellationToken cancellationToken
    )
    {
        await keycloakAdminService.SetProfileIdAsync(
            @event.AccountId,
            @event.ProfileId,
            cancellationToken
        );
    }
}
