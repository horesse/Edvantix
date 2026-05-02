namespace Edvantix.Chassis.Permissions;

/// <summary>
/// Декларативный модуль разрешений для одной функциональной области сервиса.
/// Наследники регистрируются в DI и используются <c>PermissionsDbSeeder</c>
/// для синхронизации разрешений в базе данных при старте приложения.
/// <para>
/// Для подключения разрешений из внешнего сервиса наследник регистрируется в DI
/// того сервиса, после чего разрешения синхронизируются через gRPC-эндпоинт
/// <c>PermissionGrpcService.SyncFeaturePermissions</c>.
/// </para>
/// </summary>
/// <example>
/// <code>
/// internal sealed class OrganizationPermissionModule : PermissionModule
/// {
///     public override string ServiceCode =&gt; "organizational";
///     public override string FeatureCode =&gt; "Organization";
///     public override string FeatureName =&gt; "Организация";
///
///     public override IReadOnlyList&lt;PermissionEntry&gt; GetPermissions() =&gt;
///     [
///         new("View", "Просмотр организации"),
///         new("Edit", "Редактирование организации"),
///     ];
/// }
/// </code>
/// </example>
public abstract class PermissionModule
{
    /// <summary>Код сервиса-владельца (например, "organizational").</summary>
    public abstract string ServiceCode { get; }

    /// <summary>Машиночитаемый код функциональной области (например, "Organization").</summary>
    public abstract string FeatureCode { get; }

    /// <summary>Отображаемое название функциональной области для UI (например, "Организация").</summary>
    public abstract string FeatureName { get; }

    /// <summary>Возвращает список разрешений, принадлежащих данной области.</summary>
    public abstract IReadOnlyList<PermissionEntry> GetPermissions();
}
