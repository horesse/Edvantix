namespace Edvantix.Permissions;

public sealed class LessonTypePermissionModule : PermissionModule
{
    public override string ServiceCode => "organizational";
    public override string FeatureCode => "LessonType";
    public override string FeatureName => "Типы занятий";

    public override IReadOnlyList<PermissionEntry> GetPermissions() =>
        [
            new("View", "Просмотр справочника типов занятий"),
            new("Manage", "Управление справочником типов занятий"),
        ];
}
