namespace Edvantix.Organizational.UnitTests.Features.Groups.Stats;

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
    public async Task GivenGroupsWithDifferentStatuses_WhenHandling_ThenShouldReturnCorrectCounts()
    {
        SetupCounts(total: 10, active: 3, recruiting: 4, paused: 1, finished: 1, archived: 1);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.Total.ShouldBe(10);
        result.Active.ShouldBe(3);
        result.Recruiting.ShouldBe(4);
        result.Paused.ShouldBe(1);
        result.Finished.ShouldBe(1);
        result.Archived.ShouldBe(1);
    }

    [Test]
    public async Task GivenNoGroups_WhenHandling_ThenShouldReturnAllZeroes()
    {
        SetupCounts(0, 0, 0, 0, 0, 0);

        var result = await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        result.Total.ShouldBe(0);
        result.Active.ShouldBe(0);
        result.Recruiting.ShouldBe(0);
    }

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldCallCountSixTimes()
    {
        SetupCounts(0, 0, 0, 0, 0, 0);

        await _handler.Handle(new GetGroupStatsQuery(), CancellationToken.None);

        _repoMock.Verify(
            r => r.CountAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>()),
            Times.Exactly(6)
        );
    }

    private void SetupCounts(
        int total,
        int active,
        int recruiting,
        int paused,
        int finished,
        int archived
    )
    {
        _repoMock
            .SetupSequence(r =>
                r.CountAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(total)
            .ReturnsAsync(active)
            .ReturnsAsync(recruiting)
            .ReturnsAsync(paused)
            .ReturnsAsync(finished)
            .ReturnsAsync(archived);
    }
}
