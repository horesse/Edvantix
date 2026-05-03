using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

/// <summary>
/// Агрегат «Разрешение». Описывает конкретное право доступа внутри функциональной области (<see cref="Feature"/>).
/// <para>
/// Регистрируется при старте через <c>PermissionsDbSeeder</c> (на основе <c>PermissionModule</c>)
/// или через gRPC-эндпоинт <c>SyncFeaturePermissions</c> для внешних сервисов.
/// </para>
/// Для авторизационных проверок используется <see cref="FullCode"/>.
/// </summary>
public sealed class Permission() : Entity, IAggregateRoot
{
    /// <param name="featureCode">Код функциональной области (FK к <see cref="Feature.Code"/>).</param>
    /// <param name="code">Машиночитаемый код разрешения (например, "View").</param>
    /// <param name="name">Отображаемое название разрешения для UI.</param>
    internal Permission(string featureCode, string code, string name)
        : this()
    {
        Guard.Against.NullOrWhiteSpace(featureCode, nameof(featureCode));
        Guard.Against.NullOrWhiteSpace(code, nameof(code));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Id = Guid.CreateVersion7();
        FeatureCode = featureCode.Trim();
        Code = code.Trim();
        Name = name.Trim();
    }

    /// <summary>Код функциональной области (FK к <see cref="Feature.Code"/>).</summary>
    public string FeatureCode { get; private set; } = string.Empty;

    /// <summary>Машиночитаемый код разрешения (например, "View").</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Отображаемое название разрешения для UI.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Полный авторизационный код в формате <c>{FeatureCode}.{Code}</c> (например, "Organization.View").
    /// Используется в <c>[RequirePermission]</c> и кеше авторизации.
    /// </summary>
    public string FullCode => $"{FeatureCode}.{Code}";

    /// <summary>Функциональная область, которой принадлежит разрешение.</summary>
    public Feature? Feature { get; internal set; }

    /// <summary>Обновляет отображаемое название разрешения.</summary>
    internal void Update(string name)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Name = name.Trim();
    }

    internal IReadOnlyList<OrganizationRole> OrganizationRoles => _organizationRoles;

    private readonly List<OrganizationRole> _organizationRoles = [];
}
