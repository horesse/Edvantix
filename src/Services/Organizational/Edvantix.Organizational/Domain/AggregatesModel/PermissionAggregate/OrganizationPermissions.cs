namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

/// <summary>Известные коды разрешений на уровне организации (Feature = <see cref="Feature"/>).</summary>
public static class OrganizationPermissions
{
    /// <summary>Значение Feature для разрешений уровня организации.</summary>
    public const string Feature = "Organization";

    public const string Create = "ORG_CREATE";
    public const string Read = "ORG_READ";
    public const string Update = "ORG_UPDATE";
    public const string Delete = "ORG_DELETE";
    public const string TransferOwnership = "ORG_TRANSFER_OWNERSHIP";
    public const string ManageMembers = "ORG_MANAGE_MEMBERS";
    public const string InviteMembers = "ORG_INVITE_MEMBERS";
    public const string ManageRoles = "ORG_MANAGE_ROLES";
    public const string ManageGroups = "ORG_MANAGE_GROUPS";
    public const string ViewAnalytics = "ORG_VIEW_ANALYTICS";
    public const string ManageSettings = "ORG_MANAGE_SETTINGS";
    public const string ManageSubscription = "ORG_MANAGE_SUBSCRIPTION";
}

/// <summary>Известные коды разрешений на уровне группы (Feature = <see cref="Feature"/>).</summary>
public static class GroupPermissions
{
    /// <summary>Значение Feature для разрешений уровня группы.</summary>
    public const string Feature = "Group";

    public const string Create = "GROUP_CREATE";
    public const string Read = "GROUP_READ";
    public const string Update = "GROUP_UPDATE";
    public const string Delete = "GROUP_DELETE";
    public const string ManageMembers = "GROUP_MANAGE_MEMBERS";
    public const string ViewMembers = "GROUP_VIEW_MEMBERS";
    public const string ManageContent = "GROUP_MANAGE_CONTENT";
    public const string ViewContent = "GROUP_VIEW_CONTENT";
    public const string ManageSchedule = "GROUP_MANAGE_SCHEDULE";
}
