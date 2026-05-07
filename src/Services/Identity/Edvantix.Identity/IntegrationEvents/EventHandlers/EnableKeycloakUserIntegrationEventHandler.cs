using Edvantix.Contracts;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает событие <see cref="EnableKeycloakUserIntegrationEvent"/>:
/// включает учётную запись Keycloak.
/// </summary>
internal sealed class EnableKeycloakUserIntegrationEventHandler(
    IKeycloakAdminService keycloakAdminService
)
{
    public async Task Handle(
        EnableKeycloakUserIntegrationEvent @event,
        CancellationToken cancellationToken
    )
    {
        await keycloakAdminService.EnableUserAsync(@event.AccountId, cancellationToken);
    }
}
