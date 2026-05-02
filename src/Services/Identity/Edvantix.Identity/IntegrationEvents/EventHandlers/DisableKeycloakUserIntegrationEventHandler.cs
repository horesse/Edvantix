using Edvantix.Contracts;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.IntegrationEvents.EventHandlers;

public static class DisableKeycloakUserIntegrationEventHandler
{
    public static async Task Handle(
        DisableKeycloakUserIntegrationEvent @event,
        IKeycloakAdminService keycloakAdminService,
        CancellationToken cancellationToken
    )
    {
        await keycloakAdminService.DisableUserAsync(@event.AccountId, cancellationToken);
    }
}
