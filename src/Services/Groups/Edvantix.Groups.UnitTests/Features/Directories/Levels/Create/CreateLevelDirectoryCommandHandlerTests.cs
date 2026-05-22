namespace Edvantix.Groups.UnitTests.Features.Directories.Levels.Create;

public sealed class CreateLevelDirectoryCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly CreateLevelDirectoryCommandHandler _handler;

    public CreateLevelDirectoryCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenAddsLevelToRepository()
    {
        SetupNoCodeConflict();
        SetupPersist();

        await _handler.Handle(BuildCommand(), CancellationToken.None);

        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenSavesEntities()
    {
        SetupNoCodeConflict();
        SetupPersist();

        await _handler.Handle(BuildCommand(), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenCreatedLevelBelongsToOrganization()
    {
        SetupNoCodeConflict();
        Level? captured = null;
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()))
            .Callback<Level, CancellationToken>((l, _) => captured = l)
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(BuildCommand("Beginner", 1), CancellationToken.None);

        captured.ShouldNotBeNull();
        captured.OrganizationId.ShouldBe(_organizationId);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenReturnedDtoHasCorrectName()
    {
        SetupNoCodeConflict();
        SetupPersist();

        var result = await _handler.Handle(BuildCommand("Advanced", 3), CancellationToken.None);

        result.Name.ShouldBe("Advanced");
        result.Order.ShouldBe((short)3);
        result.IsArchived.ShouldBeFalse();
    }

    [Test]
    public async Task GivenCodeConflictOnFirstAttempt_WhenHandling_ThenRetriesAndSucceeds()
    {
        var callCount = 0;
        _repoMock
            .Setup(r =>
                r.ExistsWithCodeAsync(
                    _organizationId,
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(() =>
            {
                // First call returns conflict, subsequent return no conflict.
                return callCount++ == 0;
            });
        SetupPersist();

        var result = await _handler.Handle(BuildCommand(), CancellationToken.None);

        result.ShouldNotBeNull();
        _repoMock.Verify(
            r =>
                r.ExistsWithCodeAsync(
                    _organizationId,
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.AtLeast(2)
        );
    }

    private void SetupNoCodeConflict() =>
        _repoMock
            .Setup(r =>
                r.ExistsWithCodeAsync(
                    _organizationId,
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

    private void SetupPersist()
    {
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private static CreateLevelDirectoryCommand BuildCommand(
        string name = "Beginner",
        short order = 1
    ) => new(name, order, Description: null);
}
