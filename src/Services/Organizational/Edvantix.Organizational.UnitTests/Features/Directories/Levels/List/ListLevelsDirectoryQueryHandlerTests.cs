using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Features.Directories.Levels.List;
using Edvantix.Organizational.Grpc.Services.Groups;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Levels.List;

public sealed class ListLevelsDirectoryQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Mock<IGroupsUsageService> _usageMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly ListLevelsDirectoryQueryHandler _handler;

    public ListLevelsDirectoryQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _usageMock
            .Setup(s =>
                s.CountByLevelIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, int>());
        _handler = new(_tenantMock.Object, _repoMock.Object, _usageMock.Object);
    }

    [Test]
    public async Task GivenActiveLevels_WhenListing_ThenShouldReturnPagedResult()
    {
        var levels = new List<Level>
        {
            CreateLevel("A1", "Начальный"),
            CreateLevel("B2", "Средний"),
        };
        SetupList(levels, total: 2);

        var result = await _handler.Handle(new ListLevelsDirectoryQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(2);
        result.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenEmptyOrganization_WhenListing_ThenShouldReturnEmptyResult()
    {
        SetupList([], total: 0);

        var result = await _handler.Handle(new ListLevelsDirectoryQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(0);
        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenLevelsWithGroups_WhenListing_ThenShouldReturnUsageCount()
    {
        var level = CreateLevel("A1", "Начальный");
        SetupList([level], total: 1);
        _usageMock
            .Setup(s =>
                s.CountByLevelIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, int> { [level.Id] = 4 });

        var result = await _handler.Handle(new ListLevelsDirectoryQuery(), CancellationToken.None);

        var dto = result.Single();
        dto.Usage.ShouldHaveSingleItem();
        dto.Usage[0].Label.ShouldBe("Группы");
        dto.Usage[0].Count.ShouldBe(4);
    }

    [Test]
    public async Task GivenLevelsWithNoGroups_WhenListing_ThenShouldReturnZeroUsage()
    {
        var level = CreateLevel("C1", "Продвинутый");
        SetupList([level], total: 1);

        var result = await _handler.Handle(new ListLevelsDirectoryQuery(), CancellationToken.None);

        var dto = result.Single();
        dto.Usage.ShouldHaveSingleItem();
        dto.Usage[0].Count.ShouldBe(0);
    }

    [Test]
    public async Task GivenPageSizeBeyondLimit_WhenListing_ThenShouldClampToMaximum()
    {
        SetupList([], total: 0);

        await _handler.Handle(new ListLevelsDirectoryQuery(PageSize: 9999), CancellationToken.None);

        _repoMock.Verify(
            r => r.ListForDirectoryAsync(
                _orgId,
                It.IsAny<bool>(),
                It.IsAny<string?>(),
                It.IsAny<int>(),
                100,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    private void SetupList(IReadOnlyList<Level> items, int total) =>
        _repoMock
            .Setup(r =>
                r.ListForDirectoryAsync(
                    _orgId,
                    It.IsAny<bool>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((items, total));

    private Level CreateLevel(string code, string name) =>
        new(_orgId, LevelCode.From(code), name, null, LevelTone.Blue, 1);
}
