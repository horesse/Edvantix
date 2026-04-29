namespace Edvantix.Audit.Domain.Enums;

/// <summary>
/// Тип действия, зафиксированного в журнале аудита.
/// </summary>
public enum AuditAction
{
    /// <summary>Создание сущности.</summary>
    Created,

    /// <summary>Обновление сущности.</summary>
    Updated,

    /// <summary>Удаление сущности.</summary>
    Deleted,

    /// <summary>Архивирование сущности.</summary>
    Archived,

    /// <summary>Восстановление сущности из архива.</summary>
    Restored,

    /// <summary>Отправка приглашения.</summary>
    InvitationSent,

    /// <summary>Принятие приглашения.</summary>
    InvitationAccepted,

    /// <summary>Отклонение приглашения.</summary>
    InvitationDeclined,

    /// <summary>Отзыв приглашения.</summary>
    InvitationRevoked,

    /// <summary>Назначение роли.</summary>
    RoleAssigned,

    /// <summary>Изменение роли.</summary>
    RoleChanged,

    /// <summary>Выдача разрешения.</summary>
    PermissionGranted,

    /// <summary>Отзыв разрешения.</summary>
    PermissionRevoked,

    /// <summary>Изменение статуса.</summary>
    StatusChanged,
}
