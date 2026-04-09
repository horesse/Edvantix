using Edvantix.Contracts;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает событие <see cref="DisableKeycloakUserIntegrationEvent"/>:
/// отключает учётную запись Keycloak.
/// </summary>
public sealed class DisableKeycloakUserIntegrationEventHandler(
    IKeycloakAdminService keycloakAdminService,
    ILogger<DisableKeycloakUserIntegrationEventHandler> logger,
    GlobalLogBuffer logBuffer
) : IConsumer<DisableKeycloakUserIntegrationEvent>
{
    public async Task Consume(ConsumeContext<DisableKeycloakUserIntegrationEvent> context)
    {
        try
        {
            await keycloakAdminService.DisableUserAsync(
                context.Message.AccountId,
                context.CancellationToken
            );

            logger.LogInformation(
                "Учётная запись {AccountId} отключена в Keycloak через событие {EventId}",
                context.Message.AccountId,
                context.Message.Id
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ошибка отключения аккаунта {AccountId} в Keycloak, событие {EventId}",
                context.Message.AccountId,
                context.Message.Id
            );
            logBuffer.Flush();
            throw;
        }
    }
}

[ExcludeFromCodeCoverage]
public sealed class DisableKeycloakUserIntegrationEventHandlerDefinition
    : ConsumerDefinition<DisableKeycloakUserIntegrationEventHandler>
{
    public DisableKeycloakUserIntegrationEventHandlerDefinition()
    {
        Endpoint(x => x.Name = "identity-disable-user");
        ConcurrentMessageLimit = 10;
    }
}
