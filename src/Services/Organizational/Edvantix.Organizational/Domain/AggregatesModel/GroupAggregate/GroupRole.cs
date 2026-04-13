using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Кастомная роль участника в учебной группе, управляемая набором разрешений (<see cref="Permission"/>).
/// Организация задаёт собственные наименования ролей с нужным набором разрешений.
/// </summary>
public sealed class GroupRole() : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
    private readonly List<Permission> _permissions = [];

    /// <param name="organizationId">Идентификатор организации-владельца роли.</param>
    /// <param name="code">Уникальный код роли в группе.</param>
    /// <param name="description">Описание роли и её полномочий в группе.</param>
    public GroupRole(Guid organizationId, string code, string? description = null)
        : this()
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );

        Guard.Against.NullOrWhiteSpace(code, nameof(code));

        OrganizationId = organizationId;
        Code = code.Trim();
        Description = description?.Trim();
        IsDeleted = false;
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Уникальный код роли в группе.</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Описание роли и её полномочий в группе.</summary>
    public string? Description { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>Разрешения, связанные с ролью.</summary>
    public IReadOnlyList<Permission> Permissions => _permissions;

    /// <summary>Обновляет код и описание роли.</summary>
    public void Update(string code, string? description)
    {
        Guard.Against.NullOrWhiteSpace(code, nameof(code));
        Code = code.Trim();
        Description = description?.Trim();
    }

    /// <summary>Добавляет разрешение к роли. Дублирование по идентификатору игнорируется.</summary>
    public void AddPermission(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);
        if (_permissions.All(p => p.Id != permission.Id))
            _permissions.Add(permission);
    }

    /// <summary>Назначает набор разрешений роли, заменяя текущий.</summary>
    public void AssignPermissions(IEnumerable<Permission> permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);
        _permissions.Clear();
        _permissions.AddRange(permissions.Where(p => p is not null));
    }

    /// <summary>Удаляет разрешение из роли.</summary>
    public void RemovePermission(Guid permissionId)
    {
        var entry = _permissions.FirstOrDefault(p => p.Id == permissionId);
        if (entry is not null)
            _permissions.Remove(entry);
    }

    /// <inheritdoc />
    public void Delete() => IsDeleted = true;
}
