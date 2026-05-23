using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Features.Roles.Summary;

namespace Edvantix.Organizational.UnitTests.Features.Roles.Summary;

public sealed class GetRolesSummaryQueryHandlerTests : IDisposable
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationRoleRepository> _roleRepoMock = new();
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock = new();
    private readonly IFusionCache _cache = new FusionCache(new FusionCacheOptions());
    private readonly GetRolesSummaryQueryHandler _handler;

    private readonly Guid _orgId = Guid.CreateVersion7();

    public GetRolesSummaryQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);

        _handler = new(_tenantMock.Object, _roleRepoMock.Object, _memberRepoMock.Object, _cache);
    }

    [Test]
    public async Task GivenEmptyOrganization_WhenQuerying_ThenShouldReturnZeroesAndEmptyPreview()
    {
        SetupRoleCount(0);
        SetupMemberCount(0);
        SetupRolePreview([]);

        var result = await _handler.Handle(new GetRolesSummaryQuery(), CancellationToken.None);

        result.TotalRoles.ShouldBe(0);
        result.AssignedMembersCount.ShouldBe(0);
        result.RoleNamesPreview.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenOrganizationWithSevenDefaultRoles_WhenQuerying_ThenTotalRolesShouldBeSeven()
    {
        SetupRoleCount(7);
        SetupMemberCount(0);
        SetupRolePreview([]);

        var result = await _handler.Handle(new GetRolesSummaryQuery(), CancellationToken.None);

        result.TotalRoles.ShouldBe(7);
    }

    [Test]
    public async Task GivenMembersWithRoles_WhenQuerying_ThenAssignedMembersCountShouldBeCorrect()
    {
        SetupRoleCount(3);
        SetupMemberCount(5);
        SetupRolePreview([]);

        var result = await _handler.Handle(new GetRolesSummaryQuery(), CancellationToken.None);

        result.AssignedMembersCount.ShouldBe(5);
    }

    [Test]
    public async Task GivenOneMemberWithOneRole_WhenQuerying_ThenAssignedMembersCountShouldBeOne()
    {
        SetupRoleCount(1);
        SetupMemberCount(1);
        SetupRolePreview([]);

        var result = await _handler.Handle(new GetRolesSummaryQuery(), CancellationToken.None);

        result.AssignedMembersCount.ShouldBe(1);
    }

    [Test]
    public async Task GivenSevenRoles_WhenQuerying_ThenRoleNamesPreviewShouldContainUpToFive()
    {
        var roles = CreateRoles(_orgId, ["Р1", "Р2", "Р3", "Р4", "Р5"]);
        SetupRoleCount(7);
        SetupMemberCount(0);
        SetupRolePreview(roles);

        var result = await _handler.Handle(new GetRolesSummaryQuery(), CancellationToken.None);

        result.RoleNamesPreview.Count.ShouldBeLessThanOrEqualTo(5);
        result.RoleNamesPreview.ShouldBe(["Р1", "Р2", "Р3", "Р4", "Р5"]);
    }

    [Test]
    public async Task GivenRoles_WhenQuerying_ThenRoleNamesPreviewShouldPreserveOrder()
    {
        var roles = CreateRoles(_orgId, ["Альфа", "Бета", "Гамма"]);
        SetupRoleCount(3);
        SetupMemberCount(0);
        SetupRolePreview(roles);

        var result = await _handler.Handle(new GetRolesSummaryQuery(), CancellationToken.None);

        result.RoleNamesPreview.ShouldBe(["Альфа", "Бета", "Гамма"]);
    }

    [Test]
    public async Task GivenCachedResult_WhenQueryingTwice_ThenRepositoryShouldBeCalledOnce()
    {
        SetupRoleCount(3);
        SetupMemberCount(2);
        SetupRolePreview([]);

        await _handler.Handle(new GetRolesSummaryQuery(), CancellationToken.None);
        await _handler.Handle(new GetRolesSummaryQuery(), CancellationToken.None);

        _roleRepoMock.Verify(
            r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationRole>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    public void Dispose() => _cache.Dispose();

    private void SetupRoleCount(int count) =>
        _roleRepoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(count);

    private void SetupMemberCount(int count) =>
        _memberRepoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(count);

    private void SetupRolePreview(IReadOnlyCollection<OrganizationRole> roles) =>
        _roleRepoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(roles);

    private static IReadOnlyCollection<OrganizationRole> CreateRoles(
        Guid orgId,
        IEnumerable<string> names
    ) => names.Select(name => new OrganizationRole(orgId, name)).ToList();
}
