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
    public async Task GivenPageSize_WhenListing_ThenPageMetadataIsCorrect()
    {
        SetupRepo([], 0);

        var result = await _handler.Handle(
            new ListLessonTypesQuery(PageIndex: 2, PageSize: 10),
            CancellationToken.None
        );

        result.PageIndex.ShouldBe(2);
        result.PageSize.ShouldBe(10);
    }

    [Test]
    public async Task GivenMultipleItems_WhenListing_ThenEachItemIsMapped()
    {
        var items = BuildLessonTypes(2);
        SetupRepo(items, 2);

        var result = await _handler.Handle(new ListLessonTypesQuery(), CancellationToken.None);

        result.ShouldAllBe(dto => dto != null);
        result.Select(d => d.Name).ShouldBe(items.Select(lt => lt.Name));
    }

    private void SetupRepo(IReadOnlyList<LessonType> items, int total)
    {
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<LessonType>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(items);

        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<LessonType>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(total);
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
