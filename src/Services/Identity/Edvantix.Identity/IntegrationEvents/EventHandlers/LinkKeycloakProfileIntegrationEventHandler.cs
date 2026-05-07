using Edvantix.Contracts;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает событие <see cref="LinkKeycloakProfileIntegrationEvent"/>:
/// привязывает profileId к учётной записи Keycloak.
/// </summary>
internal sealed class LinkKeycloakProfileIntegrationEventHandler(
    IKeycloakAdminService keycloakAdminService
)
{
    public async Task Handle(
        LinkKeycloakProfileIntegrationEvent @event,
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
