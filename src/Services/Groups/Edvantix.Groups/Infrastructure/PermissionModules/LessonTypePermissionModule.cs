using Edvantix.Permissions;

namespace Edvantix.Groups.Infrastructure.PermissionModules;

internal sealed class LessonTypePermissionModule : PermissionModule
{
    public override string ServiceCode => "groups";
    public override string FeatureCode => "LessonType";
    public override string FeatureName => "Типы занятий";

    public override IReadOnlyList<PermissionEntry> GetPermissions() =>
        [
            new("View", "Просмотр справочника типов занятий"),
            new("Manage", "Управление справочником типов занятий"),
        ];
}
