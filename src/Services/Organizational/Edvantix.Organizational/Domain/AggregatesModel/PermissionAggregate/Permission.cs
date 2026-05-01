using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

/// <summary>
/// Разрешение в рамках функциональной области <see cref="Feature"/>.
/// Создаётся и изменяется исключительно через методы агрегата <see cref="Feature"/>.
/// Машиночитаемый <see cref="Code"/> используется в проверках авторизации;
/// <see cref="Name"/> — только для отображения на UI.
/// </summary>
public sealed class Permission() : Entity
{
    internal Permission(Feature feature, string code, string name)
        : this()
    {
        Feature = feature;
        Code = code;
        Name = name;
    }

    /// <summary>Внешний ключ к <see cref="Feature"/>.</summary>
    public Guid FeatureId { get; private set; }

    /// <summary>Функциональная область, к которой относится разрешение.</summary>
    public Feature Feature { get; private set; } = null!;

    /// <summary>Машиночитаемый код разрешения. Используется в проверках авторизации.</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Отображаемое название для UI.</summary>
    public string Name { get; private set; } = string.Empty;

    internal void UpdateName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Name = name.Trim();
    }

    internal IReadOnlyList<OrganizationMemberRole> OrganizationMemberRoles =>
        _organizationMemberRoles;

    private readonly List<OrganizationMemberRole> _organizationMemberRoles = [];
}
