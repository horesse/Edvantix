namespace Edvantix.Groups.UnitTests.Features.Levels.Deactivate;

public sealed class DeactivateLevelCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly DeactivateLevelCommandHandler _handler;

    public DeactivateLevelCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenActiveLevel_WhenDeactivating_ThenIsActiveFalse()
    {
        var level = CreateLevel(_organizationId);
        SetupLevel(level);
        SetupSave();

        await _handler.Handle(new DeactivateLevelCommand(level.Id), CancellationToken.None);

        level.IsActive.ShouldBeFalse();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenLevelFromOtherOrg_WhenDeactivating_ThenThrowsNotFoundException()
    {
        var level = CreateLevel(Guid.CreateVersion7());
        SetupLevel(level);

        var act = async () =>
            await _handler.Handle(new DeactivateLevelCommand(level.Id), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenLevelNotFound_WhenDeactivating_ThenThrowsNotFoundException()
    {
        var levelId = Guid.CreateVersion7();

        _repoMock
            .Setup(r => r.GetByIdAsync(levelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Level?)null);

        var act = async () =>
            await _handler.Handle(new DeactivateLevelCommand(levelId), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
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
        new(orgId, LevelCode.From("A1"), "Beginner", null, LevelTone.Blue, sortOrder: 1);
}
