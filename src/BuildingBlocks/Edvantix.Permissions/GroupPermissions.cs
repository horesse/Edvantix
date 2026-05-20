namespace Edvantix.Permissions;

/// <summary>
/// Полные коды разрешений функциональной области "Группы".
/// Формат: <c>{FeatureCode}.{Code}</c>.
/// Используются в атрибуте <c>[RequirePermission]</c> и при проверках авторизации.
/// </summary>
public static class GroupPermissions
{
    /// <summary>Создание группы.</summary>
    public const string Create = "Group.Create";

    /// <summary>Просмотр группы.</summary>
    public const string View = "Group.View";

    /// <summary>Редактирование группы.</summary>
    public const string Edit = "Group.Edit";

    /// <summary>Удаление группы.</summary>
    public const string Delete = "Group.Delete";

    /// <summary>Управление участниками группы.</summary>
    public const string Members = "Group.Members";

    /// <summary>Управление учебными материалами группы.</summary>
    public const string Content = "Group.Content";

    /// <summary>Управление расписанием группы.</summary>
    public const string Schedule = "Group.Schedule";
}
