namespace Edvantix.Organizations.Domain.AggregatesModel.RoleAggregate;

/// <summary>
/// Represents a named role within a school (tenant).
/// Roles are soft-deletable and tenant-scoped — both filters are combined into a single
/// HasQueryFilter in <c>OrganizationsDbContext</c> because EF Core supports only one
/// HasQueryFilter per entity type.
/// </summary>
public sealed class Role : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
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
    public Role(string name, Guid schoolId)
    {
        Name = name;
        SchoolId = schoolId;
    }

    /// <summary>Updates the role display name.</summary>
    public void UpdateName(string name) => Name = name;

    /// <inheritdoc/>
    public void Delete() => IsDeleted = true;
}
