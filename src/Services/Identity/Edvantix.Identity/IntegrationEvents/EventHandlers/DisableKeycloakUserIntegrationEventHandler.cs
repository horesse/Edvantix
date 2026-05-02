using Edvantix.Contracts;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.IntegrationEvents.EventHandlers;

public static class DisableKeycloakUserIntegrationEventHandler
{
    public static async Task Handle(
        DisableKeycloakUserIntegrationEvent @event,
        IKeycloakAdminService keycloakAdminService,
        ILogger logger,
        GlobalLogBuffer logBuffer,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await keycloakAdminService.DisableUserAsync(@event.AccountId, cancellationToken);

            logger.LogInformation(
                "Учётная запись {AccountId} отключена в Keycloak через событие {EventId}",
                @event.AccountId,
                @event.Id
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ошибка отключения аккаунта {AccountId} в Keycloak, событие {EventId}",
                @event.AccountId,
                @event.Id
            );
            logBuffer.Flush();
            throw;
        }
    }
}
