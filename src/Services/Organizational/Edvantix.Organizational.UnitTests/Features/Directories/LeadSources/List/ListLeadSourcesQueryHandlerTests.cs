namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.List;

public sealed class ListLeadSourcesQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILeadSourceRepository> _repoMock = new();
    private readonly Mock<IMapper<LeadSource, LeadSourceListItemDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly ListLeadSourcesQueryHandler _handler;

    public ListLeadSourcesQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenSources_WhenListing_ThenShouldReturnPagedResult()
    {
        var sources = new List<LeadSource>
        {
            new(_orgId, "Инстаграм", LeadChannel.Online, null),
            new(_orgId, "Флаер", LeadChannel.Offline, null),
        };
        SetupList(sources);
        SetupCount(2);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<LeadSource>>()))
            .Returns(sources.Select(MapToDto).ToList());

        var result = await _handler.Handle(new ListLeadSourcesQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(2);
        result.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenNoSources_WhenListing_ThenShouldReturnEmptyPagedResult()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<LeadSource>>()))
            .Returns(Array.Empty<LeadSourceListItemDto>());

        var result = await _handler.Handle(new ListLeadSourcesQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(0);
        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenPaginationParams_WhenListing_ThenShouldPassThemToResult()
    {
        SetupList([]);
        SetupCount(100);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<LeadSource>>()))
            .Returns(Array.Empty<LeadSourceListItemDto>());

        var result = await _handler.Handle(
            new ListLeadSourcesQuery(Page: 3, PageSize: 10),
            CancellationToken.None
        );

        result.PageIndex.ShouldBe(3);
        result.PageSize.ShouldBe(10);
        result.TotalItems.ShouldBe(100);
    }

    [Test]
    public async Task GivenSearchTerm_WhenListing_ThenShouldCallBothSpecifications()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<LeadSource>>()))
            .Returns(Array.Empty<LeadSourceListItemDto>());

        await _handler.Handle(new ListLeadSourcesQuery(Search: "Инст"), CancellationToken.None);

        _repoMock.Verify(
            r => r.ListAsync(It.IsAny<ISpecification<LeadSource>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private void SetupList(IReadOnlyList<LeadSource> items) =>
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<LeadSource>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(items);

    private void SetupCount(int count) =>
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<LeadSource>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(count);

    private static LeadSourceListItemDto MapToDto(LeadSource ls) =>
        new(ls.Id, ls.Name, ls.Channel, ls.UtmTag, ls.IsArchived, ls.Order);
}
