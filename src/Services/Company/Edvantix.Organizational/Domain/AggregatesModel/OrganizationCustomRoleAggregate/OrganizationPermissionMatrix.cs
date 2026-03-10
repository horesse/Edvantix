using System.Collections.Frozen;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

/// <summary>
/// Хардкодная матрица доступа для MVP.
/// Роли наследуют все права нижестоящих в иерархии:
/// owner → admin → manager → teacher → student.
/// </summary>
/// <remarks>
/// Архитектурно изолирована за <see cref="IOrganizationPermissionMatrix"/>,
/// что позволит заменить хардкод на конфигурируемую систему без изменения вызывающего кода.
/// </remarks>
public sealed class OrganizationPermissionMatrix : IOrganizationPermissionMatrix
{
    private static readonly FrozenDictionary<OrganizationBaseRole, FrozenSet<Permission>> Matrix =
        BuildMatrix();

    /// <inheritdoc />
    public bool HasPermission(OrganizationBaseRole role, Permission permission) =>
        Matrix.TryGetValue(role, out var permissions) && permissions.Contains(permission);

    /// <inheritdoc />
    /// <remarks>
    /// Иерархия определяется порядком значений <see cref="OrganizationBaseRole"/>:
    /// Owner(0) > Admin(1) > Manager(2) > Teacher(3) > Student(4).
    /// Актор может управлять ролью только если его индекс строго меньше (= привилегий больше).
    /// </remarks>
    public bool CanManageRole(OrganizationBaseRole actorRole, OrganizationBaseRole targetRole) =>
        (int)actorRole < (int)targetRole;

    /// <inheritdoc />
    public IReadOnlySet<Permission> GetPermissions(OrganizationBaseRole role) =>
        Matrix.GetValueOrDefault(role) ?? FrozenSet<Permission>.Empty;

    private static FrozenDictionary<OrganizationBaseRole, FrozenSet<Permission>> BuildMatrix()
    {
        // Каждый уровень расширяет набор прав предыдущего (кумулятивное наследование).
        static FrozenSet<Permission> Extend(
            FrozenSet<Permission> parent,
            params Permission[] additional
        ) => parent.Union(additional).ToFrozenSet();

        var student = new[]
        {
            Permission.GroupView,
            Permission.MaterialView,
            Permission.AssignmentSubmit,
            Permission.ResultViewOwn,
        }.ToFrozenSet();

        var teacher = Extend(
            student,
            Permission.StudentView,
            Permission.GradeManage
        );

        var manager = Extend(
            teacher,
            Permission.MemberView,
            Permission.GroupCreate,
            Permission.GroupUpdate,
            Permission.GroupDelete,
            Permission.GroupAssignTeacher,
            Permission.AnalyticsViewBasic
        );

        var admin = Extend(
            manager,
            Permission.MemberManage,
            Permission.RoleAssign,
            Permission.AnalyticsViewAll
        );

        var owner = Extend(
            admin,
            Permission.OrganizationUpdate,
            Permission.OrganizationDelete,
            Permission.OrganizationManageCustomRoles
        );

        return new Dictionary<OrganizationBaseRole, FrozenSet<Permission>>
        {
            [OrganizationBaseRole.Student] = student,
            [OrganizationBaseRole.Teacher] = teacher,
            [OrganizationBaseRole.Manager] = manager,
            [OrganizationBaseRole.Admin] = admin,
            [OrganizationBaseRole.Owner] = owner,
        }.ToFrozenDictionary();
    }
}
