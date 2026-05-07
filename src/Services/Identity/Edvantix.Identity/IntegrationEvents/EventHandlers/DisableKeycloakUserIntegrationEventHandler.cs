using Edvantix.Contracts;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.IntegrationEvents.EventHandlers;

public sealed class DisableKeycloakUserIntegrationEventHandler(
    IKeycloakAdminService keycloakAdminService
)
{
    public async Task Handle(
        DisableKeycloakUserIntegrationEvent @event,
        CancellationToken cancellationToken
    )
    {
        await keycloakAdminService.DisableUserAsync(@event.AccountId, cancellationToken);
    }
}
