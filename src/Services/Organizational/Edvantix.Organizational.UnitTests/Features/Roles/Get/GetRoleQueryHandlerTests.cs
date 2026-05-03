namespace Edvantix.Organizational.UnitTests.Features.Roles.Get;

public sealed class GetRoleQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRoleRepository> _repoMock = new();
    private readonly Mock<IPermissionRepository> _permissionRepoMock = new();
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock = new();
    private readonly Mock<IMapper<OrganizationMemberRole, RoleDetailDto>> _mapperMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetRoleQueryHandler _handler;

    public GetRoleQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _permissionRepoMock
            .Setup(r => r.GetAllWithFeaturesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _memberRepoMock
            .Setup(r => r.CountByRoleAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _handler = new(
            _tenantMock.Object,
            _repoMock.Object,
            _permissionRepoMock.Object,
            _memberRepoMock.Object,
            _mapperMock.Object
        );
    }

    [Test]
    public async Task GivenExistingRole_WhenQuerying_ThenShouldCallMapper()
    {
        var role = CreateRole(_organizationId);
        var dto = CreateDetailDto(role.Id, _organizationId);
        _repoMock
            .Setup(r => r.GetByIdWithPermissionsAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _mapperMock.Setup(m => m.Map(role)).Returns(dto);

        await _handler.Handle(new GetRoleQuery(role.Id), CancellationToken.None);

        _mapperMock.Verify(m => m.Map(role), Times.Once);
    }

    [Test]
    public async Task GivenExistingRole_WhenQuerying_ThenShouldReturnEnrichedDto()
    {
        var role = CreateRole(_organizationId);
        var dto = CreateDetailDto(role.Id, _organizationId);
        _repoMock
            .Setup(r => r.GetByIdWithPermissionsAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _mapperMock.Setup(m => m.Map(role)).Returns(dto);

        var result = await _handler.Handle(new GetRoleQuery(role.Id), CancellationToken.None);

        result.Id.ShouldBe(dto.Id);
        result.OrganizationId.ShouldBe(dto.OrganizationId);
        result.Name.ShouldBe(dto.Name);
        result.Description.ShouldBe(dto.Description);
        result.Features.ShouldBeEmpty();
        result.TotalPermissionsCount.ShouldBe(0);
        result.MembersCount.ShouldBe(0);
    }

    [Test]
    public async Task GivenRoleNotFound_WhenQuerying_ThenShouldThrowNotFoundException()
    {
        var roleId = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdWithPermissionsAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationMemberRole?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetRoleQuery(roleId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenRoleFromDifferentOrganization_WhenQuerying_ThenShouldThrowNotFoundException()
    {
        var role = CreateRole(Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdWithPermissionsAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetRoleQuery(role.Id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenRoleWithPermissions_WhenQuerying_ThenFeaturesShouldIncludeAllWithActiveFlag()
    {
        var role = CreateRole(_organizationId);
        var orgFeature = new Feature("organizational", "Organization", "Организация");
        var activePermission = new Permission("Organization", "View", "Просмотр")
        {
            Id = Guid.CreateVersion7(),
            Feature = orgFeature,
        };
        var inactivePermission = new Permission("Organization", "Edit", "Редактирование")
        {
            Id = Guid.CreateVersion7(),
            Feature = orgFeature,
        };
        role.AddPermission(activePermission);

        _permissionRepoMock
            .Setup(r => r.GetAllWithFeaturesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([activePermission, inactivePermission]);
        _memberRepoMock
            .Setup(r => r.CountByRoleAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        _repoMock
            .Setup(r => r.GetByIdWithPermissionsAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _mapperMock.Setup(m => m.Map(role)).Returns(CreateDetailDto(role.Id, _organizationId));

        var result = await _handler.Handle(new GetRoleQuery(role.Id), CancellationToken.None);

        result.TotalPermissionsCount.ShouldBe(2);
        result.MembersCount.ShouldBe(3);
        result.Features.ShouldHaveSingleItem();

        var featureDto = result.Features[0];
        featureDto.Code.ShouldBe("Organization");
        featureDto.Name.ShouldBe("Организация");
        featureDto.Permissions.Count.ShouldBe(2);
        featureDto.Permissions.Single(p => p.Code == "View").IsActive.ShouldBeTrue();
        featureDto.Permissions.Single(p => p.Code == "Edit").IsActive.ShouldBeFalse();
    }

    private static OrganizationMemberRole CreateRole(Guid orgId) =>
        new(orgId, "Менеджер", "Управление проектами");

    private static RoleDetailDto CreateDetailDto(Guid id, Guid orgId) =>
        new(id, orgId, "Менеджер", "Управление проектами", false, false);
}
