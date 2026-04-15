namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class GroupRoleTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();

    private static GroupRole CreateValidRole(
        string code = "teacher",
        string? description = "Преподаватель группы"
    ) => new(ValidOrgId, code, description);

    private static Permission CreatePermission(string name = "GROUP_READ") =>
        new(GroupPermissions.Feature, name);

    [Test]
    public void GivenValidData_WhenCreatingGroupRole_ThenShouldInitializePropertiesCorrectly()
    {
        var role = new GroupRole(ValidOrgId, "teacher", "Преподаватель");

        role.OrganizationId.ShouldBe(ValidOrgId);
        role.Code.ShouldBe("teacher");
        role.Description.ShouldBe("Преподаватель");
        role.IsDeleted.ShouldBeFalse();
        role.Permissions.ShouldBeEmpty();
    }

    [Test]
    public void GivenNullDescription_WhenCreatingGroupRole_ThenDescriptionShouldBeNull()
    {
        var role = new GroupRole(ValidOrgId, "student");

        role.Description.ShouldBeNull();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingGroupRole_ThenShouldThrowArgumentException()
    {
        var act = () => new GroupRole(Guid.Empty, "teacher");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceCode_WhenCreatingGroupRole_ThenShouldThrowArgumentException(
        string? code
    )
    {
        var act = () => new GroupRole(ValidOrgId, code!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidData_WhenUpdating_ThenShouldUpdateCodeAndDescription()
    {
        var role = CreateValidRole();

        role.Update("manager", "Менеджер группы");

        role.Code.ShouldBe("manager");
        role.Description.ShouldBe("Менеджер группы");
    }

    [Test]
    public void GivenNullDescription_WhenUpdating_ThenDescriptionShouldBeNull()
    {
        var role = CreateValidRole();

        role.Update("student", null);

        role.Description.ShouldBeNull();
    }

    [Test]
    public void GivenNewPermission_WhenAddingPermission_ThenShouldAddToPermissions()
    {
        var role = CreateValidRole();
        var permission = CreatePermission();

        role.AddPermission(permission);

        role.Permissions.ShouldHaveSingleItem();
        role.Permissions[0].ShouldBe(permission);
    }

    [Test]
    public void GivenDuplicatePermission_WhenAddingPermission_ThenShouldNotDuplicate()
    {
        var role = CreateValidRole();
        var permission = CreatePermission();
        permission.Id = Guid.CreateVersion7();
        role.AddPermission(permission);

        role.AddPermission(permission);

        role.Permissions.ShouldHaveSingleItem();
    }

    [Test]
    public void GivenPermissions_WhenAssigningPermissions_ThenShouldReplaceCurrentPermissions()
    {
        var role = CreateValidRole();
        var oldPermission = CreatePermission("GROUP_DELETE");
        role.AddPermission(oldPermission);

        var newPermissions = new[]
        {
            CreatePermission("GROUP_READ"),
            CreatePermission("GROUP_UPDATE"),
        };
        role.AssignPermissions(newPermissions);

        role.Permissions.Count.ShouldBe(2);
        role.Permissions.ShouldAllBe(p => p.Name != "GROUP_DELETE");
    }

    [Test]
    public void GivenExistingPermission_WhenRemovingPermission_ThenShouldRemoveFromPermissions()
    {
        var role = CreateValidRole();
        var permission = CreatePermission();
        permission.Id = Guid.CreateVersion7();
        role.AddPermission(permission);

        role.RemovePermission(permission.Id);

        role.Permissions.ShouldBeEmpty();
    }

    [Test]
    public void GivenNonExistentPermissionId_WhenRemovingPermission_ThenShouldNotThrow()
    {
        var role = CreateValidRole();

        var act = () => role.RemovePermission(Guid.CreateVersion7());

        act.ShouldNotThrow();
    }

    [Test]
    public void GivenActiveRole_WhenDeleting_ThenIsDeletedShouldBeTrue()
    {
        var role = CreateValidRole();

        role.Delete();

        role.IsDeleted.ShouldBeTrue();
    }
}
