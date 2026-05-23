namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Kpi;

public sealed class GetOrganizationMembersKpiQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetOrganizationMembersKpiQueryHandler _handler;

    public GetOrganizationMembersKpiQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenMembersWithDifferentStatuses_WhenHandling_ThenShouldReturnCorrectCounts()
    {
        SetupCounts(total: 10, active: 6, archived: 3, deleted: 1);

        var result = await _handler.Handle(
            new GetOrganizationMembersKpiQuery(),
            CancellationToken.None
        );

        result.Total.ShouldBe(10);
        result.Active.ShouldBe(6);
        result.Archived.ShouldBe(3);
        result.Deleted.ShouldBe(1);
    }

    [Test]
    public async Task GivenNoMembers_WhenHandling_ThenShouldReturnAllZeroes()
    {
        SetupCounts(total: 0, active: 0, archived: 0, deleted: 0);

        var result = await _handler.Handle(
            new GetOrganizationMembersKpiQuery(),
            CancellationToken.None
        );

        result.Total.ShouldBe(0);
        result.Active.ShouldBe(0);
        result.Archived.ShouldBe(0);
        result.Deleted.ShouldBe(0);
    }

    [Test]
    public async Task GivenAllMembersActive_WhenHandling_ThenShouldReturnTotalEqualsActive()
    {
        SetupCounts(total: 5, active: 5, archived: 0, deleted: 0);

        var result = await _handler.Handle(
            new GetOrganizationMembersKpiQuery(),
            CancellationToken.None
        );

        result.Total.ShouldBe(5);
        result.Active.ShouldBe(5);
        result.Archived.ShouldBe(0);
        result.Deleted.ShouldBe(0);
    }

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldCallCountFourTimes()
    {
        SetupCounts(total: 0, active: 0, archived: 0, deleted: 0);

        await _handler.Handle(new GetOrganizationMembersKpiQuery(), CancellationToken.None);

        _repoMock.Verify(
            r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Exactly(4)
        );
    }

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldReadOrganizationIdFromTenantContext()
    {
        SetupCounts(0, 0, 0, 0);

        await _handler.Handle(new GetOrganizationMembersKpiQuery(), CancellationToken.None);

        _tenantMock.Verify(t => t.OrganizationId, Times.Once);
    }

    private void SetupCounts(int total, int active, int archived, int deleted)
    {
        _repoMock
            .SetupSequence(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(total)
            .ReturnsAsync(active)
            .ReturnsAsync(archived)
            .ReturnsAsync(deleted);
    }
}
