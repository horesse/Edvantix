namespace Edvantix.Organizations.Domain.AggregatesModel.RoleAggregate;

/// <summary>
/// Join entity linking a <see cref="Role"/> to a global <c>Permission</c>.
/// Owned by the Role aggregate root.
/// </summary>
public sealed class RolePermission : Entity
{
    /// <summary>Gets the role identifier.</summary>
    public Guid RoleId { get; private set; }

    /// <summary>Gets the permission identifier.</summary>
    public Guid PermissionId { get; private set; }

    // EF Core constructor
    private RolePermission() { }

    /// <summary>Initializes a new <see cref="RolePermission"/> link.</summary>
    public RolePermission(Guid roleId, Guid permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}
