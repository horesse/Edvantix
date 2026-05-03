namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class OrganizationMemberRoleTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();

    private static OrganizationMemberRole CreateValidRole(
        string name = "Менеджер",
        string? description = "Управление проектами"
    ) => new(ValidOrgId, name, description);

    private static Permission CreatePermission(string code = "View") =>
        new("Organization", code, $"Отображаемое название {code}");

    [Test]
    public void GivenValidData_WhenCreatingOrganizationMemberRole_ThenShouldInitializePropertiesCorrectly()
    {
        var role = new OrganizationMemberRole(
            ValidOrgId,
            "Администратор",
            "Операционное управление"
        );

        role.OrganizationId.ShouldBe(ValidOrgId);
        role.Name.ShouldBe("Администратор");
        role.Description.ShouldBe("Операционное управление");
        role.IsSystem.ShouldBeFalse();
        role.IsOwner.ShouldBeFalse();
        role.IsDeleted.ShouldBeFalse();
        role.Permissions.ShouldBeEmpty();
    }

    [Test]
    public void GivenNullDescription_WhenCreatingOrganizationMemberRole_ThenDescriptionShouldBeNull()
    {
        var role = new OrganizationMemberRole(ValidOrgId, "Студент");

        role.Description.ShouldBeNull();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingOrganizationMemberRole_ThenShouldThrowArgumentException()
    {
        var act = () => new OrganizationMemberRole(Guid.Empty, "Администратор");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenCreatingOrganizationMemberRole_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var act = () => new OrganizationMemberRole(ValidOrgId, name!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenSystemOwnerFlags_WhenCreatingOrganizationMemberRole_ThenFlagsShouldBeSet()
    {
        var role = new OrganizationMemberRole(
            ValidOrgId,
            "Владелец",
            isSystem: true,
            isOwner: true
        );

        role.IsSystem.ShouldBeTrue();
        role.IsOwner.ShouldBeTrue();
    }

    [Test]
    public void GivenValidData_WhenUpdating_ThenShouldUpdateNameAndDescription()
    {
        var role = CreateValidRole();

        role.Update("Владелец", "Владелец организации");

        role.Name.ShouldBe("Владелец");
        role.Description.ShouldBe("Владелец организации");
    }

    [Test]
    public void GivenNullDescription_WhenUpdating_ThenDescriptionShouldBeNull()
    {
        var role = CreateValidRole();

        role.Update("Владелец", null);

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
        role.Permissions.ShouldAllBe(p => p.Code != "ORG_DELETE");
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
    public void GivenNameWithLeadingSpaces_WhenCreatingOrganizationMemberRole_ThenNameShouldBeTrimmed()
    {
        var role = new OrganizationMemberRole(ValidOrgId, "  Администратор  ", "  Описание  ");

        role.Name.ShouldBe("Администратор");
        role.Description.ShouldBe("Описание");
    }
}
