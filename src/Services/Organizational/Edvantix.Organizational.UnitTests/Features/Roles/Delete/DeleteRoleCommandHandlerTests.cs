namespace Edvantix.Organizational.UnitTests.Features.Roles.Delete;

public sealed class DeleteRoleCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRoleRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly DeleteRoleCommandHandler _handler;

    public DeleteRoleCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingRole_WhenDeleting_ThenShouldSoftDeleteAndSave()
    {
        var role = CreateRole(_organizationId);
        var command = new DeleteRoleCommand(role.Id);

        _repoMock
            .Setup(r => r.GetByIdAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        role.IsDeleted.ShouldBeTrue();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenRoleNotFound_WhenDeleting_ThenShouldThrowNotFoundException()
    {
        var roleId = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationMemberRole?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteRoleCommand(roleId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenRoleFromDifferentOrganization_WhenDeleting_ThenShouldThrowNotFoundException()
    {
        var role = CreateRole(Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteRoleCommand(role.Id), CancellationToken.None).AsTask()
        );
    }

    private static OrganizationMemberRole CreateRole(Guid orgId) =>
        new(orgId, "manager", "Менеджер");
}
