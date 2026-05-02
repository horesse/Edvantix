using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

/// <summary>
/// Агрегат «Разрешение». Описывает конкретное право доступа в рамках функциональной области сервиса.
/// <para>
/// Разрешения регистрируются при старте через <c>PermissionsDbSeeder</c> (на основе <c>PermissionModule</c>)
/// или через gRPC-эндпоинт <c>SyncFeaturePermissions</c> для внешних сервисов.
/// </para>
/// Для авторизационных проверок используется <see cref="FullCode"/>.
/// </summary>
public sealed class Permission() : Entity, IAggregateRoot
{
    /// <param name="serviceCode">Код сервиса-владельца (например, "organizational").</param>
    /// <param name="featureCode">Код функциональной области (например, "Organization").</param>
    /// <param name="featureName">Отображаемое название области для UI.</param>
    /// <param name="code">Машиночитаемый код разрешения (например, "View").</param>
    /// <param name="name">Отображаемое название разрешения для UI.</param>
    internal Permission(
        string serviceCode,
        string featureCode,
        string featureName,
        string code,
        string name
    )
        : this()
    {
        Guard.Against.NullOrWhiteSpace(serviceCode, nameof(serviceCode));
        Guard.Against.NullOrWhiteSpace(featureCode, nameof(featureCode));
        Guard.Against.NullOrWhiteSpace(featureName, nameof(featureName));
        Guard.Against.NullOrWhiteSpace(code, nameof(code));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        ServiceCode = serviceCode.Trim();
        FeatureCode = featureCode.Trim();
        FeatureName = featureName.Trim();
        Code = code.Trim();
        Name = name.Trim();
    }

    /// <summary>Код сервиса-владельца (например, "organizational").</summary>
    public string ServiceCode { get; private set; } = string.Empty;

    /// <summary>Код функциональной области (например, "Organization").</summary>
    public string FeatureCode { get; private set; } = string.Empty;

    /// <summary>Отображаемое название функциональной области для UI.</summary>
    public string FeatureName { get; private set; } = string.Empty;

    /// <summary>Машиночитаемый код разрешения (например, "View").</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Отображаемое название разрешения для UI.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Полный авторизационный код в формате <c>{FeatureCode}.{Code}</c> (например, "Organization.View").
    /// Используется в <c>[RequirePermission]</c> и кеше авторизации.
    /// </summary>
    public string FullCode => $"{FeatureCode}.{Code}";

    /// <summary>Обновляет отображаемые названия области и разрешения.</summary>
    internal void Update(string featureName, string name)
    {
        Guard.Against.NullOrWhiteSpace(featureName, nameof(featureName));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        FeatureName = featureName.Trim();
        Name = name.Trim();
    }

    internal IReadOnlyList<OrganizationMemberRole> OrganizationMemberRoles =>
        _organizationMemberRoles;

    private readonly List<OrganizationMemberRole> _organizationMemberRoles = [];
}
