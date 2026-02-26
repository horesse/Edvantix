using Edvantix.Notification.Domain.Models;

namespace Edvantix.Notification.Infrastructure;

/// <summary>
/// Сервис управления in-app уведомлениями.
/// Используется как gRPC-сервисом (создание), так и REST-эндпоинтами (чтение, пометка).
/// </summary>
public sealed class InAppNotificationService(
    IInAppNotificationRepository repository,
    ILogger<InAppNotificationService> logger
)
{
    /// <summary>
    /// Возвращает страницу уведомлений пользователя с общим количеством.
    /// </summary>
    public async Task<(IReadOnlyList<InAppNotification> Items, int TotalCount)> GetAsync(
        Guid accountId,
        int page,
        int pageSize,
        bool? isRead,
        CancellationToken cancellationToken = default
    )
    {
        var spec = new InAppNotificationsByAccountSpec(accountId, page, pageSize, isRead);
        var countSpec = new InAppNotificationsCountSpec(accountId, isRead);

        return await repository.ListPagedAsync(spec, countSpec, cancellationToken);
    }

    /// <summary>
    /// Возвращает количество непрочитанных уведомлений пользователя.
    /// </summary>
    public async Task<int> GetUnreadCountAsync(
        Guid accountId,
        CancellationToken cancellationToken = default
    )
    {
        var spec = new InAppNotificationsCountSpec(accountId, isRead: false);

        return await repository.CountAsync(spec, cancellationToken);
    }

    /// <summary>
    /// Помечает одно уведомление как прочитанное.
    /// Проверяет принадлежность уведомления пользователю.
    /// </summary>
    /// <returns>True если уведомление найдено и обновлено, False если не найдено.</returns>
    public async Task<bool> MarkAsReadAsync(
        Guid notificationId,
        Guid accountId,
        CancellationToken cancellationToken = default
    )
    {
        var spec = new InAppNotificationByIdAndAccountSpec(notificationId, accountId);
        var notification = await repository.FindAsync(spec, cancellationToken);

        if (notification is null)
        {
            return false;
        }

        notification.MarkAsRead();
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Помечает все уведомления пользователя как прочитанные через bulk-операцию.
    /// </summary>
    public async Task MarkAllAsReadAsync(
        Guid accountId,
        CancellationToken cancellationToken = default
    )
    {
        await repository.MarkAllAsReadAsync(accountId, cancellationToken);
    }

    /// <summary>
    /// Создаёт новое уведомление для пользователя и сохраняет его в БД.
    /// </summary>
    public async Task<InAppNotification> CreateAsync(
        Guid accountId,
        NotificationType type,
        string title,
        string message,
        string? metadata = null,
        CancellationToken cancellationToken = default
    )
    {
        var notification = new InAppNotification(accountId, type, title, message, metadata);

        await repository.AddAsync(notification, cancellationToken);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "In-app notification {NotificationId} created for account {AccountId} (type: {Type})",
            notification.Id,
            accountId,
            type
        );

        return notification;
    }
}
