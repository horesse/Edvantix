namespace Edvantix.Notification.Infrastructure.Senders.InApp;

/// <summary>
/// Реализация <see cref="IInAppSender"/>: сохраняет уведомление в БД.
/// Для in-app уведомлений «транспортом» является само хранилище.
/// </summary>
internal sealed class InAppNotificationSender(
    IInAppNotificationRepository repository,
    ILogger<InAppNotificationSender> logger
) : IInAppSender
{
    /// <inheritdoc />
    public async Task SendAsync(
        InAppNotificationMessage message,
        CancellationToken cancellationToken = default
    )
    {
        var notification = new InAppNotification(
            message.AccountId,
            message.Type,
            message.Title,
            message.Message,
            message.Metadata
        );

        await repository.AddAsync(notification, cancellationToken);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "In-app notification {NotificationId} sent to account {AccountId} (type: {Type})",
            notification.Id,
            message.AccountId,
            message.Type
        );
    }
}
