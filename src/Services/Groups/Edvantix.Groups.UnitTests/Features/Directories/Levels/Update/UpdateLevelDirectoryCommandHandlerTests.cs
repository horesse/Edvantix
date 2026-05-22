namespace Edvantix.Groups.UnitTests.Features.Directories.Levels.Update;

public sealed class UpdateLevelDirectoryCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly UpdateLevelDirectoryCommandHandler _handler;

    public UpdateLevelDirectoryCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingLevel_WhenUpdating_ThenNameAndOrderChanged()
    {
        var level = CreateLevel(_organizationId, name: "Old Name", sortOrder: 1);
        SetupLevel(level);
        SetupSave();

        var result = await _handler.Handle(
            new UpdateLevelDirectoryCommand(level.Id, "New Name", Order: 5, Description: "desc"),
            CancellationToken.None
        );

        result.Name.ShouldBe("New Name");
        result.Order.ShouldBe((short)5);
    }

    [Test]
    public async Task GivenExistingLevel_WhenUpdating_ThenTonePreserved()
    {
        var level = CreateLevel(_organizationId, tone: LevelTone.Violet);
        SetupLevel(level);
        SetupSave();

        var result = await _handler.Handle(
            new UpdateLevelDirectoryCommand(level.Id, "Name", Order: 1, Description: null),
            CancellationToken.None
        );

        result.Tone.ShouldBe(LevelTone.Violet);
    }

    [Test]
    public async Task GivenExistingLevel_WhenUpdating_ThenSavesEntities()
    {
        var level = CreateLevel(_organizationId);
        SetupLevel(level);
        SetupSave();

        await _handler.Handle(
            new UpdateLevelDirectoryCommand(level.Id, "Name", Order: 1, Description: null),
            CancellationToken.None
        );

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenLevelNotFound_WhenUpdating_ThenThrowsNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Level?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new UpdateLevelDirectoryCommand(id, "Name", Order: 1, Description: null),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenLevelFromOtherOrg_WhenUpdating_ThenThrowsNotFoundException()
    {
        var level = CreateLevel(Guid.CreateVersion7());
        SetupLevel(level);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new UpdateLevelDirectoryCommand(level.Id, "Name", Order: 1, Description: null),
                    CancellationToken.None
                )
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

    private Level CreateLevel(
        Guid orgId,
        string name = "Beginner",
        short sortOrder = 1,
        LevelTone tone = LevelTone.Slate
    ) => new(orgId, LevelCode.From("A1"), name, null, tone, sortOrder);
}
