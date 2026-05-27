using Edvantix.Organizational.Features.Directories.Levels.Reorder;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Levels.Reorder;

public sealed class ReorderLevelDirectoryCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly ReorderLevelDirectoryCommandHandler _handler;

    public ReorderLevelDirectoryCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenThreeLevels_WhenReordering_ThenSortOrderShouldMatchIndexPosition()
    {
        var l1 = CreateLevel(0);
        var l2 = CreateLevel(1);
        var l3 = CreateLevel(2);
        SetupList([l1, l2, l3]);

        await _handler.Handle(
            new ReorderLevelDirectoryCommand([l3.Id, l1.Id, l2.Id]),
            CancellationToken.None
        );

        l3.SortOrder.ShouldBe((short)0);
        l1.SortOrder.ShouldBe((short)1);
        l2.SortOrder.ShouldBe((short)2);
    }

    [Test]
    public async Task GivenThreeLevels_WhenReordering_ThenShouldCallSaveTwiceForTwoPhase()
    {
        var l1 = CreateLevel(0);
        var l2 = CreateLevel(1);
        var l3 = CreateLevel(2);
        SetupList([l1, l2, l3]);

        await _handler.Handle(
            new ReorderLevelDirectoryCommand([l3.Id, l1.Id, l2.Id]),
            CancellationToken.None
        );

        // Двухфазный подход: фаза 1 (временные значения) и фаза 2 (финальные значения)
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }

    [Test]
    public async Task GivenLevelsWithConflictingSortOrder_WhenReordering_ThenShouldNotThrow()
    {
        // Все три уровня имеют одинаковый SortOrder — реальная ситуация при конфликтах.
        var l1 = CreateLevel(0);
        var l2 = CreateLevel(0);
        var l3 = CreateLevel(0);
        SetupList([l1, l2, l3]);

        await Should.NotThrowAsync(() =>
            _handler
                .Handle(new ReorderLevelDirectoryCommand([l1.Id, l2.Id, l3.Id]), CancellationToken.None)
                .AsTask()
        );
    }

    [Test]
    public async Task GivenIdFromDifferentOrganization_WhenReordering_ThenShouldBeIgnored()
    {
        var own = CreateLevel(0);
        SetupList([own]);
        var foreignId = Guid.CreateVersion7();

        await _handler.Handle(
            new ReorderLevelDirectoryCommand([foreignId, own.Id]),
            CancellationToken.None
        );

        own.SortOrder.ShouldBe((short)1);
    }

    private Level CreateLevel(short sortOrder)
    {
        var level = new Level(
            _orgId,
            LevelCode.From("A1"),
            "A1 Начальный",
            null,
            LevelTone.Slate,
            sortOrder
        );
        level.Id = Guid.CreateVersion7();
        return level;
    }

    private void SetupList(IReadOnlyCollection<Level> items) =>
        _repoMock
            .Setup(r =>
                r.ListByOrganizationAsync(
                    _orgId,
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(items);
}
