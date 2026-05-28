namespace Edvantix.Permissions;

public sealed class OrganizationPermissionModule : PermissionModule
{
    public override string ServiceCode => "organizational";
    public override string FeatureCode => "Organization";
    public override string FeatureName => "Организация";

    public override IReadOnlyList<PermissionEntry> GetPermissions() =>
        [
            new("View", "Просмотр организации"),
            new("Edit", "Редактирование организации"),
            new("Delete", "Удаление организации"),
            new("Members", "Приглашение участников"),
            new("Roles", "Управление ролями"),
            new("Groups", "Управление группами"),
            new("Analytics", "Просмотр аналитики"),
            new("Subscription", "Управление подпиской"),
            new("Rooms", "Управление кабинетами"),
        ];
}
