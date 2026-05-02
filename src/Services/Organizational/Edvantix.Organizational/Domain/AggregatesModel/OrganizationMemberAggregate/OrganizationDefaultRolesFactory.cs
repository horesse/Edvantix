using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Фабрика для создания стандартного набора ролей организации на основе матрицы прав.
/// Создаёт 5 ролей уровня организации и назначает каждой соответствующие разрешения
/// из переданного списка.
/// </summary>
public static class OrganizationDefaultRolesFactory
{
    /// <summary>
    /// Создаёт стандартный набор ролей для организации.
    /// Разрешения подбираются из <paramref name="availablePermissions"/> по полному коду (<c>Permission.FullCode</c>).
    /// Отсутствующие разрешения пропускаются без ошибки.
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

        // --- Роли уровня организации (матрица прав 5.1) ---

        var owner = new OrganizationMemberRole(organizationId, "owner", "Владелец");
        owner.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                OrganizationPermissions.Edit,
                OrganizationPermissions.Delete,
                OrganizationPermissions.Members,
                OrganizationPermissions.Roles,
                OrganizationPermissions.Groups,
                OrganizationPermissions.Analytics,
                OrganizationPermissions.Subscription
            )
        );

        var admin = new OrganizationMemberRole(organizationId, "admin", "Администратор");
        admin.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                OrganizationPermissions.Edit,
                OrganizationPermissions.Members,
                OrganizationPermissions.Roles,
                OrganizationPermissions.Groups,
                OrganizationPermissions.Analytics
            )
        );

        var manager = new OrganizationMemberRole(organizationId, "manager", "Менеджер");
        manager.AssignPermissions(
            Resolve(
                OrganizationPermissions.View,
                OrganizationPermissions.Members,
                OrganizationPermissions.Groups,
                OrganizationPermissions.Analytics
            )
        );

        var teacher = new OrganizationMemberRole(organizationId, "teacher", "Преподаватель");
        teacher.AssignPermissions(Resolve(OrganizationPermissions.View));

        var student = new OrganizationMemberRole(organizationId, "student", "Студент");
        student.AssignPermissions(Resolve(OrganizationPermissions.View));

        return [owner, admin, manager, teacher, student];
    }
}
