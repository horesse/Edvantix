namespace Edvantix.Organizational.UnitTests.Features.Groups.Members.Remove;

public sealed class RemoveGroupMemberCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly RemoveGroupMemberCommandHandler _handler;

    public RemoveGroupMemberCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenActiveMember_WhenRemoving_ThenShouldSetExitedAtAndSave()
    {
        var (group, member) = CreateGroupWithMember();
        var exitDate = new DateOnly(2025, 11, 1);

        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(
            new RemoveGroupMemberCommand(group.Id, member.Id, exitDate, "Отчисление"),
            CancellationToken.None
        );

        member.ExitedAt.ShouldBe(exitDate);
        member.ExitReason.ShouldBe("Отчисление");
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenGroupNotFound_WhenRemoving_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var act = async () =>
            await _handler.Handle(
                new RemoveGroupMemberCommand(
                    id,
                    Guid.CreateVersion7(),
                    new DateOnly(2025, 11, 1),
                    null
                ),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenGroupOfDifferentOrganization_WhenRemoving_ThenShouldThrowForbiddenException()
    {
        var (group, member) = CreateGroupWithMember(orgId: Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () =>
            await _handler.Handle(
                new RemoveGroupMemberCommand(group.Id, member.Id, new DateOnly(2025, 11, 1), null),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    [Test]
    public async Task GivenMemberNotFound_WhenRemoving_ThenShouldThrowNotFoundException()
    {
        var (group, _) = CreateGroupWithMember();
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () =>
            await _handler.Handle(
                new RemoveGroupMemberCommand(
                    group.Id,
                    Guid.CreateVersion7(),
                    new DateOnly(2025, 11, 1),
                    null
                ),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenExitedAtBeforeJoinedAt_WhenRemoving_ThenShouldThrowArgumentException()
    {
        var joinedAt = new DateOnly(2025, 9, 1);
        var (group, member) = CreateGroupWithMember(joinedAt: joinedAt);
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () =>
            await _handler.Handle(
                new RemoveGroupMemberCommand(group.Id, member.Id, joinedAt.AddDays(-1), null),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<ArgumentException>();
    }

    private (Group group, GroupMember member) CreateGroupWithMember(
        Guid? orgId = null,
        DateOnly? joinedAt = null
    )
    {
        var org = orgId ?? _organizationId;
        var start = new DateOnly(2025, 9, 1);
        var group = new Group(
            org,
            GroupCode.From("B1-01"),
            "Английский B1",
            "Описание",
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupFormat.Online,
            null,
            OnlinePlatform.Zoom,
            10,
            start,
            new DateOnly(2026, 6, 30)
        );
        group.Id = Guid.CreateVersion7();

        var member = new GroupMember(
            org,
            group.Id,
            Guid.CreateVersion7(),
            GroupMemberRole.Student,
            joinedAt ?? start
        );
        group.AddMember(member);

        return (group, member);
    }
}
