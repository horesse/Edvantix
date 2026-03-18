namespace Edvantix.Organizations.Domain.AggregatesModel.RoleAggregate;

/// <summary>
/// Represents a named role within a school (tenant).
/// Roles are soft-deletable and tenant-scoped — both filters are combined into a single
/// HasQueryFilter in <c>OrganizationsDbContext</c> because EF Core only supports one
/// HasQueryFilter per entity type.
/// </summary>
public sealed class Role : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
    // Backing field — EF Core accesses this directly via PropertyAccessMode.Field
    private readonly List<RolePermission> _permissions = [];

    /// <summary>Gets the display name of the role.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Gets the school this role belongs to.</summary>
    public Guid SchoolId { get; private set; }

    /// <inheritdoc/>
    public bool IsDeleted { get; set; }

    /// <summary>Gets the permissions assigned to this role.</summary>
    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    // EF Core constructor
    private Role() { }

    /// <summary>Initializes a new <see cref="Role"/> for the given school.</summary>
    /// <param name="name">Display name for the role. Must not be null or whitespace.</param>
    /// <param name="schoolId">The school this role belongs to. Must not be empty.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null/whitespace or <paramref name="schoolId"/> is empty.</exception>
    public Role(string name, Guid schoolId)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.Default(schoolId, nameof(schoolId));

        Name = name.Trim();
        SchoolId = schoolId;
    }

    /// <summary>Updates the role display name.</summary>
    /// <param name="name">The new name. Must not be null or whitespace.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public void UpdateName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Name = name.Trim();
    }

    /// <inheritdoc/>
    public void Delete() => IsDeleted = true;

    /// <summary>Assigns a permission to this role. Idempotent — no-op if already assigned.</summary>
    /// <param name="permissionId">The permission to assign. Must not be empty.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="permissionId"/> is empty.</exception>
    public void AssignPermission(Guid permissionId)
    {
        Guard.Against.Default(permissionId, nameof(permissionId));

        if (_permissions.Any(p => p.PermissionId == permissionId))
        {
            return;
        }

        _permissions.Add(new RolePermission(Id, permissionId));
    }

    /// <summary>Removes a permission from this role. No-op if not assigned.</summary>
    /// <param name="permissionId">The permission to remove.</param>
    public void RemovePermission(Guid permissionId)
    {
        var existing = _permissions.FirstOrDefault(p => p.PermissionId == permissionId);

        if (existing is not null)
        {
            _permissions.Remove(existing);
        }
    }

    /// <summary>
    /// Replaces all assigned permissions with the given set.
    /// Duplicate IDs in the input are de-duplicated automatically.
    /// </summary>
    /// <param name="permissionIds">The new set of permission IDs.</param>
    public void SetPermissions(IEnumerable<Guid> permissionIds)
    {
        _permissions.Clear();

        foreach (var permissionId in permissionIds.Distinct())
        {
            _permissions.Add(new RolePermission(Id, permissionId));
        }
    }
}
