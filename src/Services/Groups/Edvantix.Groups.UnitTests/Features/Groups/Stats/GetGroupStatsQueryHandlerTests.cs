namespace Edvantix.Groups.UnitTests.Features.Groups.Stats;

public sealed class GetGroupStatsQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetGroupStatsQueryHandler _handler;

    public GetGroupStatsQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenMixedGroups_WhenGetStats_ThenAllMetricsCorrect()
    {
        // 3 Active (members: 5, 3, 2), 1 Recruiting (members: 1), 1 Paused (members: 0),
        // 1 Finished (members: 4), 1 Archived (members: 6), capacity varies
        var rows = new List<GroupStatRow>
        {
            new(GroupStatus.Active, Capacity: 10, ActiveMemberCount: 5),
            new(GroupStatus.Active, Capacity: 8, ActiveMemberCount: 3),
            new(GroupStatus.Active, Capacity: 6, ActiveMemberCount: 2),
            new(GroupStatus.Recruiting, Capacity: 12, ActiveMemberCount: 1),
            new(GroupStatus.Paused, Capacity: 10, ActiveMemberCount: 0),
            new(GroupStatus.Finished, Capacity: 10, ActiveMemberCount: 4),
            new(GroupStatus.Archived, Capacity: 10, ActiveMemberCount: 6),
        };
        SetupProjection(rows);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.Total.ShouldBe(7);
        result.Active.ShouldBe(3);
        result.Recruiting.ShouldBe(1);
        result.Paused.ShouldBe(1);
        result.Finished.ShouldBe(1);
        result.Archived.ShouldBe(1);

        // TotalActiveStudents = sum of ActiveMemberCount for Active groups = 5+3+2
        result.TotalActiveStudents.ShouldBe(10);

        // TotalCapacity = sum of Capacity for non-Archived groups = 10+8+6+12+10+10
        result.TotalCapacity.ShouldBe(56);

        // TotalFilledSeats = sum of ActiveMemberCount for non-Archived groups = 5+3+2+1+0+4
        result.TotalFilledSeats.ShouldBe(15);

        // FillRatePercent = round(15 * 100 / 56) = round(26.78) = 27
        result.FillRatePercent.ShouldBe(27);
    }

    [Test]
    [Arguments(GroupStatus.Active)]
    [Arguments(GroupStatus.Recruiting)]
    [Arguments(GroupStatus.Paused)]
    [Arguments(GroupStatus.Finished)]
    [Arguments(GroupStatus.Archived)]
    public async Task GivenSingleGroupWithStatus_WhenGetStats_ThenOnlyThatStatusCountIsOne(
        GroupStatus status
    )
    {
        SetupProjection([new(status, Capacity: 10, ActiveMemberCount: 0)]);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.Total.ShouldBe(1);
        result.Active.ShouldBe(status == GroupStatus.Active ? 1 : 0);
        result.Recruiting.ShouldBe(status == GroupStatus.Recruiting ? 1 : 0);
        result.Paused.ShouldBe(status == GroupStatus.Paused ? 1 : 0);
        result.Finished.ShouldBe(status == GroupStatus.Finished ? 1 : 0);
        result.Archived.ShouldBe(status == GroupStatus.Archived ? 1 : 0);
    }

    [Test]
    public async Task GivenRecruitingGroupsWithMembers_WhenGetStats_ThenTotalActiveStudentsIsZero()
    {
        // Recruiting-группы с участниками не входят в TotalActiveStudents
        SetupProjection([
            new(GroupStatus.Recruiting, Capacity: 10, ActiveMemberCount: 8),
            new(GroupStatus.Paused, Capacity: 10, ActiveMemberCount: 5),
            new(GroupStatus.Finished, Capacity: 10, ActiveMemberCount: 3),
        ]);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.TotalActiveStudents.ShouldBe(0);
    }

    [Test]
    public async Task GivenActiveGroupsAmongOthers_WhenGetStats_ThenTotalActiveStudentsOnlyCountsActive()
    {
        SetupProjection([
            new(GroupStatus.Active, Capacity: 10, ActiveMemberCount: 7),
            new(GroupStatus.Recruiting, Capacity: 10, ActiveMemberCount: 9),
            new(GroupStatus.Archived, Capacity: 10, ActiveMemberCount: 10),
        ]);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.TotalActiveStudents.ShouldBe(7);
    }

    [Test]
    public async Task GivenArchivedGroupsExcluded_WhenGetStats_ThenCapacityIgnoresArchived()
    {
        SetupProjection([
            new(GroupStatus.Active, Capacity: 20, ActiveMemberCount: 10),
            new(GroupStatus.Archived, Capacity: 100, ActiveMemberCount: 80),
        ]);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        // Archived не входит в TotalCapacity и TotalFilledSeats
        result.TotalCapacity.ShouldBe(20);
        result.TotalFilledSeats.ShouldBe(10);
    }

    [Test]
    public async Task GivenAllNonArchivedStatuses_WhenGetStats_ThenFilledSeatsIncludesAll()
    {
        // Recruiting, Paused и Finished тоже входят в TotalFilledSeats
        SetupProjection([
            new(GroupStatus.Active, Capacity: 10, ActiveMemberCount: 3),
            new(GroupStatus.Recruiting, Capacity: 10, ActiveMemberCount: 2),
            new(GroupStatus.Paused, Capacity: 10, ActiveMemberCount: 1),
            new(GroupStatus.Finished, Capacity: 10, ActiveMemberCount: 4),
        ]);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.TotalCapacity.ShouldBe(40);
        result.TotalFilledSeats.ShouldBe(10);
    }

    [Test]
    public async Task GivenOneThirdFilled_WhenGetStats_ThenFillRateRoundsDown()
    {
        // 1/3 * 100 = 33.33... → 33
        SetupProjection([new(GroupStatus.Active, Capacity: 3, ActiveMemberCount: 1)]);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.FillRatePercent.ShouldBe(33);
    }

    [Test]
    public async Task GivenTwoThirdsFilled_WhenGetStats_ThenFillRateRoundsUp()
    {
        // 2/3 * 100 = 66.66... → 67
        SetupProjection([new(GroupStatus.Active, Capacity: 3, ActiveMemberCount: 2)]);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.FillRatePercent.ShouldBe(67);
    }

    [Test]
    public async Task GivenFullCapacity_WhenGetStats_ThenFillRateIsHundred()
    {
        SetupProjection([new(GroupStatus.Active, Capacity: 10, ActiveMemberCount: 10)]);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.FillRatePercent.ShouldBe(100);
    }

    [Test]
    public async Task GivenZeroCapacity_WhenGetStats_ThenFillRateIsZero()
    {
        SetupProjection([new(GroupStatus.Active, Capacity: 0, ActiveMemberCount: 0)]);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.TotalCapacity.ShouldBe(0);
        result.FillRatePercent.ShouldBe(0);
    }

    [Test]
    public async Task GivenNoGroups_WhenGetStats_ThenAllZeroes()
    {
        SetupProjection([]);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.Total.ShouldBe(0);
        result.Active.ShouldBe(0);
        result.Recruiting.ShouldBe(0);
        result.Paused.ShouldBe(0);
        result.Finished.ShouldBe(0);
        result.Archived.ShouldBe(0);
        result.TotalActiveStudents.ShouldBe(0);
        result.TotalCapacity.ShouldBe(0);
        result.TotalFilledSeats.ShouldBe(0);
        result.FillRatePercent.ShouldBe(0);
    }

    [Test]
    public async Task GivenOnlyArchivedGroups_WhenGetStats_ThenCapacityAndFilledSeatsAreZero()
    {
        SetupProjection([
            new(GroupStatus.Archived, Capacity: 20, ActiveMemberCount: 15),
            new(GroupStatus.Archived, Capacity: 10, ActiveMemberCount: 8),
        ]);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.Total.ShouldBe(2);
        result.Archived.ShouldBe(2);
        result.TotalActiveStudents.ShouldBe(0);
        result.TotalCapacity.ShouldBe(0);
        result.TotalFilledSeats.ShouldBe(0);
        result.FillRatePercent.ShouldBe(0);
    }

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldCallProjectionOnce()
    {
        SetupProjection([]);

        await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        _repoMock.Verify(
            r => r.GetStatsProjectionAsync(_organizationId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private void SetupProjection(IReadOnlyList<GroupStatRow> rows) =>
        _repoMock
            .Setup(r => r.GetStatsProjectionAsync(_organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rows);
}
