namespace Edvantix.Notification.Features.Notifications.MarkNotificationAsRead;

using Mediator;

/// <summary>
/// Команда: пометить одно уведомление как прочитанное.
/// Проверяет принадлежность уведомления пользователю через <see cref="AccountId"/>.
/// </summary>
public sealed record MarkNotificationAsReadCommand(Guid NotificationId, Guid AccountId)
    : ICommand<bool>;

/// <summary>
/// Обработчик команды: ищет уведомление по id+accountId, помечает прочитанным.
/// Возвращает false если уведомление не найдено (→ 404 в эндпоинте).
/// </summary>
public sealed class MarkNotificationAsReadCommandHandler(IServiceProvider provider)
    : ICommandHandler<MarkNotificationAsReadCommand, bool>
{
    /// <inheritdoc />
    public async ValueTask<bool> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<IInAppNotificationRepository>();
        var spec = new InAppNotificationByIdAndAccountSpec(
            request.NotificationId,
            request.AccountId
        );
        var notification = await repo.FindAsync(spec, cancellationToken);

        if (notification is null)
        {
            return false;
        }

        notification.MarkAsRead();
        await repo.UnitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
