using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Builders;

namespace Edvantix.Notification.Domain.Models;

/// <summary>
/// Спецификация для получения уведомлений пользователя с поддержкой пагинации и фильтрации.
/// </summary>
public sealed class InAppNotificationsByAccountSpec : Specification<InAppNotification>
{
    /// <summary>
    /// Возвращает страницу уведомлений пользователя.
    /// </summary>
    /// <param name="profileId">Идентификатор профиля пользователя.</param>
    /// <param name="page">Номер страницы (с 1).</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <param name="isRead">Фильтр по прочитанности; null — возвращать все.</param>
    public InAppNotificationsByAccountSpec(
        Guid profileId,
        int page,
        int pageSize,
        bool? isRead = null
    )
    {
        Query.Where(n => n.ProfileId == profileId);

        if (isRead.HasValue)
        {
            Query.Where(n => n.IsRead == isRead.Value);
        }

        Query.OrderByDescending(n => n.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
    }
}

/// <summary>
/// Спецификация для подсчёта уведомлений пользователя (без пагинации).
/// </summary>
public sealed class InAppNotificationsCountSpec : Specification<InAppNotification>
{
    public InAppNotificationsCountSpec(Guid profileId, bool? isRead = null)
    {
        Query.Where(n => n.ProfileId == profileId);

        if (isRead.HasValue)
        {
            Query.Where(n => n.IsRead == isRead.Value);
        }
    }
}

/// <summary>
/// Спецификация для нахождения конкретного уведомления по id и profile_id.
/// Включает проверку владельца, чтобы исключить несанкционированный доступ.
/// </summary>
public sealed class InAppNotificationByIdAndAccountSpec : Specification<InAppNotification>
{
    public InAppNotificationByIdAndAccountSpec(Guid id, Guid profileId)
    {
        Query.Where(n => n.Id == id && n.ProfileId == profileId);
    }
}
