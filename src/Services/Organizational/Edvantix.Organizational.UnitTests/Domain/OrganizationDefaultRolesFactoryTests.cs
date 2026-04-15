namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class OrganizationDefaultRolesFactoryTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();

    /// <summary>Создаёт полный список разрешений, имитируя данные из БД.</summary>
    private static IReadOnlyList<Permission> CreateAllPermissions()
    {
        var orgPermissions = new[]
        {
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
            OrganizationPermissions.ManageSubscription,
        }.Select(name => new Permission(OrganizationPermissions.Feature, name));

        var groupPermissions = new[]
        {
            GroupPermissions.Create,
            GroupPermissions.Read,
            GroupPermissions.Update,
            GroupPermissions.Delete,
            GroupPermissions.ManageMembers,
            GroupPermissions.ViewMembers,
            GroupPermissions.ManageContent,
            GroupPermissions.ViewContent,
            GroupPermissions.ManageSchedule,
        }.Select(name => new Permission(GroupPermissions.Feature, name));

        return [.. orgPermissions, .. groupPermissions];
    }

    [Test]
    public void GivenValidData_WhenCreatingDefaultRoles_ThenShouldCreate5OrgRolesAnd4GroupRoles()
    {
        var permissions = CreateAllPermissions();

        var (orgRoles, groupRoles) = OrganizationDefaultRolesFactory.CreateFor(
            ValidOrgId,
            permissions
        );

        orgRoles.Count.ShouldBe(5);
        groupRoles.Count.ShouldBe(4);
    }

    [Test]
    public void GivenValidData_WhenCreatingDefaultRoles_ThenOrgRolesCodesShouldMatchExpected()
    {
        var permissions = CreateAllPermissions();

        var (orgRoles, _) = OrganizationDefaultRolesFactory.CreateFor(ValidOrgId, permissions);

        orgRoles
            .Select(r => r.Code)
            .ShouldBe(["owner", "admin", "manager", "teacher", "student"], ignoreOrder: true);
    }

    [Test]
    public void GivenValidData_WhenCreatingDefaultRoles_ThenGroupRolesCodesShouldMatchExpected()
    {
        var permissions = CreateAllPermissions();

        var (_, groupRoles) = OrganizationDefaultRolesFactory.CreateFor(ValidOrgId, permissions);

        groupRoles
            .Select(r => r.Code)
            .ShouldBe(["manager", "teacher", "assistant", "student"], ignoreOrder: true);
    }

    [Test]
    public void GivenAllPermissions_WhenCreatingDefaultRoles_ThenOwnerShouldHaveAllOrgPermissions()
    {
        var permissions = CreateAllPermissions();

        var (orgRoles, _) = OrganizationDefaultRolesFactory.CreateFor(ValidOrgId, permissions);

        var owner = orgRoles.Single(r => r.Code == "owner");
        owner.Permissions.Count.ShouldBe(12);
        owner
            .Permissions.Select(p => p.Name)
            .ShouldContain(OrganizationPermissions.TransferOwnership);
        owner
            .Permissions.Select(p => p.Name)
            .ShouldContain(OrganizationPermissions.ManageSubscription);
    }

    [Test]
    public void GivenAllPermissions_WhenCreatingDefaultRoles_ThenStudentShouldOnlyHaveReadPermission()
    {
        var permissions = CreateAllPermissions();

        var (orgRoles, _) = OrganizationDefaultRolesFactory.CreateFor(ValidOrgId, permissions);

        var student = orgRoles.Single(r => r.Code == "student");
        student.Permissions.ShouldHaveSingleItem();
        student.Permissions[0].Name.ShouldBe(OrganizationPermissions.Read);
    }

    [Test]
    public void GivenAllPermissions_WhenCreatingDefaultRoles_ThenGroupManagerShouldHaveAllGroupPermissions()
    {
        var permissions = CreateAllPermissions();

        var (_, groupRoles) = OrganizationDefaultRolesFactory.CreateFor(ValidOrgId, permissions);

        var groupManager = groupRoles.Single(r => r.Code == "manager");
        groupManager.Permissions.Count.ShouldBe(9);
    }

    [Test]
    public void GivenAllPermissions_WhenCreatingDefaultRoles_ThenGroupStudentShouldOnlyHaveReadAndViewContentPermissions()
    {
        var permissions = CreateAllPermissions();

        var (_, groupRoles) = OrganizationDefaultRolesFactory.CreateFor(ValidOrgId, permissions);

        var groupStudent = groupRoles.Single(r => r.Code == "student");
        groupStudent.Permissions.Count.ShouldBe(2);
        groupStudent.Permissions.Select(p => p.Name).ShouldContain(GroupPermissions.Read);
        groupStudent.Permissions.Select(p => p.Name).ShouldContain(GroupPermissions.ViewContent);
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingDefaultRoles_ThenShouldThrowArgumentException()
    {
        var permissions = CreateAllPermissions();

        Action act = () => OrganizationDefaultRolesFactory.CreateFor(Guid.Empty, permissions);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNullPermissions_WhenCreatingDefaultRoles_ThenShouldThrowArgumentNullException()
    {
        Action act = () => OrganizationDefaultRolesFactory.CreateFor(ValidOrgId, null!);

        act.ShouldThrow<ArgumentNullException>();
    }

    [Test]
    public void GivenEmptyPermissions_WhenCreatingDefaultRoles_ThenShouldCreateRolesWithNoPermissions()
    {
        var (orgRoles, groupRoles) = OrganizationDefaultRolesFactory.CreateFor(ValidOrgId, []);

        orgRoles.ShouldAllBe(r => r.Permissions.Count == 0);
        groupRoles.ShouldAllBe(r => r.Permissions.Count == 0);
    }

    [Test]
    public void GivenValidData_WhenCreatingDefaultRoles_ThenAllRolesShouldBelongToOrganization()
    {
        var permissions = CreateAllPermissions();

        var (orgRoles, groupRoles) = OrganizationDefaultRolesFactory.CreateFor(
            ValidOrgId,
            permissions
        );

        orgRoles.ShouldAllBe(r => r.OrganizationId == ValidOrgId);
        groupRoles.ShouldAllBe(r => r.OrganizationId == ValidOrgId);
    }
}
