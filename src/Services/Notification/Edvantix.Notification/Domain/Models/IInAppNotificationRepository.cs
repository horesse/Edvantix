using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;

namespace Edvantix.Notification.Domain.Models;

/// <summary>
/// Репозиторий in-app уведомлений. Следует паттерну репозитория из Chassis.
/// </summary>
public interface IInAppNotificationRepository : IRepository<InAppNotification>
{
    /// <summary>Добавляет новое уведомление в хранилище.</summary>
    Task AddAsync(InAppNotification notification, CancellationToken cancellationToken = default);

    /// <summary>Возвращает уведомление по спецификации или null.</summary>
    Task<InAppNotification?> FindAsync(
        ISpecification<InAppNotification> spec,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает страницу уведомлений и общее количество.</summary>
    Task<(IReadOnlyList<InAppNotification> Items, int TotalCount)> ListPagedAsync(
        ISpecification<InAppNotification> spec,
        ISpecification<InAppNotification> countSpec,
        CancellationToken cancellationToken = default
    );

    /// <summary>Подсчитывает количество уведомлений по спецификации.</summary>
    Task<int> CountAsync(
        ISpecification<InAppNotification> spec,
        CancellationToken cancellationToken = default
    );

    /// <summary>Отмечает все непрочитанные уведомления пользователя как прочитанные.</summary>
    Task MarkAllAsReadAsync(Guid profileId, CancellationToken cancellationToken = default);
}
