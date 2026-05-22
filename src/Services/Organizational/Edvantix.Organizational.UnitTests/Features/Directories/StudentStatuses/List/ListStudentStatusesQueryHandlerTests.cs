using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Features.Directories.StudentStatuses;
using Edvantix.Organizational.Features.Directories.StudentStatuses.List;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentStatuses.List;

public sealed class ListStudentStatusesQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IStudentStatusRepository> _repoMock = new();
    private readonly Mock<IMapper<StudentStatus, StudentStatusListItemDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly ListStudentStatusesQueryHandler _handler;

    public ListStudentStatusesQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenActiveStatuses_WhenListing_ThenShouldReturnPagedResult()
    {
        var statuses = new List<StudentStatus>
        {
            new(_orgId, "Активный", "ACTIVE", StudentStatusTone.Active),
            new(_orgId, "В академе", "ON_LEAVE", StudentStatusTone.Warning),
        };
        var paged = new PagedResult<StudentStatus>(statuses, 1, 50, 2);
        SetupList(paged);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentStatus>>()))
            .Returns(statuses.Select(s => MapToDto(s)).ToList());

        var result = await _handler.Handle(new ListStudentStatusesQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(2);
        result.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenEmptyOrganization_WhenListing_ThenShouldReturnEmptyResult()
    {
        var paged = new PagedResult<StudentStatus>([], 1, 50, 0);
        SetupList(paged);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentStatus>>()))
            .Returns(Array.Empty<StudentStatusListItemDto>());

        var result = await _handler.Handle(new ListStudentStatusesQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(0);
        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenIncludeArchivedFalse_WhenListing_ThenShouldPassCorrectFlag()
    {
        var paged = new PagedResult<StudentStatus>([], 1, 50, 0);
        SetupList(paged);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentStatus>>()))
            .Returns(Array.Empty<StudentStatusListItemDto>());

        await _handler.Handle(
            new ListStudentStatusesQuery(IncludeArchived: false),
            CancellationToken.None
        );

        _repoMock.Verify(
            r => r.ListAsync(_orgId, false, null, 1, 50, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenSearchTerm_WhenListing_ThenShouldPassSearchToRepository()
    {
        var paged = new PagedResult<StudentStatus>([], 1, 50, 0);
        SetupList(paged);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentStatus>>()))
            .Returns(Array.Empty<StudentStatusListItemDto>());

        await _handler.Handle(
            new ListStudentStatusesQuery(Search: "Актив"),
            CancellationToken.None
        );

        _repoMock.Verify(
            r => r.ListAsync(_orgId, false, "Актив", 1, 50, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private void SetupList(PagedResult<StudentStatus> result) =>
        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<bool>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(result);

    private static StudentStatusListItemDto MapToDto(StudentStatus s) =>
        new(s.Id, s.Name, s.Code, s.Tone, s.IsSystem, s.IsArchived, s.Order);
}
