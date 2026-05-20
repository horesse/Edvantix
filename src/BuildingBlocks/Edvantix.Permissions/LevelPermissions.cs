namespace Edvantix.Permissions;

/// <summary>
/// Полные коды разрешений функциональной области "Уровни".
/// Формат: <c>{FeatureCode}.{Code}</c>.
/// Используются в атрибуте <c>[RequirePermission]</c> и при проверках авторизации.
/// </summary>
public static class LevelPermissions
{
    /// <summary>Просмотр справочника уровней.</summary>
    public const string View = "Level.View";

    /// <summary>Создание, редактирование, удаление и изменение статуса уровней.</summary>
    public const string Manage = "Level.Manage";
}
