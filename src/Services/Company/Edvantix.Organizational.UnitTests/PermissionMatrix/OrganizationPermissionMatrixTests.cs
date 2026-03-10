using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

namespace Edvantix.Organizational.UnitTests.PermissionMatrix;

public sealed class OrganizationPermissionMatrixTests
{
    private readonly IOrganizationPermissionMatrix _matrix = new OrganizationPermissionMatrix();

    // ─── HasPermission: student ──────────────────────────────────────────────────

    [Test]
    public void GivenStudentRole_WhenCheckingMaterialView_ThenShouldHavePermission()
    {
        _matrix.HasPermission(OrganizationBaseRole.Student, Permission.MaterialView).ShouldBeTrue();
    }

    [Test]
    public void GivenStudentRole_WhenCheckingAssignmentSubmit_ThenShouldHavePermission()
    {
        _matrix
            .HasPermission(OrganizationBaseRole.Student, Permission.AssignmentSubmit)
            .ShouldBeTrue();
    }

    [Test]
    public void GivenStudentRole_WhenCheckingGradeManage_ThenShouldNotHavePermission()
    {
        _matrix.HasPermission(OrganizationBaseRole.Student, Permission.GradeManage).ShouldBeFalse();
    }

    [Test]
    public void GivenStudentRole_WhenCheckingMemberManage_ThenShouldNotHavePermission()
    {
        _matrix
            .HasPermission(OrganizationBaseRole.Student, Permission.MemberManage)
            .ShouldBeFalse();
    }

    // ─── HasPermission: teacher ──────────────────────────────────────────────────

    [Test]
    public void GivenTeacherRole_WhenCheckingGradeManage_ThenShouldHavePermission()
    {
        _matrix.HasPermission(OrganizationBaseRole.Teacher, Permission.GradeManage).ShouldBeTrue();
    }

    [Test]
    public void GivenTeacherRole_WhenCheckingStudentView_ThenShouldHavePermission()
    {
        _matrix.HasPermission(OrganizationBaseRole.Teacher, Permission.StudentView).ShouldBeTrue();
    }

    [Test]
    public void GivenTeacherRole_WhenCheckingInheritedMaterialView_ThenShouldHavePermission()
    {
        // Teacher наследует права student.
        _matrix
            .HasPermission(OrganizationBaseRole.Teacher, Permission.MaterialView)
            .ShouldBeTrue();
    }

    [Test]
    public void GivenTeacherRole_WhenCheckingGroupCreate_ThenShouldNotHavePermission()
    {
        _matrix.HasPermission(OrganizationBaseRole.Teacher, Permission.GroupCreate).ShouldBeFalse();
    }

    // ─── HasPermission: manager ──────────────────────────────────────────────────

    [Test]
    public void GivenManagerRole_WhenCheckingGroupCreate_ThenShouldHavePermission()
    {
        _matrix.HasPermission(OrganizationBaseRole.Manager, Permission.GroupCreate).ShouldBeTrue();
    }

    [Test]
    public void GivenManagerRole_WhenCheckingGroupAssignTeacher_ThenShouldHavePermission()
    {
        _matrix
            .HasPermission(OrganizationBaseRole.Manager, Permission.GroupAssignTeacher)
            .ShouldBeTrue();
    }

    [Test]
    public void GivenManagerRole_WhenCheckingInheritedGradeManage_ThenShouldHavePermission()
    {
        // Manager наследует права teacher.
        _matrix.HasPermission(OrganizationBaseRole.Manager, Permission.GradeManage).ShouldBeTrue();
    }

    [Test]
    public void GivenManagerRole_WhenCheckingMemberManage_ThenShouldNotHavePermission()
    {
        // Manager не управляет пользователями с ролью admin и выше.
        _matrix
            .HasPermission(OrganizationBaseRole.Manager, Permission.MemberManage)
            .ShouldBeFalse();
    }

    // ─── HasPermission: admin ────────────────────────────────────────────────────

    [Test]
    public void GivenAdminRole_WhenCheckingMemberManage_ThenShouldHavePermission()
    {
        _matrix.HasPermission(OrganizationBaseRole.Admin, Permission.MemberManage).ShouldBeTrue();
    }

    [Test]
    public void GivenAdminRole_WhenCheckingRoleAssign_ThenShouldHavePermission()
    {
        _matrix.HasPermission(OrganizationBaseRole.Admin, Permission.RoleAssign).ShouldBeTrue();
    }

    [Test]
    public void GivenAdminRole_WhenCheckingAnalyticsViewAll_ThenShouldHavePermission()
    {
        _matrix
            .HasPermission(OrganizationBaseRole.Admin, Permission.AnalyticsViewAll)
            .ShouldBeTrue();
    }

    [Test]
    public void GivenAdminRole_WhenCheckingOrganizationDelete_ThenShouldNotHavePermission()
    {
        // Удаление организации — только owner.
        _matrix
            .HasPermission(OrganizationBaseRole.Admin, Permission.OrganizationDelete)
            .ShouldBeFalse();
    }

    [Test]
    public void GivenAdminRole_WhenCheckingOrganizationManageCustomRoles_ThenShouldNotHavePermission()
    {
        _matrix
            .HasPermission(OrganizationBaseRole.Admin, Permission.OrganizationManageCustomRoles)
            .ShouldBeFalse();
    }

