namespace Edvantix.Groups.UnitTests.Features.Directories.Levels.List;

public sealed class ListLevelsDirectoryQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly ListLevelsDirectoryQueryHandler _handler;

    public ListLevelsDirectoryQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenLevels_WhenHandling_ThenReturnsMappedPagedResult()
    {
        var level1 = CreateLevel(sortOrder: 1);
        var level2 = CreateLevel(sortOrder: 2);
        SetupRepository([level1, level2], total: 2);

        var result = await _handler.Handle(new ListLevelsDirectoryQuery(), CancellationToken.None);

        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe(level1.Name);
        result[1].Name.ShouldBe(level2.Name);
    }

    [Test]
    public async Task GivenNoLevels_WhenHandling_ThenReturnsEmptyPagedResult()
    {
        SetupRepository([], total: 0);

        var result = await _handler.Handle(new ListLevelsDirectoryQuery(), CancellationToken.None);

        result.ShouldBeEmpty();
        result.TotalItems.ShouldBe(0);
    }

    [Test]
    public async Task GivenPagedQuery_WhenHandling_ThenPageMetadataIsCorrect()
    {
        SetupRepository([], total: 50, pageIndex: 2, pageSize: 10);

        var result = await _handler.Handle(
            new ListLevelsDirectoryQuery(PageIndex: 2, PageSize: 10),
            CancellationToken.None
        );

        result.PageIndex.ShouldBe(2);
        result.PageSize.ShouldBe(10);
        result.TotalItems.ShouldBe(50);
        result.TotalPages.ShouldBe(5);
    }

    [Test]
    public async Task GivenIncludeArchivedFalse_WhenHandling_ThenPassesFalseToRepository()
    {
        SetupRepository([]);

        await _handler.Handle(
            new ListLevelsDirectoryQuery(IncludeArchived: false),
            CancellationToken.None
        );

        _repoMock.Verify(
            r =>
                r.ListForDirectoryAsync(
                    _organizationId,
                    includeInactive: false,
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenIncludeArchivedTrue_WhenHandling_ThenPassesTrueToRepository()
    {
        SetupRepository([]);

        await _handler.Handle(
            new ListLevelsDirectoryQuery(IncludeArchived: true),
            CancellationToken.None
        );

        _repoMock.Verify(
            r =>
                r.ListForDirectoryAsync(
                    _organizationId,
                    includeInactive: true,
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenSearchFilter_WhenHandling_ThenPassesSearchToRepository()
    {
        SetupRepository([], search: "Beg");

        await _handler.Handle(new ListLevelsDirectoryQuery(Search: "Beg"), CancellationToken.None);

        _repoMock.Verify(
            r =>
                r.ListForDirectoryAsync(
                    _organizationId,
                    It.IsAny<bool>(),
                    "Beg",
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenArchivedLevel_WhenMappingListItem_ThenIsArchivedIsTrue()
    {
        var level = CreateLevel(sortOrder: 1);
        level.Deactivate();
        SetupRepository([level], total: 1);

        var result = await _handler.Handle(
            new ListLevelsDirectoryQuery(IncludeArchived: true),
            CancellationToken.None
        );

        result[0].IsArchived.ShouldBeTrue();
    }

    private void SetupRepository(
        IReadOnlyList<Level> levels,
        int total = 0,
        int pageIndex = 1,
        int pageSize = 20,
        string? search = null
    ) =>
        _repoMock
            .Setup(r =>
                r.ListForDirectoryAsync(
                    _organizationId,
                    It.IsAny<bool>(),
                    search,
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((levels, total == 0 ? levels.Count : total));

    private Level CreateLevel(short sortOrder = 1) =>
        new(_organizationId, LevelCode.From("A1"), "Beginner", null, LevelTone.Slate, sortOrder);
}
