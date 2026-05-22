using Edvantix.Groups.Domain.LessonTypeAggregate;
using Edvantix.Groups.Features.Directories.LessonTypes.List;

namespace Edvantix.Groups.UnitTests.Features.Directories.LessonTypes.List;

public sealed class ListLessonTypesQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILessonTypeRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly ListLessonTypesQueryHandler _handler;

    public ListLessonTypesQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenMultipleItems_WhenListing_ThenReturnsPagedResult()
    {
        var items = BuildLessonTypes(3);
        SetupRepo(items, 3);

        var result = await _handler.Handle(new ListLessonTypesQuery(), CancellationToken.None);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result.TotalItems.ShouldBe(3);
    }

    [Test]
    public async Task GivenNoItems_WhenListing_ThenReturnsEmptyPage()
    {
        SetupRepo([], 0);

        var result = await _handler.Handle(new ListLessonTypesQuery(), CancellationToken.None);

        result.Count.ShouldBe(0);
        result.TotalItems.ShouldBe(0);
    }

    [Test]
    public async Task GivenPageSize_WhenListing_ThenAppliesPagination()
    {
        SetupRepo([], 0);

        await _handler.Handle(
            new ListLessonTypesQuery(PageIndex: 2, PageSize: 10),
            CancellationToken.None
        );

        _repoMock.Verify(
            r =>
                r.ListAsync(
                    _organizationId,
                    false,
                    null,
                    10, // offset = (2-1) * 10
                    10,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenIncludeArchivedTrue_WhenListing_ThenPassesToRepository()
    {
        SetupRepo([], 0);

        await _handler.Handle(
            new ListLessonTypesQuery(IncludeArchived: true),
            CancellationToken.None
        );

        _repoMock.Verify(
            r => r.ListAsync(_organizationId, true, null, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenSearch_WhenListing_ThenPassesSearchToRepository()
    {
        SetupRepo([], 0);

        await _handler.Handle(
            new ListLessonTypesQuery(Search: "урок"),
            CancellationToken.None
        );

        _repoMock.Verify(
            r => r.ListAsync(_organizationId, false, "урок", It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private void SetupRepo(IReadOnlyList<LessonType> items, int total)
    {
        _repoMock
            .Setup(r =>
                r.ListAsync(
                    _organizationId,
                    It.IsAny<bool>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((items, total));
    }

    private static List<LessonType> BuildLessonTypes(int count)
    {
        var orgId = Guid.CreateVersion7();

        return Enumerable
            .Range(1, count)
            .Select(i => new LessonType(orgId, $"Тип {i}", $"TYPE{i}", 45, "#3B82F6", null))
            .ToList();
    }
}
