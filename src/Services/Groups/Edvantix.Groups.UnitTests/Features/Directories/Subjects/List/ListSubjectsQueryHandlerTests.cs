using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Groups.Features.Directories.Subjects;
using Edvantix.Groups.Features.Directories.Subjects.List;

namespace Edvantix.Groups.UnitTests.Features.Directories.Subjects.List;

public sealed class ListSubjectsQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ISubjectRepository> _repoMock = new();
    private readonly Mock<IMapper<Subject, SubjectListItemDto>> _mapperMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly ListSubjectsQueryHandler _handler;

    public ListSubjectsQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenSubjects_WhenListing_ThenReturnsPagedResult()
    {
        var subjects = new List<Subject>
        {
            new(_organizationId, "Математика", SubjectCode.From("MATH"), "#6366F1", null),
            new(_organizationId, "Физика", SubjectCode.From("PHYS"), "#EF4444", null),
        };

        SetupRepo(subjects, total: 2);
        _mapperMock.Setup(m => m.Map(It.IsAny<Subject>())).Returns(BuildListItem());

        var result = await _handler.Handle(new ListSubjectsQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(2);
        result.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenNoSubjects_WhenListing_ThenReturnsEmptyResult()
    {
        SetupRepo([], total: 0);

        var result = await _handler.Handle(new ListSubjectsQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(0);
        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenSearchParam_WhenListing_ThenPassesSearchToRepository()
    {
        SetupRepo([], total: 0);

        await _handler.Handle(new ListSubjectsQuery(Search: "Мате"), CancellationToken.None);

        _repoMock.Verify(
            r =>
                r.ListAsync(
                    _organizationId,
                    "Мате",
                    false,
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenPage2_WhenListing_ThenCalculatesCorrectOffset()
    {
        SetupRepo([], total: 0);

        await _handler.Handle(new ListSubjectsQuery(Page: 2, Size: 10), CancellationToken.None);

        _repoMock.Verify(
            r =>
                r.ListAsync(
                    _organizationId,
                    null,
                    false,
                    10, // offset = (2-1)*10
                    10,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    private void SetupRepo(IReadOnlyList<Subject> subjects, long total)
    {
        _repoMock
            .Setup(r =>
                r.ListAsync(
                    _organizationId,
                    It.IsAny<string?>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(subjects);

        _repoMock
            .Setup(r =>
                r.CountAsync(
                    _organizationId,
                    It.IsAny<string?>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(total);
    }

    private static SubjectListItemDto BuildListItem() =>
        new(Guid.CreateVersion7(), "Математика", "MATH", "#6366F1", 0, false);
}
