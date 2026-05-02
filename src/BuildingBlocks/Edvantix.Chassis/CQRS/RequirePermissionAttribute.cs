namespace Edvantix.Chassis.CQRS;

/// <summary>
/// Указывает, что команда или запрос требуют наличия указанного разрешения у профиля в контексте организации.
/// Проверка выполняется в <c>AuthorizationBehavior</c> до вызова обработчика.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RequirePermissionAttribute(string permission) : Attribute
{
    /// <summary>Полный код разрешения в формате <c>FeatureCode.Code</c> (например, "Organization.View").</summary>
    public string Permission { get; } = permission;
}
