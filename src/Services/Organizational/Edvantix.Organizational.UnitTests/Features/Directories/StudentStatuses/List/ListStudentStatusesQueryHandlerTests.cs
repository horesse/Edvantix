using Edvantix.Chassis.Specification;
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
        SetupList(statuses);
        SetupCount(2);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentStatus>>()))
            .Returns(statuses.Select(MapToDto).ToList());

        var result = await _handler.Handle(new ListStudentStatusesQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(2);
        result.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenEmptyOrganization_WhenListing_ThenShouldReturnEmptyResult()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentStatus>>()))
            .Returns(Array.Empty<StudentStatusListItemDto>());

        var result = await _handler.Handle(new ListStudentStatusesQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(0);
        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenIncludeArchivedFalse_WhenListing_ThenShouldCallListAndCount()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentStatus>>()))
            .Returns(Array.Empty<StudentStatusListItemDto>());

        await _handler.Handle(
            new ListStudentStatusesQuery(IncludeArchived: false),
            CancellationToken.None
        );

        _repoMock.Verify(
            r =>
                r.ListAsync(
                    It.IsAny<ISpecification<StudentStatus>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        _repoMock.Verify(
            r =>
                r.CountAsync(
                    It.IsAny<ISpecification<StudentStatus>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenSearchTerm_WhenListing_ThenShouldCallBothSpecifications()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<StudentStatus>>()))
            .Returns(Array.Empty<StudentStatusListItemDto>());

        await _handler.Handle(
            new ListStudentStatusesQuery(Search: "Актив"),
            CancellationToken.None
        );

        _repoMock.Verify(
            r =>
                r.ListAsync(
                    It.IsAny<ISpecification<StudentStatus>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        _repoMock.Verify(
            r =>
                r.CountAsync(
                    It.IsAny<ISpecification<StudentStatus>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    private void SetupList(IReadOnlyList<StudentStatus> items) =>
        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<StudentStatus>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(items);

    private void SetupCount(int count) =>
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<StudentStatus>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(count);

    private static StudentStatusListItemDto MapToDto(StudentStatus s) =>
        new(s.Id, s.Name, s.Code, s.Tone, s.IsSystem, s.IsArchived, s.Order);
}
