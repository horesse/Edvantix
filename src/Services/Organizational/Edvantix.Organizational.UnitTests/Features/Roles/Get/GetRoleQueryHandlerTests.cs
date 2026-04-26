namespace Edvantix.Organizational.UnitTests.Features.Roles.Get;

public sealed class GetRoleQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRoleRepository> _repoMock = new();
    private readonly Mock<IMapper<OrganizationMemberRole, RoleDetailDto>> _mapperMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetRoleQueryHandler _handler;

    public GetRoleQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenExistingRole_WhenQuerying_ThenShouldReturnDto()
    {
        var role = CreateRole(_organizationId);
        var dto = CreateDetailDto(role.Id, _organizationId);
        _repoMock
            .Setup(r => r.GetByIdWithPermissionsAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _mapperMock.Setup(m => m.Map(role)).Returns(dto);

        var result = await _handler.Handle(new GetRoleQuery(role.Id), CancellationToken.None);

        result.ShouldBe(dto);
    }

    [Test]
    public async Task GivenExistingRole_WhenQuerying_ThenShouldCallMapper()
    {
        var role = CreateRole(_organizationId);
        _repoMock
            .Setup(r => r.GetByIdWithPermissionsAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _mapperMock.Setup(m => m.Map(role)).Returns(CreateDetailDto(role.Id, _organizationId));

        await _handler.Handle(new GetRoleQuery(role.Id), CancellationToken.None);

        _mapperMock.Verify(m => m.Map(role), Times.Once);
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

    private static OrganizationMemberRole CreateRole(Guid orgId) =>
        new(orgId, "manager", "Менеджер");

    private static RoleDetailDto CreateDetailDto(Guid id, Guid orgId) =>
        new(id, orgId, "manager", "Менеджер", []);
}
