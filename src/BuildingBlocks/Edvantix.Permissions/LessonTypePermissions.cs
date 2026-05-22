namespace Edvantix.Permissions;

/// <summary>
/// Полные коды разрешений функциональной области "Типы занятий".
/// Формат: <c>{FeatureCode}.{Code}</c>.
/// Используются в атрибуте <c>[RequirePermission]</c> и при проверках авторизации.
/// </summary>
public static class LessonTypePermissions
{
    /// <summary>Просмотр справочника типов занятий.</summary>
    public const string View = "LessonType.View";

    /// <summary>Создание, редактирование и изменение статуса типов занятий.</summary>
    public const string Manage = "LessonType.Manage";
}
