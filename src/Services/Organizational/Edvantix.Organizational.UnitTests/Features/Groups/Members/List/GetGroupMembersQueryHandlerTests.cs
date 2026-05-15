namespace Edvantix.Organizational.UnitTests.Features.Groups.Members.List;

public sealed class GetGroupMembersQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Mock<IMapper<GroupMember, GroupMemberDto>> _mapperMock = new();
    private readonly Mock<IProfileService> _profileServiceMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetGroupMembersQueryHandler _handler;

    public GetGroupMembersQueryHandlerTests()
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
    public async Task GivenActiveMembers_WhenGettingWithoutExited_ThenShouldReturnOnlyActive()
    {
        var group = CreateGroupWithMembers(activeCount: 2, exitedCount: 1);

        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _profileServiceMock
            .Setup(p =>
                p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new GetProfilesResponse());
        _mapperMock
            .Setup(m => m.Map(It.IsAny<GroupMember>()))
            .Returns(
                (GroupMember m) =>
                    new GroupMemberDto(
                        m.Id,
                        m.ProfileId,
                        string.Empty,
                        null,
                        m.Role,
                        m.JoinedAt,
                        m.ExitedAt,
                        m.ExitReason
                    )
            );

        var result = await _handler.Handle(
            new GetGroupMembersQuery(group.Id, IncludeExited: false),
            CancellationToken.None
        );

        result.TotalItems.ShouldBe(2);
        result.All(m => m.ExitedAt is null).ShouldBeTrue();
    }

    [Test]
    public async Task GivenMixedMembers_WhenGettingWithExited_ThenShouldReturnAll()
    {
        var group = CreateGroupWithMembers(activeCount: 2, exitedCount: 1);

        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _profileServiceMock
            .Setup(p =>
                p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new GetProfilesResponse());
        _mapperMock
            .Setup(m => m.Map(It.IsAny<GroupMember>()))
            .Returns(
                (GroupMember m) =>
                    new GroupMemberDto(
                        m.Id,
                        m.ProfileId,
                        string.Empty,
                        null,
                        m.Role,
                        m.JoinedAt,
                        m.ExitedAt,
                        m.ExitReason
                    )
            );

        var result = await _handler.Handle(
            new GetGroupMembersQuery(group.Id, IncludeExited: true),
            CancellationToken.None
        );

        result.TotalItems.ShouldBe(3);
    }

    [Test]
    public async Task GivenGroupNotFound_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var act = async () =>
            await _handler.Handle(new GetGroupMembersQuery(id), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenGroupOfDifferentOrganization_WhenHandling_ThenShouldThrowForbiddenException()
    {
        var group = CreateGroup(orgId: Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () =>
            await _handler.Handle(new GetGroupMembersQuery(group.Id), CancellationToken.None);

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    private Group CreateGroupWithMembers(int activeCount, int exitedCount)
    {
        var group = CreateGroup();
        var start = new DateOnly(2025, 9, 1);

        for (var i = 0; i < activeCount; i++)
        {
            group.AddMember(
                new GroupMember(
                    _organizationId,
                    group.Id,
                    Guid.CreateVersion7(),
                    GroupMemberRole.Student,
                    start
                )
            );
        }

        for (var i = 0; i < exitedCount; i++)
        {
            var member = new GroupMember(
                _organizationId,
                group.Id,
                Guid.CreateVersion7(),
                GroupMemberRole.Student,
                start
            );
            group.AddMember(member);
            member.Exit(start.AddMonths(1));
        }

        return group;
    }

    private Group CreateGroup(Guid? orgId = null)
    {
        var group = new Group(
            orgId ?? _organizationId,
            GroupCode.From("B1-01"),
            "Английский B1",
            "Описание",
            GroupLevel.B1,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupFormat.Online,
            null,
            OnlinePlatform.Zoom,
            20,
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 6, 30)
        );
        group.Id = Guid.CreateVersion7();
        return group;
    }
}
