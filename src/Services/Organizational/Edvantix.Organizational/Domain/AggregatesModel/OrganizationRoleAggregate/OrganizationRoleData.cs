using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;

[ExcludeFromCodeCoverage]
internal sealed class OrganizationRoleData : List<OrganizationRole>
{
    /// <summary>Роль «Владелец» — всегда содержит все доступные разрешения платформы.</summary>
    public OrganizationRole OwnerRole { get; }

    public OrganizationRoleData(Guid organizationId, IReadOnlyList<Permission> availablePermissions)
    {
        var byCode = availablePermissions.ToDictionary(
            p => p.FullCode,
            StringComparer.OrdinalIgnoreCase
        );

        Permission[] Resolve(params string[] codes) =>
            codes.Select(c => byCode.GetValueOrDefault(c)).OfType<Permission>().ToArray();

        // Владелец получает ВСЕ доступные разрешения, чтобы любое новое разрешение
        // автоматически попадало к нему без ручного обновления этого списка.
        var owner = new OrganizationRole(
            organizationId,
            "Владелец",
            "Полный доступ ко всем разделам, включая удаление организации",
            isSystem: true
        );
        owner.AssignPermissions(availablePermissions);

        // Директор: всё кроме биллинга и удаления, включая управление всеми справочниками.
        var director = new OrganizationRole(
            organizationId,
            "Директор",
            "Управление всеми разделами, кроме биллинга и удаления"
        );
        director.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                OrganizationPermissions.Edit,
                OrganizationPermissions.Members,
                OrganizationPermissions.Roles,
                OrganizationPermissions.Groups,
                OrganizationPermissions.Analytics,
                OrganizationPermissions.Rooms,
                LevelPermissions.View,
                LevelPermissions.Manage,
                LessonTypePermissions.View,
                LessonTypePermissions.Manage,
                SubjectPermissions.View,
                SubjectPermissions.Manage,
                GroupPermissions.Create,
                GroupPermissions.View,
                GroupPermissions.Edit,
                GroupPermissions.Delete,
                GroupPermissions.Members,
                GroupPermissions.Content,
                GroupPermissions.Schedule
            )
        );

        // Преподаватель: ведение занятий; просмотр справочников для работы с расписанием и журналом.
        var teacher = new OrganizationRole(
            organizationId,
            "Преподаватель",
            "Ведение занятий и журнала своих групп"
        );
        teacher.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                LevelPermissions.View,
                LessonTypePermissions.View,
                SubjectPermissions.View,
                GroupPermissions.View,
                GroupPermissions.Content,
                GroupPermissions.Schedule
            )
        );

        // Администратор: операционное управление участниками, ролями и группами;
        // просмотр справочников для корректного назначения участников и групп.
        var admin = new OrganizationRole(
            organizationId,
            "Администратор",
            "Операционное управление: участники, роли, группы"
        );
        admin.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                OrganizationPermissions.Edit,
                OrganizationPermissions.Members,
                OrganizationPermissions.Roles,
                OrganizationPermissions.Groups,
                OrganizationPermissions.Analytics,
                LevelPermissions.View,
                LessonTypePermissions.View,
                SubjectPermissions.View,
                GroupPermissions.Create,
                GroupPermissions.View,
                GroupPermissions.Edit,
                GroupPermissions.Delete,
                GroupPermissions.Members,
                GroupPermissions.Content,
                GroupPermissions.Schedule
            )
        );

        // Методист: формирует учебные программы — управляет предметами и типами занятий,
        // просматривает уровни для привязки к группам.
        var methodist = new OrganizationRole(
            organizationId,
            "Методист",
            "Курсы, программы и учебные материалы"
        );
        methodist.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                OrganizationPermissions.Groups,
                LevelPermissions.View,
                LessonTypePermissions.View,
                LessonTypePermissions.Manage,
                SubjectPermissions.View,
                SubjectPermissions.Manage,
                GroupPermissions.Create,
                GroupPermissions.View,
                GroupPermissions.Edit,
                GroupPermissions.Members,
                GroupPermissions.Content,
                GroupPermissions.Schedule
            )
        );

        // Куратор групп: сопровождение студентов; просматривает уровни для отслеживания прогресса.
        var curator = new OrganizationRole(
            organizationId,
            "Куратор групп",
            "Сопровождение студентов и коммуникация с участниками"
        );
        curator.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                OrganizationPermissions.Members,
                LevelPermissions.View,
                GroupPermissions.View,
                GroupPermissions.Members
            )
        );

        // Бухгалтер: финансы, аналитика, выгрузки — справочники не нужны.
        var accountant = new OrganizationRole(
            organizationId,
            "Бухгалтер",
            "Финансы, договоры, выгрузки"
        );
        accountant.AssignPermissions(
            Resolve(OrganizationPermissions.View, OrganizationPermissions.Analytics)
        );

        OwnerRole = owner;
        AddRange([owner, director, teacher, admin, methodist, curator, accountant]);
    }
}
