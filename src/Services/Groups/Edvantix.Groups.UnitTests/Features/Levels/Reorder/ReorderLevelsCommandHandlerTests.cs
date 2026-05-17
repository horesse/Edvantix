namespace Edvantix.Groups.UnitTests.Features.Levels.Reorder;

public sealed class ReorderLevelsCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly ReorderLevelsCommandHandler _handler;

    public ReorderLevelsCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenValidItems_WhenReordering_ThenSortOrdersAreUpdated()
    {
        // Entity.Id is not auto-generated in tests — set explicitly to avoid duplicate-key errors
        var level1 = CreateLevel(_organizationId, code: "A1", sortOrder: 1);
        var level2 = CreateLevel(_organizationId, code: "B1", sortOrder: 2);

        SetupByIds([level1, level2]);
        SetupSave();

        var items = new List<LevelOrderItem>
        {
            new(level1.Id, SortOrder: 10),
            new(level2.Id, SortOrder: 20),
        };

        await _handler.Handle(new ReorderLevelsCommand(items), CancellationToken.None);

        level1.SortOrder.ShouldBe((short)10);
        level2.SortOrder.ShouldBe((short)20);
    }

    [Test]
    public async Task GivenValidItems_WhenReordering_ThenSavesChanges()
    {
        var level = CreateLevel(_organizationId, code: "A1", sortOrder: 1);

        SetupByIds([level]);
        SetupSave();

        await _handler.Handle(
            new ReorderLevelsCommand([new(level.Id, SortOrder: 5)]),
            CancellationToken.None
        );

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenMissingLevelId_WhenReordering_ThenThrowsNotFoundException()
    {
        var existingLevel = CreateLevel(_organizationId, code: "A1", sortOrder: 1);
        var missingId = Guid.CreateVersion7();

        // Only one level returned — the second ID is not found
        SetupByIds([existingLevel]);

        var items = new List<LevelOrderItem>
        {
            new(existingLevel.Id, SortOrder: 1),
            new(missingId, SortOrder: 2),
        };

        var act = async () =>
            await _handler.Handle(new ReorderLevelsCommand(items), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenLevelFromOtherOrg_WhenReordering_ThenThrowsNotFoundException()
    {
        var foreignLevel = CreateLevel(Guid.CreateVersion7(), code: "A1", sortOrder: 1);

        SetupByIds([foreignLevel]);

        var act = async () =>
            await _handler.Handle(
                new ReorderLevelsCommand([new(foreignLevel.Id, SortOrder: 1)]),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenDuplicateSortOrders_WhenReordering_ThenThrowsInvalidOperationException()
    {
        // Two levels with distinct IDs but the command requests the same SortOrder for both
        var level1 = CreateLevel(_organizationId, code: "A1", sortOrder: 1);
        var level2 = CreateLevel(_organizationId, code: "B1", sortOrder: 2);

        SetupByIds([level1, level2]);

        var items = new List<LevelOrderItem>
        {
            new(level1.Id, SortOrder: 5),
            new(level2.Id, SortOrder: 5), // duplicate sort order
        };

        var act = async () =>
            await _handler.Handle(new ReorderLevelsCommand(items), CancellationToken.None);

        await act.ShouldThrowAsync<InvalidOperationException>();
    }

    private void SetupByIds(IReadOnlyCollection<Level> levels) =>
        _repoMock
            .Setup(r =>
                r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(levels);

    private void SetupSave() =>
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

    /// <summary>Creates a level with a unique auto-generated Id (Entity.Id is a settable Guid).</summary>
    private static Level CreateLevel(Guid orgId, string code, short sortOrder)
    {
        var level = new Level(
            orgId,
            LevelCode.From(code),
            "Level",
            null,
            LevelTone.Slate,
            sortOrder
        );
        level.Id = Guid.CreateVersion7(); // assign unique Id, since Entity does not auto-generate one
        return level;
    }
}
