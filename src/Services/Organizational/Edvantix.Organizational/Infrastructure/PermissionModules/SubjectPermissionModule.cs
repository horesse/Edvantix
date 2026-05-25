using Edvantix.Permissions;

namespace Edvantix.Organizational.Infrastructure.PermissionModules;

internal sealed class SubjectPermissionModule : PermissionModule
{
    public override string ServiceCode => "organizational";
    public override string FeatureCode => "Subject";
    public override string FeatureName => "Предметы";

    public override IReadOnlyList<PermissionEntry> GetPermissions() =>
        [
            new("View", "Просмотр справочника предметов"),
            new("Manage", "Управление справочником предметов"),
        ];
}
