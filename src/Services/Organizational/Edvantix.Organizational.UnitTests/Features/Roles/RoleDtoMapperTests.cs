namespace Edvantix.Organizational.UnitTests.Features.Roles;

public sealed class RoleDtoMapperTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();

    private static OrganizationMemberRole CreateRole(string code = "manager") =>
        new(ValidOrgId, code, "Описание");

    [Test]
    public void GivenRole_WhenMappingToRoleDto_ThenShouldMapAllFields()
    {
        var role = CreateRole();
        var mapper = new RoleDtoMapper();

        var result = mapper.Map(role);

        result.Id.ShouldBe(role.Id);
        result.OrganizationId.ShouldBe(role.OrganizationId);
        result.Code.ShouldBe(role.Code);
        result.Description.ShouldBe(role.Description);
    }

    [Test]
    public void GivenRoleWithNullDescription_WhenMappingToRoleDto_ThenDescriptionShouldBeNull()
    {
        var role = new OrganizationMemberRole(ValidOrgId, "admin");
        var mapper = new RoleDtoMapper();

        var result = mapper.Map(role);

        result.Description.ShouldBeNull();
    }

    [Test]
    public void GivenRole_WhenMappingToRoleDetailDto_ThenShouldMapPermissions()
    {
        var role = CreateRole();
        var permission = new Permission(OrganizationPermissions.Feature, "ORG_READ");
        permission.Id = Guid.CreateVersion7();
        role.AddPermission(permission);
        var mapper = new RoleDetailDtoMapper();

        var result = mapper.Map(role);

        result.Permissions.ShouldHaveSingleItem();
        result.Permissions[0].Id.ShouldBe(permission.Id);
        result.Permissions[0].Feature.ShouldBe(OrganizationPermissions.Feature);
        result.Permissions[0].Name.ShouldBe("ORG_READ");
    }

    [Test]
    public void GivenRoleWithoutPermissions_WhenMappingToRoleDetailDto_ThenPermissionsShouldBeEmpty()
    {
        var role = CreateRole();
        var mapper = new RoleDetailDtoMapper();

        var result = mapper.Map(role);

        result.Permissions.ShouldBeEmpty();
    }
}
