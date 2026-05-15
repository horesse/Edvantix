namespace Edvantix.Organizational.UnitTests.Features.Levels.Activate;

public sealed class ActivateLevelCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly ActivateLevelCommandHandler _handler;

    public ActivateLevelCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenInactiveLevel_WhenActivating_ThenIsActiveTrue()
    {
        var level = CreateLevel(_organizationId);
        level.Deactivate();
        SetupLevel(level);
        SetupSave();

        await _handler.Handle(new ActivateLevelCommand(level.Id), CancellationToken.None);

        level.IsActive.ShouldBeTrue();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenLevelFromOtherOrg_WhenActivating_ThenThrowsNotFoundException()
    {
        var level = CreateLevel(Guid.CreateVersion7());
        SetupLevel(level);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new ActivateLevelCommand(level.Id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenLevelNotFound_WhenActivating_ThenThrowsNotFoundException()
    {
        var levelId = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(levelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Level?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new ActivateLevelCommand(levelId), CancellationToken.None).AsTask()
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
        new(orgId, LevelCode.From("A1"), "Beginner", null, LevelTone.Blue, sortOrder: 1);
}
