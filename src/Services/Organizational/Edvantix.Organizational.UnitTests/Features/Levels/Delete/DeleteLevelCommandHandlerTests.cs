namespace Edvantix.Organizational.UnitTests.Features.Levels.Delete;

public sealed class DeleteLevelCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly DeleteLevelCommandHandler _handler;

    public DeleteLevelCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingUnusedLevel_WhenDeleting_ThenSoftDeletes()
    {
        var level = CreateLevel(_organizationId);
        SetupLevel(level);
        SetupNotUsed(level.Id);
        SetupSave();

        await _handler.Handle(new DeleteLevelCommand(level.Id), CancellationToken.None);

        level.IsDeleted.ShouldBeTrue();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenUsedLevel_WhenDeleting_ThenThrowsInvalidOperationException()
    {
        var level = CreateLevel(_organizationId);
        SetupLevel(level);

        _repoMock
            .Setup(r => r.IsUsedByGroupsAsync(level.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _handler.Handle(new DeleteLevelCommand(level.Id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenLevelNotFound_WhenDeleting_ThenThrowsNotFoundException()
    {
        var levelId = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(levelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Level?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteLevelCommand(levelId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenLevelFromDifferentOrganization_WhenDeleting_ThenThrowsNotFoundException()
    {
        var level = CreateLevel(Guid.CreateVersion7());
        SetupLevel(level);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteLevelCommand(level.Id), CancellationToken.None).AsTask()
        );
    }

    private void SetupLevel(Level level) =>
        _repoMock
            .Setup(r => r.GetByIdAsync(level.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(level);

    private void SetupNotUsed(Guid levelId) =>
        _repoMock
            .Setup(r => r.IsUsedByGroupsAsync(levelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

    private void SetupSave() =>
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

    private static Level CreateLevel(Guid orgId) =>
        new(orgId, LevelCode.From("A1"), "Beginner", null, LevelTone.Blue, sortOrder: 1);
}
