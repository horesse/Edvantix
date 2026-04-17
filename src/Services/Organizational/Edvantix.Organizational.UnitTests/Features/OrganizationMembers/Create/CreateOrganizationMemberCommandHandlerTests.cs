namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Create;

public sealed class CreateOrganizationMemberCommandHandlerTests
{
    private readonly Mock<IOrganizationMemberRepository> _repoMock = new();
    private readonly CreateOrganizationMemberCommandHandler _handler;

    public CreateOrganizationMemberCommandHandlerTests()
    {
        _handler = new(_repoMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldAddMemberAndReturnId()
    {
        var command = BuildCommand();

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<OrganizationMember>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBe(Guid.Empty);
        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<OrganizationMember>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldSaveChanges()
    {
        var command = BuildCommand();

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<OrganizationMember>(), It.IsAny<CancellationToken>()))
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
    public async Task GivenCommandWithEndDate_WhenHandling_ThenShouldSucceed()
    {
        var command = BuildCommand(endDate: new DateOnly(2026, 12, 31));

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<OrganizationMember>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.ShouldNotThrowAsync();
    }

    private static CreateOrganizationMemberCommand BuildCommand(DateOnly? endDate = null) =>
        new(
            OrganizationId: Guid.CreateVersion7(),
            ProfileId: Guid.CreateVersion7(),
            OrganizationMemberRoleId: Guid.CreateVersion7(),
            StartDate: new DateOnly(2025, 1, 1),
            EndDate: endDate
        );
}
