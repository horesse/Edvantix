namespace Edvantix.Organizational.Domain.Permissions;

/// <summary>
/// Полные коды разрешений функциональной области "Организация".
/// Формат: <c>{FeatureCode}.{Code}</c>.
/// Используются в атрибуте <c>[RequirePermission]</c> и при проверках авторизации.
/// </summary>
public static class OrganizationPermissions
{
    /// <summary>Просмотр организации.</summary>
    public const string View = "Organization.View";

    /// <summary>Редактирование организации.</summary>
    public const string Edit = "Organization.Edit";

    /// <summary>Удаление организации.</summary>
    public const string Delete = "Organization.Delete";

    /// <summary>Приглашение участников.</summary>
    public const string Members = "Organization.Members";

    /// <summary>Управление ролями.</summary>
    public const string Roles = "Organization.Roles";

    /// <summary>Управление группами.</summary>
    public const string Groups = "Organization.Groups";

    /// <summary>Просмотр аналитики.</summary>
    public const string Analytics = "Organization.Analytics";

    /// <summary>Управление подпиской.</summary>
    public const string Subscription = "Organization.Subscription";

    /// <summary>Управление кабинетами.</summary>
    public const string Rooms = "Organization.Rooms";
}
