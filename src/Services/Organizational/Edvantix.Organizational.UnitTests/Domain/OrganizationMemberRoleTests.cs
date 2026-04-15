namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class OrganizationMemberRoleTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();

    private static OrganizationMemberRole CreateValidRole(
        string code = "manager",
        string? description = "Менеджер"
    ) => new(ValidOrgId, code, description);

    private static Permission CreatePermission(string name = "ORG_READ") =>
        new(OrganizationPermissions.Feature, name);

    [Test]
    public void GivenValidData_WhenCreatingOrganizationMemberRole_ThenShouldInitializePropertiesCorrectly()
    {
        var role = new OrganizationMemberRole(ValidOrgId, "admin", "Администратор");

        role.OrganizationId.ShouldBe(ValidOrgId);
        role.Code.ShouldBe("admin");
        role.Description.ShouldBe("Администратор");
        role.IsDeleted.ShouldBeFalse();
        role.Permissions.ShouldBeEmpty();
    }

    [Test]
    public void GivenNullDescription_WhenCreatingOrganizationMemberRole_ThenDescriptionShouldBeNull()
    {
        var role = new OrganizationMemberRole(ValidOrgId, "student");

        role.Description.ShouldBeNull();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingOrganizationMemberRole_ThenShouldThrowArgumentException()
    {
        var act = () => new OrganizationMemberRole(Guid.Empty, "admin");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceCode_WhenCreatingOrganizationMemberRole_ThenShouldThrowArgumentException(
        string? code
    )
    {
        var act = () => new OrganizationMemberRole(ValidOrgId, code!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidData_WhenUpdating_ThenShouldUpdateCodeAndDescription()
    {
        var role = CreateValidRole();

        role.Update("owner", "Владелец организации");

        role.Code.ShouldBe("owner");
        role.Description.ShouldBe("Владелец организации");
    }

    [Test]
    public void GivenNullDescription_WhenUpdating_ThenDescriptionShouldBeNull()
    {
        var role = CreateValidRole();

        role.Update("owner", null);

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
        var oldPermission = CreatePermission("ORG_DELETE");
        role.AddPermission(oldPermission);

        var newPermissions = new[] { CreatePermission("ORG_READ"), CreatePermission("ORG_UPDATE") };
        role.AssignPermissions(newPermissions);

        role.Permissions.Count.ShouldBe(2);
        role.Permissions.ShouldAllBe(p => p.Name != "ORG_DELETE");
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

    [Test]
    public void GivenCodeWithLeadingSpaces_WhenCreatingOrganizationMemberRole_ThenCodeShouldBeTrimmed()
    {
        var role = new OrganizationMemberRole(ValidOrgId, "  admin  ", "  Описание  ");

        role.Code.ShouldBe("admin");
        role.Description.ShouldBe("Описание");
    }
}
