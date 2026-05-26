using Edvantix.Contracts;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает событие <see cref="UpdateKeycloakFullNameIntegrationEvent"/>:
/// обновляет имя и фамилию пользователя в Keycloak.
/// </summary>
public sealed class UpdateKeycloakFullNameIntegrationEventHandler(
    IKeycloakAdminService keycloakAdminService
)
{
    public async Task Handle(
        UpdateKeycloakFullNameIntegrationEvent @event,
        CancellationToken cancellationToken
    )
    {
        await keycloakAdminService.UpdateFullNameAsync(
            @event.AccountId,
            @event.FirstName,
            @event.LastName,
            cancellationToken
        );
    }
}
