using Edvantix.SharedKernel.Results;

namespace Edvantix.Notification.Infrastructure;

/// <summary>
/// Сервис управления in-app уведомлениями (чтение и пометка как прочитанное).
/// Создание уведомлений выполняется через <see cref="Senders.InApp.IInAppSender"/>.
/// </summary>
public sealed class InAppNotificationService(IInAppNotificationRepository repository)
{
    /// <summary>
    /// Возвращает страницу уведомлений пользователя в формате <see cref="PagedResult{T}"/>.
    /// </summary>
    public async Task<PagedResult<InAppNotification>> GetAsync(
        Guid accountId,
        int pageIndex,
        int pageSize,
        bool? isRead,
        CancellationToken cancellationToken = default
    )
    {
        var spec = new InAppNotificationsByAccountSpec(accountId, pageIndex, pageSize, isRead);
        var countSpec = new InAppNotificationsCountSpec(accountId, isRead);

        var (items, totalCount) = await repository.ListPagedAsync(spec, countSpec, cancellationToken);

        return new PagedResult<InAppNotification>(items, pageIndex, pageSize, totalCount);
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
    ) => await repository.MarkAllAsReadAsync(accountId, cancellationToken);
}
