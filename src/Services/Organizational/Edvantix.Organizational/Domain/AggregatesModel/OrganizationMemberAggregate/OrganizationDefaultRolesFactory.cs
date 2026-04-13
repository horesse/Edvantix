using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Фабрика для создания стандартного набора ролей организации на основе матрицы прав.
/// Создаёт 5 ролей уровня организации и 4 роли уровня группы, назначая каждой
/// соответствующие разрешения из переданного списка.
/// </summary>
public static class OrganizationDefaultRolesFactory
{
    /// <summary>
    /// Создаёт стандартный набор ролей для организации.
    /// Разрешения подбираются из <paramref name="availablePermissions"/> по коду (<c>Permission.Name</c>).
    /// Отсутствующие разрешения пропускаются без ошибки.
    /// </summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="availablePermissions">Все доступные разрешения из базы данных.</param>
    /// <returns>
    /// Кортеж из ролей уровня организации (<see cref="OrganizationMemberRole"/>)
    /// и ролей уровня группы (<see cref="GroupRole"/>).
    /// </returns>
    public static (
        IReadOnlyList<OrganizationMemberRole> OrgRoles,
        IReadOnlyList<GroupRole> GroupRoles
    ) CreateFor(Guid organizationId, IReadOnlyList<Permission> availablePermissions)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );

        ArgumentNullException.ThrowIfNull(availablePermissions);

        var byName = availablePermissions.ToDictionary(p => p.Name);

        Permission[] Resolve(params string[] names) =>
            names.Select(n => byName.GetValueOrDefault(n)).OfType<Permission>().ToArray();

        // --- Роли уровня организации (матрица прав 5.1) ---

        var owner = new OrganizationMemberRole(organizationId, "owner", "Владелец");
        owner.AssignPermissions(
            Resolve(
                OrganizationPermissions.Create,
                OrganizationPermissions.Read,
                OrganizationPermissions.Update,
                OrganizationPermissions.Delete,
                OrganizationPermissions.TransferOwnership,
                OrganizationPermissions.ManageMembers,
                OrganizationPermissions.InviteMembers,
                OrganizationPermissions.ManageRoles,
                OrganizationPermissions.ManageGroups,
                OrganizationPermissions.ViewAnalytics,
                OrganizationPermissions.ManageSettings,
                OrganizationPermissions.ManageSubscription
            )
        );

        var admin = new OrganizationMemberRole(organizationId, "admin", "Администратор");
        admin.AssignPermissions(
            Resolve(
                OrganizationPermissions.Read,
                OrganizationPermissions.Update,
                OrganizationPermissions.ManageMembers,
                OrganizationPermissions.InviteMembers,
                OrganizationPermissions.ManageRoles,
                OrganizationPermissions.ManageGroups,
                OrganizationPermissions.ViewAnalytics,
                OrganizationPermissions.ManageSettings
            )
        );

        var manager = new OrganizationMemberRole(organizationId, "manager", "Менеджер");
        manager.AssignPermissions(
            Resolve(
                OrganizationPermissions.Read,
                OrganizationPermissions.InviteMembers,
                OrganizationPermissions.ManageGroups,
                OrganizationPermissions.ViewAnalytics
            )
        );

        var teacher = new OrganizationMemberRole(organizationId, "teacher", "Преподаватель");
        teacher.AssignPermissions(Resolve(OrganizationPermissions.Read));

        var student = new OrganizationMemberRole(organizationId, "student", "Студент");
        student.AssignPermissions(Resolve(OrganizationPermissions.Read));

        // --- Роли уровня группы (матрица прав 5.2) ---

        var groupManager = new GroupRole(organizationId, "manager", "Менеджер группы");
        groupManager.AssignPermissions(
            Resolve(
                GroupPermissions.Create,
                GroupPermissions.Read,
                GroupPermissions.Update,
                GroupPermissions.Delete,
                GroupPermissions.ManageMembers,
                GroupPermissions.ViewMembers,
                GroupPermissions.ManageContent,
                GroupPermissions.ViewContent,
                GroupPermissions.ManageSchedule
            )
        );

        var groupTeacher = new GroupRole(organizationId, "teacher", "Преподаватель группы");
        groupTeacher.AssignPermissions(
            Resolve(
                GroupPermissions.Read,
                GroupPermissions.ViewMembers,
                GroupPermissions.ManageContent,
                GroupPermissions.ViewContent,
                GroupPermissions.ManageSchedule
            )
        );

        var groupAssistant = new GroupRole(organizationId, "assistant", "Ассистент");
        groupAssistant.AssignPermissions(
            Resolve(
                GroupPermissions.Read,
                GroupPermissions.ViewMembers,
                GroupPermissions.ViewContent
            )
        );

        var groupStudent = new GroupRole(organizationId, "student", "Студент группы");
        groupStudent.AssignPermissions(
            Resolve(GroupPermissions.Read, GroupPermissions.ViewContent)
        );

        return (
            OrgRoles: [owner, admin, manager, teacher, student],
            GroupRoles: [groupManager, groupTeacher, groupAssistant, groupStudent]
        );
    }
}
