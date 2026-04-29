namespace Edvantix.Audit.Domain.Enums;

/// <summary>
/// Тип сущности, над которой выполнено действие.
/// </summary>
public enum AuditEntityType
{
    /// <summary>Организация.</summary>
    Organization,

    /// <summary>Участник организации.</summary>
    OrganizationMember,

    /// <summary>Группа.</summary>
    Group,

    /// <summary>Участник группы.</summary>
    GroupMember,

    /// <summary>Приглашение.</summary>
    Invitation,

    /// <summary>Роль.</summary>
    Role,

    /// <summary>Разрешение.</summary>
    Permission,

    /// <summary>Профиль пользователя.</summary>
    Profile,
}
