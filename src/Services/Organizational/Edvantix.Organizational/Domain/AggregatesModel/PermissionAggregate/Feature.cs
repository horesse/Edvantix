using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

/// <summary>
/// Функциональная область (feature) — логическая группировка разрешений одного сервиса.
/// Примеры: «Организация», «Группы», «Финансы».
/// Создаётся через <c>PermissionsDbSeeder</c> или gRPC-эндпоинт <c>SyncFeaturePermissions</c>.
/// </summary>
public sealed class Feature() : Entity, IAggregateRoot
{
    /// <param name="serviceCode">Код сервиса-владельца (например, "organizational").</param>
    /// <param name="code">Машиночитаемый код области (например, "Organization"). Уникален глобально.</param>
    /// <param name="name">Отображаемое название для UI (например, "Организация").</param>
    internal Feature(string serviceCode, string code, string name)
        : this()
    {
        Guard.Against.NullOrWhiteSpace(serviceCode, nameof(serviceCode));
        Guard.Against.NullOrWhiteSpace(code, nameof(code));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Id = Guid.CreateVersion7();
        ServiceCode = serviceCode.Trim();
        Code = code.Trim();
        Name = name.Trim();
    }

    /// <summary>Код сервиса-владельца (например, "organizational").</summary>
    public string ServiceCode { get; private set; } = string.Empty;

    /// <summary>Машиночитаемый код области (например, "Organization"). Уникален глобально.</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Отображаемое название для UI (например, "Организация").</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Обновляет отображаемое название области.</summary>
    internal void Update(string name)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Name = name.Trim();
    }
}
