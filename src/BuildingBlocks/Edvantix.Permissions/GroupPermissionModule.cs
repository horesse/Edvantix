namespace Edvantix.Permissions;

public sealed class GroupPermissionModule : PermissionModule
{
    public override string ServiceCode => "organizational";
    public override string FeatureCode => "Group";
    public override string FeatureName => "Группы";

    public override IReadOnlyList<PermissionEntry> GetPermissions() =>
        [
            new("Create", "Создание группы"),
            new("View", "Просмотр группы"),
            new("Edit", "Редактирование группы"),
            new("Delete", "Удаление группы"),
            new("Members", "Управление участниками группы"),
            new("Content", "Управление контентом"),
            new("Schedule", "Управление расписанием"),
        ];
}
