using Edvantix.Contracts;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает событие <see cref="EnableKeycloakUserIntegrationEvent"/>:
/// включает учётную запись Keycloak.
/// </summary>
public static class EnableKeycloakUserIntegrationEventHandler
{
    public static async Task Handle(
        EnableKeycloakUserIntegrationEvent @event,
        IKeycloakAdminService keycloakAdminService,
        CancellationToken cancellationToken
    )
    {
        await keycloakAdminService.EnableUserAsync(@event.AccountId, cancellationToken);
    }
}
