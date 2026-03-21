namespace Edvantix.Constants.Permissions;

/// <summary>
/// Permission string constants for group management.
/// Format: groups.verb-noun (kebab-case).
/// Groups live in the Organizations service but use a separate permission namespace
/// to distinguish from organization-level RBAC permissions.
/// </summary>
public static class GroupsPermissions
{
    /// <summary>Permission to create a new group within the tenant.</summary>
    public const string CreateGroup = "groups.create-group";

    /// <summary>Permission to update an existing group's properties.</summary>
    public const string UpdateGroup = "groups.update-group";

    /// <summary>Permission to delete a group from the tenant.</summary>
    public const string DeleteGroup = "groups.delete-group";

    /// <summary>Permission to add or remove students from a group.</summary>
    public const string ManageGroupMembership = "groups.manage-group-membership";

    /// <summary>Returns all Groups permission strings for seeding into the permission catalogue.</summary>
    public static IReadOnlyList<string> All =>
        [CreateGroup, UpdateGroup, DeleteGroup, ManageGroupMembership];
}
