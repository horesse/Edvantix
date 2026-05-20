using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;

/// <summary>
/// Фабрика стандартного набора ролей организации.
/// Создаёт 7 ролей согласно матрице прав платформы и назначает каждой доступные разрешения
/// из переданного списка. Недоступные разрешения (ещё не зарегистрированные) пропускаются.
/// </summary>
public static class OrganizationDefaultRolesFactory
{
    /// <summary>
    /// Создаёт стандартный набор ролей для организации.
    /// Первая роль в возвращаемом списке — всегда «Владелец» (<see cref="OrganizationRole.IsOwner"/> = <see langword="true"/>).
    /// </summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="availablePermissions">Все доступные разрешения из базы данных.</param>
    public static IReadOnlyList<OrganizationRole> CreateFor(
        Guid organizationId,
        IReadOnlyList<Permission> availablePermissions
    )
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );

        ArgumentNullException.ThrowIfNull(availablePermissions);

        var byFullCode = availablePermissions.ToDictionary(
            p => p.FullCode,
            StringComparer.OrdinalIgnoreCase
        );

        Permission[] Resolve(params string[] fullCodes) =>
            fullCodes.Select(c => byFullCode.GetValueOrDefault(c)).OfType<Permission>().ToArray();

        // Владелец: полный доступ ко всем разделам, включая биллинг и удаление.
        // Роль защищена от изменений (IsOwner = true).
        var owner = new OrganizationRole(
            organizationId,
            "Владелец",
            "Полный доступ ко всем разделам, включая удаление организации",
            isSystem: true
        );
        owner.AssignPermissions(Resolve(AllOrgPermissions.Concat(AllGroupPermissions).ToArray()));

        // Директор: управление всеми разделами, кроме биллинга и удаления организации.
        var director = new OrganizationRole(
            organizationId,
            "Директор",
            "Управление всеми разделами, кроме биллинга и удаления"
        );
        director.AssignPermissions(
            Resolve([
                OrganizationPermissions.View,
                OrganizationPermissions.Edit,
                OrganizationPermissions.Members,
                OrganizationPermissions.Roles,
                OrganizationPermissions.Groups,
                OrganizationPermissions.Analytics,
                .. AllGroupPermissions,
            ])
        );

        // Преподаватель: ведение занятий и просмотр учебных материалов своих групп.
        var teacher = new OrganizationRole(
            organizationId,
            "Преподаватель",
            "Ведение занятий и журнала своих групп"
        );
        teacher.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                GroupView,
                GroupContent,
                GroupSchedule
            )
        );

        // Администратор: операционное управление участниками, ролями и группами.
        var admin = new OrganizationRole(
            organizationId,
            "Администратор",
            "Операционное управление: участники, роли, группы"
        );
        admin.AssignPermissions(
            Resolve([
                OrganizationPermissions.View,
                OrganizationPermissions.Edit,
                OrganizationPermissions.Members,
                OrganizationPermissions.Roles,
                OrganizationPermissions.Groups,
                OrganizationPermissions.Analytics,
                .. AllGroupPermissions,
            ])
        );

        // Методист: курсы, программы и учебные группы.
        var methodist = new OrganizationRole(
            organizationId,
            "Методист",
            "Курсы, программы и учебные материалы"
        );
        methodist.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                OrganizationPermissions.Groups,
                GroupCreate,
                GroupView,
                GroupEdit,
                GroupMembers,
                GroupContent,
                GroupSchedule
            )
        );

        // Куратор групп: сопровождение студентов, управление участниками групп.
        var curator = new OrganizationRole(
            organizationId,
            "Куратор групп",
            "Сопровождение студентов и коммуникация с участниками"
        );
        curator.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                OrganizationPermissions.Members,
                GroupView,
                GroupMembers
            )
        );

        // Бухгалтер: финансы, аналитика, выгрузки.
        var accountant = new OrganizationRole(
            organizationId,
            "Бухгалтер",
            "Финансы, договоры, выгрузки"
        );
        accountant.AssignPermissions(
            Resolve(OrganizationPermissions.View, OrganizationPermissions.Analytics)
        );

        return [owner, director, teacher, admin, methodist, curator, accountant];
    }

    private static readonly string[] AllOrgPermissions =
    [
        OrganizationPermissions.View,
        OrganizationPermissions.Edit,
        OrganizationPermissions.Delete,
        OrganizationPermissions.Members,
        OrganizationPermissions.Roles,
        OrganizationPermissions.Groups,
        OrganizationPermissions.Analytics,
        OrganizationPermissions.Subscription,
    ];

    // Коды разрешений Groups-сервиса. Resolve() возвращает пустой массив,
    // если разрешение не зарегистрировано в organizationaldb.
    private const string GroupCreate = "Group.Create";
    private const string GroupView = "Group.View";
    private const string GroupEdit = "Group.Edit";
    private const string GroupDelete = "Group.Delete";
    private const string GroupMembers = "Group.Members";
    private const string GroupContent = "Group.Content";
    private const string GroupSchedule = "Group.Schedule";

    private static readonly string[] AllGroupPermissions =
    [
        GroupCreate,
        GroupView,
        GroupEdit,
        GroupDelete,
        GroupMembers,
        GroupContent,
        GroupSchedule,
    ];
}
