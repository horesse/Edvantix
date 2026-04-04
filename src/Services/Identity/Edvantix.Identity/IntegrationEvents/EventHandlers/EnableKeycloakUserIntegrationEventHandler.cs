using Edvantix.Contracts;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает событие <see cref="EnableKeycloakUserIntegrationEvent"/>:
/// включает учётную запись Keycloak.
/// </summary>
public sealed class EnableKeycloakUserIntegrationEventHandler(
    IKeycloakAdminService keycloakAdminService,
    ILogger<EnableKeycloakUserIntegrationEventHandler> logger,
    GlobalLogBuffer logBuffer
) : IConsumer<EnableKeycloakUserIntegrationEvent>
{
    public async Task Consume(ConsumeContext<EnableKeycloakUserIntegrationEvent> context)
    {
        try
        {
            await keycloakAdminService.EnableUserAsync(
                context.Message.AccountId,
                context.CancellationToken
            );

            logger.LogInformation(
                "Учётная запись {AccountId} включена в Keycloak через событие {EventId}",
                context.Message.AccountId,
                context.Message.Id
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ошибка включения аккаунта {AccountId} в Keycloak, событие {EventId}",
                context.Message.AccountId,
                context.Message.Id
            );
            logBuffer.Flush();
            throw;
        }
    }
}

[ExcludeFromCodeCoverage]
public sealed class EnableKeycloakUserIntegrationEventHandlerDefinition
    : ConsumerDefinition<EnableKeycloakUserIntegrationEventHandler>
{
    public EnableKeycloakUserIntegrationEventHandlerDefinition()
    {
        Endpoint(x => x.Name = "identity-enable-user");
        ConcurrentMessageLimit = 10;
    }
}
