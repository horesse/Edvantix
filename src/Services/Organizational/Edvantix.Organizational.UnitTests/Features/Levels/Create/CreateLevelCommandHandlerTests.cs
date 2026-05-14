namespace Edvantix.Organizational.UnitTests.Features.Levels.Create;

public sealed class CreateLevelCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly CreateLevelCommandHandler _handler;

    public CreateLevelCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenAddsLevel()
    {
        var command = BuildCommand();
        SetupNoDuplicate();
        SetupRepoPersist();

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenSavesChanges()
    {
        var command = BuildCommand();
        SetupNoDuplicate();
        SetupRepoPersist();

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenLevelBelongsToOrganization()
    {
        var command = BuildCommand();
        SetupNoDuplicate();
        Level? capturedLevel = null;

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()))
            .Callback<Level, CancellationToken>((l, _) => capturedLevel = l)
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        capturedLevel.ShouldNotBeNull();
        capturedLevel.OrganizationId.ShouldBe(_organizationId);
    }

    [Test]
    public async Task GivenDuplicateCodeInOrganization_WhenCreating_ThenThrowsInvalidOperationException()
    {
        var command = BuildCommand();

        _repoMock
            .Setup(r => r.ExistsWithCodeAsync(_organizationId, "A1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    private void SetupNoDuplicate() =>
        _repoMock
            .Setup(r =>
                r.ExistsWithCodeAsync(
                    _organizationId,
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

    private void SetupRepoPersist()
    {
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private static CreateLevelCommand BuildCommand() =>
        new("A1", "Beginner", null, LevelTone.Blue, SortOrder: 1);
}
