using Edvantix.Organizational.Grpc.Services.Profiles;
using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Organizational.UnitTests.Features.Groups.List;

public sealed class GetGroupsQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Mock<IMapper<Group, GroupListItemDto>> _mapperMock = new();
    private readonly Mock<IProfileService> _profileServiceMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetGroupsQueryHandler _handler;

    public GetGroupsQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(
            _tenantMock.Object,
            _repoMock.Object,
            _mapperMock.Object,
            _profileServiceMock.Object
        );
    }

    [Test]
    public async Task GivenGroupsExist_WhenHandling_ThenShouldReturnPagedResult()
    {
        var group = CreateGroup();
        var dto = CreateDto(group.Id);
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 10);
        var teacherProfileId = Guid.CreateVersion7();

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<Group>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([group]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<Group>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        _repoMock
            .Setup(r =>
                r.GetTeacherProfileIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new Dictionary<Guid, Guid> { [group.TeacherMemberId] = teacherProfileId });
        _repoMock
            .Setup(r =>
                r.GetRoomsByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new Dictionary<Guid, Room>());
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);
        _profileServiceMock
            .Setup(p =>
                p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new GetProfilesResponse
                {
                    Profiles =
                    {
                        new GetProfileResponse
                        {
                            Id = teacherProfileId.ToString(),
                            FullName = "Иванов Иван Иванович",
                        },
                    },
                }
            );

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.TotalItems.ShouldBe(1);
        result[0].TeacherFullName.ShouldBe("Иванов Иван Иванович");
    }

    [Test]
    public async Task GivenNoGroups_WhenHandling_ThenShouldReturnEmptyPagedResult()
    {
        var query = new GetGroupsQuery();

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<Group>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<Group>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.ShouldBeEmpty();
        result.TotalItems.ShouldBe(0);
        _profileServiceMock.Verify(
            p => p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenPageIndexBelowOne_WhenHandling_ThenShouldClampToOne()
    {
        var query = new GetGroupsQuery(PageIndex: -5, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<Group>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<Group>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageIndex.ShouldBe(1);
    }

    private Group CreateGroup() =>
        new(
            _organizationId,
            GroupCode.From("B1-01"),
            "Английский B1",
            "Описание",
            GroupLevel.B1,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupFormat.Online,
            null,
            OnlinePlatform.Zoom,
            10,
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 6, 30)
        );

    private static GroupListItemDto CreateDto(Guid id) =>
        new(
            id,
            "B1-01",
            "Английский B1",
            GroupLevel.B1,
            GroupFormat.Online,
            GroupStatus.Recruiting,
            10,
            0,
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 6, 30),
            Guid.CreateVersion7(),
            string.Empty,
            null,
            null,
            Guid.CreateVersion7()
        );
}
