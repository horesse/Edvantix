namespace Edvantix.Groups.UnitTests.Features.Directories.Levels.Restore;

public sealed class RestoreLevelDirectoryCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly RestoreLevelDirectoryCommandHandler _handler;

    public RestoreLevelDirectoryCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenArchivedLevel_WhenRestoring_ThenLevelIsActivated()
    {
        var level = CreateLevel(_organizationId);
        level.Deactivate();
        SetupLevel(level);
        SetupSave();

        await _handler.Handle(
            new RestoreLevelDirectoryCommand(level.Id),
            CancellationToken.None
        );

        level.IsActive.ShouldBeTrue();
    }

    [Test]
    public async Task GivenArchivedLevel_WhenRestoring_ThenSavesEntities()
    {
        var level = CreateLevel(_organizationId);
        level.Deactivate();
        SetupLevel(level);
        SetupSave();

        await _handler.Handle(
            new RestoreLevelDirectoryCommand(level.Id),
            CancellationToken.None
        );

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenAlreadyActiveLevel_WhenRestoringAgain_ThenIsIdempotent()
    {
        var level = CreateLevel(_organizationId);
        SetupLevel(level);
        SetupSave();

        await _handler.Handle(
            new RestoreLevelDirectoryCommand(level.Id),
            CancellationToken.None
        );

        level.IsActive.ShouldBeTrue();
    }

    [Test]
    public async Task GivenLevelNotFound_WhenRestoring_ThenThrowsNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Level?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new RestoreLevelDirectoryCommand(id), CancellationToken.None)
                .AsTask()
        );
    }

    [Test]
    public async Task GivenLevelFromOtherOrg_WhenRestoring_ThenThrowsNotFoundException()
    {
        var level = CreateLevel(Guid.CreateVersion7());
        SetupLevel(level);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new RestoreLevelDirectoryCommand(level.Id), CancellationToken.None)
                .AsTask()
        );
    }

    private void SetupLevel(Level level) =>
        _repoMock
            .Setup(r => r.GetByIdAsync(level.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(level);

    private void SetupSave() =>
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

    private static Level CreateLevel(Guid orgId) =>
        new(orgId, LevelCode.From("A1"), "Beginner", null, LevelTone.Slate, sortOrder: 1);
}