    // ─── HasPermission: owner ────────────────────────────────────────────────────

    [Test]
    public void GivenOwnerRole_WhenCheckingOrganizationDelete_ThenShouldHavePermission()
    {
        _matrix
            .HasPermission(OrganizationBaseRole.Owner, Permission.OrganizationDelete)
            .ShouldBeTrue();
    }

    [Test]
    public void GivenOwnerRole_WhenCheckingOrganizationManageCustomRoles_ThenShouldHavePermission()
    {
        _matrix
            .HasPermission(OrganizationBaseRole.Owner, Permission.OrganizationManageCustomRoles)
            .ShouldBeTrue();
    }

    [Test]
    public void GivenOwnerRole_WhenCheckingInheritedMemberManage_ThenShouldHavePermission()
    {
        // Owner наследует все права admin.
        _matrix.HasPermission(OrganizationBaseRole.Owner, Permission.MemberManage).ShouldBeTrue();
    }

    [Test]
    public void GivenOwnerRole_WhenCheckingInheritedGradeManage_ThenShouldHavePermission()
    {
        // Owner транзитивно наследует права teacher.
        _matrix.HasPermission(OrganizationBaseRole.Owner, Permission.GradeManage).ShouldBeTrue();
    }

    // ─── GetPermissions ──────────────────────────────────────────────────────────

    [Test]
    public void GivenOwnerRole_WhenGettingPermissions_ThenShouldContainAllPermissions()
    {
        var permissions = _matrix.GetPermissions(OrganizationBaseRole.Owner);

        foreach (var permission in Enum.GetValues<Permission>())
        {
            permissions.ShouldContain(permission, $"Owner должен иметь право {permission}");
        }
    }

    [Test]
    public void GivenStudentRole_WhenGettingPermissions_ThenShouldContainOnlyStudentPermissions()
    {
        var permissions = _matrix.GetPermissions(OrganizationBaseRole.Student);

        permissions.ShouldContain(Permission.MaterialView);
        permissions.ShouldContain(Permission.AssignmentSubmit);
        permissions.ShouldContain(Permission.ResultViewOwn);
        permissions.ShouldContain(Permission.GroupView);
        permissions.Count.ShouldBe(4);
    }

    // ─── CanManageRole: иерархия ─────────────────────────────────────────────────

    [Test]
    public void GivenOwnerActor_WhenManagingAdminRole_ThenShouldBeAllowed()
    {
        _matrix
            .CanManageRole(OrganizationBaseRole.Owner, OrganizationBaseRole.Admin)
            .ShouldBeTrue();
    }

    [Test]
    public void GivenAdminActor_WhenManagingManagerRole_ThenShouldBeAllowed()
    {
        _matrix
            .CanManageRole(OrganizationBaseRole.Admin, OrganizationBaseRole.Manager)
            .ShouldBeTrue();
    }

    [Test]
    public void GivenManagerActor_WhenManagingTeacherRole_ThenShouldBeAllowed()
    {
        _matrix
            .CanManageRole(OrganizationBaseRole.Manager, OrganizationBaseRole.Teacher)
            .ShouldBeTrue();
    }

    [Test]
    public void GivenManagerActor_WhenManagingStudentRole_ThenShouldBeAllowed()
    {
        _matrix
            .CanManageRole(OrganizationBaseRole.Manager, OrganizationBaseRole.Student)
            .ShouldBeTrue();
    }

    // ─── CanManageRole: явные ограничения ────────────────────────────────────────

    [Test]
    public void GivenAdminActor_WhenManagingOwnerRole_ThenShouldBeDenied()
    {
        // Admin не может удалять или понижать owner.
        _matrix
            .CanManageRole(OrganizationBaseRole.Admin, OrganizationBaseRole.Owner)
            .ShouldBeFalse();
    }

    [Test]
    public void GivenManagerActor_WhenManagingAdminRole_ThenShouldBeDenied()
    {
        // Manager не управляет пользователями с ролью admin и выше.
        _matrix
            .CanManageRole(OrganizationBaseRole.Manager, OrganizationBaseRole.Admin)
            .ShouldBeFalse();
    }

    [Test]
    public void GivenTeacherActor_WhenManagingStudentRole_ThenShouldBeAllowed()
    {
        // Teacher иерархически выше Student, поэтому CanManageRole возвращает true.
        // Фактическая возможность назначать роли защищена Permission.RoleAssign (у teacher его нет).
        _matrix
            .CanManageRole(OrganizationBaseRole.Teacher, OrganizationBaseRole.Student)
            .ShouldBeTrue();
    }

    [Test]
    public void GivenStudentActor_WhenManagingAnyRole_ThenShouldBeDenied()
    {
        foreach (var role in Enum.GetValues<OrganizationBaseRole>())
        {
            _matrix
                .CanManageRole(OrganizationBaseRole.Student, role)
                .ShouldBeFalse($"Student не должен иметь возможность управлять ролью {role}");
        }
    }

    [Test]
    public void GivenSameRole_WhenManagingItself_ThenShouldBeDenied()
    {
        // Нельзя управлять ролью одного уровня (включая самого себя).
        foreach (var role in Enum.GetValues<OrganizationBaseRole>())
        {
            _matrix
                .CanManageRole(role, role)
                .ShouldBeFalse($"Роль {role} не должна иметь возможность управлять собой");
        }
    }
}
