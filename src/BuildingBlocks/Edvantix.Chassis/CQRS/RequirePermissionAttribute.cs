namespace Edvantix.Chassis.CQRS;

/// <summary>
/// Указывает, что команда или запрос требуют наличия указанного разрешения у профиля в контексте организации.
/// Проверка выполняется в <c>AuthorizationBehavior</c> до вызова обработчика.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RequirePermissionAttribute(string permission) : Attribute
{
    /// <summary>Машинный код разрешения из <c>OrganizationPermission</c> или <c>GroupPermission</c>.</summary>
    public string Permission { get; } = permission;
}
