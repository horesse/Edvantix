namespace Edvantix.Organizational.UnitTests.Features.Roles.Create;

public sealed class CreateRoleCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRoleRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly CreateRoleCommandHandler _handler;

    public CreateRoleCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldAddRoleAndReturnId()
    {
        var command = new CreateRoleCommand("manager", "Менеджер");

        _repoMock
            .Setup(r =>
                r.AddAsync(It.IsAny<OrganizationMemberRole>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBe(Guid.Empty);
        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<OrganizationMemberRole>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldSaveChanges()
    {
        var command = new CreateRoleCommand("viewer", null);

        _repoMock
            .Setup(r =>
                r.AddAsync(It.IsAny<OrganizationMemberRole>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenRoleShouldBelongToCurrentOrganization()
    {
        OrganizationMemberRole? capturedRole = null;
        var command = new CreateRoleCommand("admin", "Администратор");

        _repoMock
            .Setup(r =>
                r.AddAsync(It.IsAny<OrganizationMemberRole>(), It.IsAny<CancellationToken>())
            )
            .Callback<OrganizationMemberRole, CancellationToken>((role, _) => capturedRole = role)
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        capturedRole.ShouldNotBeNull();
        capturedRole.OrganizationId.ShouldBe(_organizationId);
        capturedRole.Code.ShouldBe("admin");
        capturedRole.Description.ShouldBe("Администратор");
    }
}
