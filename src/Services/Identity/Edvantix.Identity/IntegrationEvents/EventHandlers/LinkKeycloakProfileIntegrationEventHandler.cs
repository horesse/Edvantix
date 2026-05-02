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
        ILogger logger,
        GlobalLogBuffer logBuffer,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await keycloakAdminService.SetProfileIdAsync(
                @event.AccountId,
                @event.ProfileId,
                cancellationToken
            );

            logger.LogInformation(
                "ProfileId {ProfileId} привязан к аккаунту {AccountId} в Keycloak через событие {EventId}",
                @event.ProfileId,
                @event.AccountId,
                @event.Id
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ошибка привязки ProfileId {ProfileId} к аккаунту {AccountId}, событие {EventId}",
                @event.ProfileId,
                @event.AccountId,
                @event.Id
            );
            logBuffer.Flush();
            throw;
        }
    }
}
