namespace Edvantix.Permissions;

/// <summary>
/// Полные коды разрешений функциональной области "Предметы".
/// Формат: <c>{FeatureCode}.{Code}</c>.
/// Используются в атрибуте <c>[RequirePermission]</c> и при проверках авторизации.
/// </summary>
public static class SubjectPermissions
{
    /// <summary>Просмотр справочника предметов.</summary>
    public const string View = "Subject.View";

    /// <summary>Создание, редактирование, архивирование и восстановление предметов.</summary>
    public const string Manage = "Subject.Manage";
}
