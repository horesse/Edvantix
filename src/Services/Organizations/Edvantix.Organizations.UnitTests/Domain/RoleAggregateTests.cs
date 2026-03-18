namespace Edvantix.Organizations.UnitTests.Domain;

public sealed class RoleAggregateTests
{
    private static readonly Guid ValidSchoolId = Guid.CreateVersion7();

    [Test]
    public void GivenValidNameAndSchoolId_WhenCreatingRole_ThenRoleHasCorrectProperties()
    {
        var schoolId = Guid.CreateVersion7();

        var role = new Role("Teacher", schoolId);

        role.Name.ShouldBe("Teacher");
        role.SchoolId.ShouldBe(schoolId);
        role.IsDeleted.ShouldBeFalse();
        role.Permissions.ShouldBeEmpty();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyName_WhenCreatingRole_ThenThrowsArgumentException(string? name)
    {
        var act = () => new Role(name!, ValidSchoolId);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenExistingRole_WhenUpdatingName_ThenNameIsChanged()
    {
        var role = new Role("Teacher", ValidSchoolId);

        role.UpdateName("Senior Teacher");

        role.Name.ShouldBe("Senior Teacher");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyName_WhenUpdatingName_ThenThrowsArgumentException(string? name)
    {
        var role = new Role("Teacher", ValidSchoolId);

        var act = () => role.UpdateName(name!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenExistingRole_WhenDeleting_ThenIsDeletedIsTrue()
    {
        var role = new Role("Teacher", ValidSchoolId);

        role.Delete();

        role.IsDeleted.ShouldBeTrue();
    }

    [Test]
    public void GivenExistingRole_WhenAssigningPermission_ThenPermissionIsAdded()
    {
        var role = new Role("Teacher", ValidSchoolId);
        role.Id = Guid.CreateVersion7();
        var permissionId = Guid.CreateVersion7();

        role.AssignPermission(permissionId);

        role.Permissions.ShouldHaveSingleItem();
        role.Permissions.Single().PermissionId.ShouldBe(permissionId);
    }

    [Test]
    public void GivenDuplicatePermission_WhenAssigningPermission_ThenNoOp()
    {
        var role = new Role("Teacher", ValidSchoolId);
        role.Id = Guid.CreateVersion7();
        var permissionId = Guid.CreateVersion7();
        role.AssignPermission(permissionId);

        // Assign the same permission again
        role.AssignPermission(permissionId);

        role.Permissions.Count.ShouldBe(1);
    }

    [Test]
    public void GivenExistingPermission_WhenRemovingPermission_ThenPermissionIsRemoved()
    {
        var role = new Role("Teacher", ValidSchoolId);
        role.Id = Guid.CreateVersion7();
        var permissionId = Guid.CreateVersion7();
        role.AssignPermission(permissionId);

        role.RemovePermission(permissionId);

        role.Permissions.ShouldBeEmpty();
    }

    [Test]
    public void GivenNonExistentPermission_WhenRemovingPermission_ThenNoOp()
    {
        var role = new Role("Teacher", ValidSchoolId);
        role.Id = Guid.CreateVersion7();
        var permissionId = Guid.CreateVersion7();

        // Should not throw
        role.RemovePermission(permissionId);

        role.Permissions.ShouldBeEmpty();
    }

    [Test]
    public void GivenPermissionIds_WhenSettingPermissions_ThenPermissionsReplaced()
    {
        var role = new Role("Teacher", ValidSchoolId);
        role.Id = Guid.CreateVersion7();
        var firstPermId = Guid.CreateVersion7();
        role.AssignPermission(firstPermId);

        var newPermIds = new[] { Guid.CreateVersion7(), Guid.CreateVersion7() };
        role.SetPermissions(newPermIds);

        role.Permissions.Count.ShouldBe(2);
        role.Permissions.Select(p => p.PermissionId).ShouldBe(newPermIds, ignoreOrder: true);
    }

    [Test]
    public void GivenWhitespaceName_WhenCreatingRole_ThenNameIsTrimmedAndValid()
    {
        var role = new Role("  Admin  ", ValidSchoolId);

        role.Name.ShouldBe("Admin");
    }
}
