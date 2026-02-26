namespace Edvantix.Notification.Features.Notifications.MarkAllNotificationsAsRead;

/// <summary>
/// Команда: пометить все непрочитанные уведомления пользователя как прочитанные.
/// Выполняется bulk-операцией через репозиторий.
/// </summary>
public sealed record MarkAllNotificationsAsReadCommand(Guid AccountId) : IRequest;

/// <summary>
/// Обработчик команды: bulk-обновление всех непрочитанных уведомлений пользователя.
/// </summary>
public sealed class MarkAllNotificationsAsReadCommandHandler(IServiceProvider provider)
    : IRequestHandler<MarkAllNotificationsAsReadCommand>
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
