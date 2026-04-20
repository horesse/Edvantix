using Edvantix.Chassis.Security.Tenant;

namespace Edvantix.Organizational.UnitTests.Features.Roles.List;

public sealed class GetRolesQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRoleRepository> _repoMock = new();
    private readonly Mock<IMapper<OrganizationMemberRole, RoleDto>> _mapperMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetRolesQueryHandler _handler;

    public GetRolesQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenRolesExist_WhenHandling_ThenShouldReturnPagedResult()
    {
        var role = CreateRole(_organizationId);
        var dto = CreateDto(role.Id, _organizationId);
        var query = new GetRolesQuery(PageIndex: 1, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMemberRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([role]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMemberRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map(role)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.TotalItems.ShouldBe(1);
        result.PageIndex.ShouldBe(1);
        result.PageSize.ShouldBe(10);
    }

    [Test]
    public async Task GivenNoRoles_WhenHandling_ThenShouldReturnEmptyPagedResult()
    {
        var query = new GetRolesQuery();

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMemberRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMemberRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.ShouldBeEmpty();
        result.TotalItems.ShouldBe(0);
    }

    [Test]
    public async Task GivenPageIndexBelowOne_WhenHandling_ThenShouldClampToOne()
    {
        var query = new GetRolesQuery(PageIndex: -5, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMemberRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMemberRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageIndex.ShouldBe(1);
    }

    [Test]
    public async Task GivenPageSizeAbove100_WhenHandling_ThenShouldClampTo100()
    {
        var query = new GetRolesQuery(PageIndex: 1, PageSize: 999);

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMemberRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMemberRole>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageSize.ShouldBe(100);
    }

    private static OrganizationMemberRole CreateRole(Guid orgId) =>
        new(orgId, "manager", "Менеджер");

    private static RoleDto CreateDto(Guid id, Guid orgId) => new(id, orgId, "manager", "Менеджер");
}
