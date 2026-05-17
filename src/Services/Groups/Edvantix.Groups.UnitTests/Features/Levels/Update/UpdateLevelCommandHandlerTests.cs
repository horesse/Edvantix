namespace Edvantix.Groups.UnitTests.Features.Levels.Update;

public sealed class UpdateLevelCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly UpdateLevelCommandHandler _handler;

    public UpdateLevelCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingLevel_WhenUpdating_ThenUpdatesFieldsAndSaves()
    {
        var level = CreateLevel(_organizationId);
        var command = BuildCommand(level.Id);

        SetupLevel(level);
        SetupSave();

        await _handler.Handle(command, CancellationToken.None);

        level.Name.ShouldBe(command.Name);
        level.Description.ShouldBe(command.Description);
        level.Tone.ShouldBe(command.Tone);
        level.SortOrder.ShouldBe(command.SortOrder);
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenExistingLevel_WhenUpdating_ThenCodeRemainedUnchanged()
    {
        var level = CreateLevel(_organizationId);
        var originalCode = level.Code;

        SetupLevel(level);
        SetupSave();

        await _handler.Handle(BuildCommand(level.Id), CancellationToken.None);

        level.Code.ShouldBe(originalCode);
    }

    [Test]
    public async Task GivenLevelNotFound_WhenUpdating_ThenThrowsNotFoundException()
    {
        var id = Guid.CreateVersion7();

        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Level?)null);

        var act = async () => await _handler.Handle(BuildCommand(id), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenLevelFromOtherOrg_WhenUpdating_ThenThrowsNotFoundException()
    {
        var level = CreateLevel(Guid.CreateVersion7());

        SetupLevel(level);

        var act = async () => await _handler.Handle(BuildCommand(level.Id), CancellationToken.None);

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

    private static UpdateLevelCommand BuildCommand(Guid id) =>
        new(id, "Advanced", "Продвинутый", LevelTone.Red, SortOrder: 5);
}
