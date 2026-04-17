namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.List;

public sealed class GetOrganizationMembersQueryHandlerTests
{
    private readonly Mock<IOrganizationMemberRepository> _repoMock = new();
    private readonly Mock<IMapper<OrganizationMember, OrganizationMemberDto>> _mapperMock = new();
    private readonly GetOrganizationMembersQueryHandler _handler;

    public GetOrganizationMembersQueryHandlerTests()
    {
        _handler = new(_repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenMembersExist_WhenHandling_ThenShouldReturnPagedResult()
    {
        var orgId = Guid.CreateVersion7();
        var member = CreateMember(orgId);
        var dto = CreateDto(member.Id, orgId);
        var query = new GetOrganizationMembersQuery(orgId, PageIndex: 1, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([member]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map(member)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.TotalItems.ShouldBe(1);
        result.PageIndex.ShouldBe(1);
        result.PageSize.ShouldBe(10);
    }

    [Test]
    public async Task GivenNoMembers_WhenHandling_ThenShouldReturnEmptyPagedResult()
    {
        var query = new GetOrganizationMembersQuery(Guid.CreateVersion7());

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
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
        var query = new GetOrganizationMembersQuery(
            Guid.CreateVersion7(),
            PageIndex: -3,
            PageSize: 10
        );

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
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
        var query = new GetOrganizationMembersQuery(
            Guid.CreateVersion7(),
            PageIndex: 1,
            PageSize: 999
        );

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageSize.ShouldBe(100);
    }

    private static OrganizationMember CreateMember(Guid orgId) =>
        new(orgId, Guid.CreateVersion7(), Guid.CreateVersion7(), new DateOnly(2025, 1, 1));

    private static OrganizationMemberDto CreateDto(Guid id, Guid orgId) =>
        new(
            id,
            orgId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            OrganizationStatus.Active,
            new DateOnly(2025, 1, 1),
            null
        );
}
