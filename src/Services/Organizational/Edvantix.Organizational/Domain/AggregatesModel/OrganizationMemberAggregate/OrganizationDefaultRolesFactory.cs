using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Фабрика стандартного набора ролей организации.
/// Создаёт 7 ролей согласно матрице прав платформы и назначает каждой доступные разрешения
/// из переданного списка. Недоступные разрешения (ещё не зарегистрированные) пропускаются.
/// </summary>
public static class OrganizationDefaultRolesFactory
{
    /// <summary>
    /// Создаёт стандартный набор ролей для организации.
    /// Первая роль в возвращаемом списке — всегда «Владелец» (<see cref="OrganizationMemberRole.IsOwner"/> = <see langword="true"/>).
    /// </summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="availablePermissions">Все доступные разрешения из базы данных.</param>
    public static IReadOnlyList<OrganizationMemberRole> CreateFor(
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
        var owner = new OrganizationMemberRole(
            organizationId,
            "Владелец",
            "Полный доступ ко всем разделам, включая удаление организации",
            isSystem: true,
            isOwner: true
        );
        owner.AssignPermissions(Resolve(AllOrgPermissions.Concat(AllGroupPermissions).ToArray()));

        // Директор: управление всеми разделами, кроме биллинга и удаления организации.
        var director = new OrganizationMemberRole(
            organizationId,
            "Директор",
            "Управление всеми разделами, кроме биллинга и удаления",
            isSystem: true
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
        var teacher = new OrganizationMemberRole(
            organizationId,
            "Преподаватель",
            "Ведение занятий и журнала своих групп",
            isSystem: true
        );
        teacher.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                GroupPermissions.View,
                GroupPermissions.Content,
                GroupPermissions.Schedule
            )
        );

        // Администратор: операционное управление участниками, ролями и группами.
        var admin = new OrganizationMemberRole(
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
        var methodist = new OrganizationMemberRole(
            organizationId,
            "Методист",
            "Курсы, программы и учебные материалы"
        );
        methodist.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                OrganizationPermissions.Groups,
                GroupPermissions.Create,
                GroupPermissions.View,
                GroupPermissions.Edit,
                GroupPermissions.Members,
                GroupPermissions.Content,
                GroupPermissions.Schedule
            )
        );

        // Куратор групп: сопровождение студентов, управление участниками групп.
        var curator = new OrganizationMemberRole(
            organizationId,
            "Куратор групп",
            "Сопровождение студентов и коммуникация с участниками"
        );
        curator.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                OrganizationPermissions.Members,
                GroupPermissions.View,
                GroupPermissions.Members
            )
        );

        // Бухгалтер: финансы, аналитика, выгрузки.
        var accountant = new OrganizationMemberRole(
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

    private static readonly string[] AllGroupPermissions =
    [
        GroupPermissions.Create,
        GroupPermissions.View,
        GroupPermissions.Edit,
        GroupPermissions.Delete,
        GroupPermissions.Members,
        GroupPermissions.Content,
        GroupPermissions.Schedule,
    ];
}
