namespace Edvantix.Notification.Features.Notifications.MarkAllNotificationsAsRead;

using Mediator;

/// <summary>
/// Команда: пометить все непрочитанные уведомления пользователя как прочитанные.
/// Выполняется bulk-операцией через репозиторий.
/// </summary>
public sealed record MarkAllNotificationsAsReadCommand(Guid AccountId) : ICommand;

/// <summary>
/// Обработчик команды: bulk-обновление всех непрочитанных уведомлений пользователя.
/// </summary>
public sealed class MarkAllNotificationsAsReadCommandHandler(IServiceProvider provider)
    : ICommandHandler<MarkAllNotificationsAsReadCommand>
{
    /// <inheritdoc />
    public async ValueTask<Unit> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<IInAppNotificationRepository>();
        await repo.MarkAllAsReadAsync(request.AccountId, cancellationToken);

        return Unit.Value;
    }
}
