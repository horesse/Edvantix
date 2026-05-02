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
        ILogger logger,
        GlobalLogBuffer logBuffer,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await keycloakAdminService.EnableUserAsync(@event.AccountId, cancellationToken);

            logger.LogInformation(
                "Учётная запись {AccountId} включена в Keycloak через событие {EventId}",
                @event.AccountId,
                @event.Id
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ошибка включения аккаунта {AccountId} в Keycloak, событие {EventId}",
                @event.AccountId,
                @event.Id
            );
            logBuffer.Flush();
            throw;
        }
    }
}
