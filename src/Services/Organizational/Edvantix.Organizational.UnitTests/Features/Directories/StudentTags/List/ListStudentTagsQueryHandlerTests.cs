using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Features.Directories.StudentTags;
using Edvantix.Organizational.Features.Directories.StudentTags.List;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.List;

public sealed class ListStudentTagsQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IStudentTagRepository> _repoMock = new();
    private readonly Mock<IMapper<StudentTag, StudentTagListItemDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly ListStudentTagsQueryHandler _handler;

    public ListStudentTagsQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenActiveTags_WhenListing_ThenShouldReturnPagedResult()
    {
        var tags = new List<StudentTag>
        {
            new(_orgId, "VIP", "#FF5733"),
            new(_orgId, "Premium", "#0000FF"),
        };
        SetupList(tags);
        SetupCount(2);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentTag>>()))
            .Returns(tags.Select(MapToDto).ToList());

        var result = await _handler.Handle(new ListStudentTagsQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(2);
        result.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenEmptyOrganization_WhenListing_ThenShouldReturnEmptyResult()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentTag>>()))
            .Returns(Array.Empty<StudentTagListItemDto>());

        var result = await _handler.Handle(new ListStudentTagsQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(0);
        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenIncludeArchivedFalse_WhenListing_ThenShouldCallListAndCount()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentTag>>()))
            .Returns(Array.Empty<StudentTagListItemDto>());

        await _handler.Handle(new ListStudentTagsQuery(IncludeArchived: false), CancellationToken.None);

        _repoMock.Verify(
            r => r.ListAsync(It.IsAny<ISpecification<StudentTag>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _repoMock.Verify(
            r => r.CountAsync(It.IsAny<ISpecification<StudentTag>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenSearchTerm_WhenListing_ThenShouldCallBothSpecifications()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentTag>>()))
            .Returns(Array.Empty<StudentTagListItemDto>());

        await _handler.Handle(new ListStudentTagsQuery(Search: "VIP"), CancellationToken.None);

        _repoMock.Verify(
            r => r.ListAsync(It.IsAny<ISpecification<StudentTag>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private void SetupList(IReadOnlyList<StudentTag> items) =>
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<StudentTag>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(items);

    private void SetupCount(int count) =>
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<StudentTag>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(count);

    private static StudentTagListItemDto MapToDto(StudentTag t) =>
        new(t.Id, t.Name, t.Color, t.IsArchived, t.Order);
}
