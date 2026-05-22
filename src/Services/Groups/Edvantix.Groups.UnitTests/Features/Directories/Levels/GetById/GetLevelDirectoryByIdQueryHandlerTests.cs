namespace Edvantix.Groups.UnitTests.Features.Directories.Levels.GetById;

public sealed class GetLevelDirectoryByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetLevelDirectoryByIdQueryHandler _handler;

    public GetLevelDirectoryByIdQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingLevel_WhenHandling_ThenReturnsDto()
    {
        var level = CreateLevel(_organizationId);
        SetupLevel(level);

        var result = await _handler.Handle(
            new GetLevelDirectoryByIdQuery(level.Id),
            CancellationToken.None
        );

        result.Id.ShouldBe(level.Id);
        result.Name.ShouldBe(level.Name);
        result.Order.ShouldBe(level.SortOrder);
    }

    [Test]
    public async Task GivenActiveLevel_WhenHandling_ThenIsArchivedFalse()
    {
        var level = CreateLevel(_organizationId);
        SetupLevel(level);

        var result = await _handler.Handle(
            new GetLevelDirectoryByIdQuery(level.Id),
            CancellationToken.None
        );

        result.IsArchived.ShouldBeFalse();
    }

    [Test]
    public async Task GivenDeactivatedLevel_WhenHandling_ThenIsArchivedTrue()
    {
        var level = CreateLevel(_organizationId);
        level.Deactivate();
        SetupLevel(level);

        var result = await _handler.Handle(
            new GetLevelDirectoryByIdQuery(level.Id),
            CancellationToken.None
        );

        result.IsArchived.ShouldBeTrue();
    }

    [Test]
    public async Task GivenLevelNotFound_WhenHandling_ThenThrowsNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Level?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetLevelDirectoryByIdQuery(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenLevelFromOtherOrg_WhenHandling_ThenThrowsNotFoundException()
    {
        var level = CreateLevel(Guid.CreateVersion7());
        SetupLevel(level);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new GetLevelDirectoryByIdQuery(level.Id), CancellationToken.None)
                .AsTask()
        );
    }

    private void SetupLevel(Level level) =>
        _repoMock
            .Setup(r => r.GetByIdAsync(level.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(level);

    private static Level CreateLevel(Guid orgId) =>
        new(orgId, LevelCode.From("A1"), "Beginner", null, LevelTone.Slate, sortOrder: 1);
}
