using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Events;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;

/// <summary>
/// Роль участника организации с набором разрешений (<see cref="Permission"/>).
/// Организация создаёт роли с отображаемыми именами и назначает им нужные разрешения.
/// Системные роли создаются автоматически при создании организации.
/// Роль «Владелец» дополнительно помечается флагом <see cref="IsOwner"/> и не может быть изменена.
/// </summary>
public sealed class OrganizationRole() : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
    private readonly List<Permission> _permissions = [];

    /// <param name="organizationId">Идентификатор организации-владельца роли.</param>
    /// <param name="name">Отображаемое название роли (например, «Преподаватель»).</param>
    /// <param name="description">Краткое описание: кому назначается эта роль.</param>
    /// <param name="isSystem">Признак системной роли (создана платформой, не удаляется).</param>
    /// <param name="isOwner">Признак роли владельца организации (полный доступ, не редактируется).</param>
    public OrganizationRole(
        Guid organizationId,
        string name,
        string? description = null,
        bool isSystem = false,
        bool isOwner = false
    )
        : this()
    {
        Id = Guid.CreateVersion7();
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );

        Guard.Against.NullOrWhiteSpace(name, nameof(name));

        OrganizationId = organizationId;
        Name = name.Trim();
        Description = description?.Trim();
        IsSystem = isSystem;
        IsOwner = isOwner;
        IsDeleted = false;
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Отображаемое название роли (например, «Преподаватель»).</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Краткое описание: кому назначается эта роль.</summary>
    public string? Description { get; private set; }

    /// <summary>Признак системной роли (создана платформой, не удаляется).</summary>
    public bool IsSystem { get; private init; }

    /// <summary>
    /// Признак роли владельца организации.
    /// Владелец имеет полный доступ ко всем разделам; роль не может быть изменена или удалена.
    /// </summary>
    public bool IsOwner { get; private init; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>Разрешения, связанные с ролью.</summary>
    public IReadOnlyList<Permission> Permissions => _permissions;

    /// <summary>Обновляет название и описание роли.</summary>
    public void Update(string name, string? description)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Name = name.Trim();
        Description = description?.Trim();
    }

    /// <summary>Добавляет разрешение к роли. Дублирование по идентификатору игнорируется.</summary>
    public void AddPermission(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);
        if (_permissions.All(p => p.Id != permission.Id))
        {
            _permissions.Add(permission);
            RegisterDomainEvent(
                new OrganizationRolePermissionsChangedDomainEvent(OrganizationId, Id)
            );
        }
    }

    /// <summary>Назначает набор разрешений роли, заменяя текущий.</summary>
    public void AssignPermissions(IEnumerable<Permission> permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);
        _permissions.Clear();
        _permissions.AddRange(permissions.Where(p => p is not null));
        RegisterDomainEvent(new OrganizationRolePermissionsChangedDomainEvent(OrganizationId, Id));
    }

    /// <summary>Удаляет разрешение из роли.</summary>
    public void RemovePermission(Guid permissionId)
    {
        var entry = _permissions.FirstOrDefault(p => p.Id == permissionId);
        if (entry is not null)
        {
            _permissions.Remove(entry);
            RegisterDomainEvent(
                new OrganizationRolePermissionsChangedDomainEvent(OrganizationId, Id)
            );
        }
    }

    /// <inheritdoc />
    public void Delete() => IsDeleted = true;
}

internal sealed class OrganizationRolePermission
{
    public Guid OrganizationRoleId { get; set; }
    public Guid PermissionId { get; set; }
}
