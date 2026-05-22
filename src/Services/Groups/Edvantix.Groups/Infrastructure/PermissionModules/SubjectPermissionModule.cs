using Edvantix.Permissions;

namespace Edvantix.Groups.Infrastructure.PermissionModules;

internal sealed class SubjectPermissionModule : PermissionModule
{
    public override string ServiceCode => "groups";
    public override string FeatureCode => "Subject";
    public override string FeatureName => "Предметы";

    public override IReadOnlyList<PermissionEntry> GetPermissions() =>
        [
            new("View", "Просмотр справочника предметов"),
            new("Manage", "Управление справочником предметов"),
        ];
}
