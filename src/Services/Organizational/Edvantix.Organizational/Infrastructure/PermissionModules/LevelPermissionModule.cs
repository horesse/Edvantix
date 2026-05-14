using Edvantix.Chassis.Permissions;

namespace Edvantix.Organizational.Infrastructure.PermissionModules;

internal sealed class LevelPermissionModule : PermissionModule
{
    public override string ServiceCode => "organizational";
    public override string FeatureCode => "Level";
    public override string FeatureName => "Уровни";

    public override IReadOnlyList<PermissionEntry> GetPermissions() =>
        [
            new("View", "Просмотр справочника уровней"),
            new("Manage", "Управление справочником уровней"),
        ];
}
