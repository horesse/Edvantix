namespace Edvantix.Organizational.UnitTests.Features.Roles;

public sealed class RoleDtoMapperTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();

    private static OrganizationMemberRole CreateRole(string name = "Менеджер") =>
        new(ValidOrgId, name, "Описание");

    [Test]
    public void GivenRole_WhenMappingToRoleDto_ThenShouldMapAllFields()
    {
        var role = CreateRole();
        var mapper = new RoleDtoMapper();

        var result = mapper.Map(role);

        result.Id.ShouldBe(role.Id);
        result.OrganizationId.ShouldBe(role.OrganizationId);
        result.Name.ShouldBe(role.Name);
        result.Description.ShouldBe(role.Description);
        result.IsSystem.ShouldBeFalse();
        result.IsOwner.ShouldBeFalse();
        result.PermissionsCount.ShouldBe(0);
    }

    [Test]
    public void GivenSystemOwnerRole_WhenMappingToRoleDto_ThenFlagsShouldBeSet()
    {
        var role = new OrganizationMemberRole(
            ValidOrgId,
            "Владелец",
            isSystem: true,
            isOwner: true
        );
        var mapper = new RoleDtoMapper();

        var result = mapper.Map(role);

        result.IsSystem.ShouldBeTrue();
        result.IsOwner.ShouldBeTrue();
    }

    [Test]
    public void GivenRoleWithNullDescription_WhenMappingToRoleDto_ThenDescriptionShouldBeNull()
    {
        var role = new OrganizationMemberRole(ValidOrgId, "Администратор");
        var mapper = new RoleDtoMapper();

        var result = mapper.Map(role);

        result.Description.ShouldBeNull();
    }

    [Test]
    public void GivenRole_WhenMappingToRoleDetailDto_ThenShouldMapAllFields()
    {
        var role = CreateRole();
        var mapper = new RoleDetailDtoMapper();

        var result = mapper.Map(role);

        result.Id.ShouldBe(role.Id);
        result.OrganizationId.ShouldBe(role.OrganizationId);
        result.Name.ShouldBe(role.Name);
        result.Description.ShouldBe(role.Description);
        result.IsSystem.ShouldBeFalse();
        result.IsOwner.ShouldBeFalse();
        result.Features.ShouldBeEmpty();
    }

    [Test]
    public void GivenSystemOwnerRole_WhenMappingToRoleDetailDto_ThenFlagsShouldBeSet()
    {
        var role = new OrganizationMemberRole(
            ValidOrgId,
            "Владелец",
            isSystem: true,
            isOwner: true
        );
        var mapper = new RoleDetailDtoMapper();

        var result = mapper.Map(role);

        result.IsSystem.ShouldBeTrue();
        result.IsOwner.ShouldBeTrue();
    }
}
