namespace Edvantix.Constants.Permissions;

/// <summary>
/// Permission string constants for the Organizations service.
/// Format: service.verb-noun (kebab-case).
/// </summary>
public static class OrganizationsPermissions
{
    /// <summary>Permission to create a new role within the tenant.</summary>
    public const string CreateRole = "organizations.create-role";

    /// <summary>Permission to update an existing role's name.</summary>
    public const string UpdateRole = "organizations.update-role";

    /// <summary>Permission to delete a role from the tenant.</summary>
    public const string DeleteRole = "organizations.delete-role";

    /// <summary>Permission to assign a role to a user profile.</summary>
    public const string AssignRole = "organizations.assign-role";

    /// <summary>Permission to revoke a role from a user profile.</summary>
    public const string RevokeRole = "organizations.revoke-role";

    /// <summary>Permission to assign permission strings to a role.</summary>
    public const string AssignPermissions = "organizations.assign-permissions";

    /// <summary>Returns all Organizations permission strings for seeding into the permission catalogue.</summary>
    public static IReadOnlyList<string> All =>
    [
        CreateRole,
        UpdateRole,
        DeleteRole,
        AssignRole,
        RevokeRole,
        AssignPermissions,
    ];
}
