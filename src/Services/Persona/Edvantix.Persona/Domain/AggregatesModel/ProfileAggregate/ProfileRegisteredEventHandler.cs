namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

/// <summary>
/// Обрабатывает доменное событие регистрации профиля.
/// Логирует факт регистрации; в будущем здесь будет публикация интеграционного события
/// в шину (MassTransit) для уведомления других сервисов.
/// </summary>
public sealed class ProfileRegisteredEventHandler(ILogger<ProfileRegisteredEventHandler> logger)
    : INotificationHandler<ProfileRegisteredEvent>
{
    public ValueTask Handle(
        ProfileRegisteredEvent notification,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Profile registered: AccountId={AccountId}, Login={Login}",
            notification.AccountId,
            notification.Login
        );

        return ValueTask.CompletedTask;
    }
}
