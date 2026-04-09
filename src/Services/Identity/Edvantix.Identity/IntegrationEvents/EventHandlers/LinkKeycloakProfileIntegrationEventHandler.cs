using Edvantix.Contracts;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает событие <see cref="LinkKeycloakProfileIntegrationEvent"/>:
/// привязывает profileId к учётной записи Keycloak.
/// </summary>
public sealed class LinkKeycloakProfileIntegrationEventHandler(
    IKeycloakAdminService keycloakAdminService,
    ILogger<LinkKeycloakProfileIntegrationEventHandler> logger,
    GlobalLogBuffer logBuffer
) : IConsumer<LinkKeycloakProfileIntegrationEvent>
{
    public async Task Consume(ConsumeContext<LinkKeycloakProfileIntegrationEvent> context)
    {
        try
        {
            await keycloakAdminService.SetProfileIdAsync(
                context.Message.AccountId,
                context.Message.ProfileId,
                context.CancellationToken
            );

            logger.LogInformation(
                "ProfileId {ProfileId} привязан к аккаунту {AccountId} в Keycloak через событие {EventId}",
                context.Message.ProfileId,
                context.Message.AccountId,
                context.Message.Id
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ошибка привязки ProfileId {ProfileId} к аккаунту {AccountId}, событие {EventId}",
                context.Message.ProfileId,
                context.Message.AccountId,
                context.Message.Id
            );
            logBuffer.Flush();
            throw;
        }
    }
}

[ExcludeFromCodeCoverage]
public sealed class LinkKeycloakProfileIntegrationEventHandlerDefinition
    : ConsumerDefinition<LinkKeycloakProfileIntegrationEventHandler>
{
    public LinkKeycloakProfileIntegrationEventHandlerDefinition()
    {
        Endpoint(x => x.Name = "identity-link-profile");
        ConcurrentMessageLimit = 10;
    }
}
