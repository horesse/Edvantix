namespace Edvantix.Organizational.UnitTests.Features.Groups.Members.Add;

public sealed class AddGroupMemberCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _groupRepoMock = new();
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly AddGroupMemberCommandHandler _handler;

    public AddGroupMemberCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _groupRepoMock.Object, _memberRepoMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldAddMemberAndReturnId()
    {
        var group = CreateGroup();
        var profileId = Guid.CreateVersion7();
        var command = new AddGroupMemberCommand(
            group.Id,
            profileId,
            GroupMemberRole.Student,
            new DateOnly(2025, 9, 1)
        );

        _groupRepoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _groupRepoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _memberRepoMock
            .Setup(r =>
                r.GetActiveMemberRoleIdAsync(
                    _organizationId,
                    profileId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Guid.CreateVersion7());

        await _handler.Handle(command, CancellationToken.None);

        group.Members.ShouldHaveSingleItem();
        group.Members[0].ProfileId.ShouldBe(profileId);
        _groupRepoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenGroupNotFound_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _groupRepoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var act = async () =>
            await _handler.Handle(
                new AddGroupMemberCommand(id, Guid.CreateVersion7(), GroupMemberRole.Student, new DateOnly(2025, 9, 1)),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenGroupOfDifferentOrganization_WhenHandling_ThenShouldThrowForbiddenException()
    {
        var group = CreateGroup(orgId: Guid.CreateVersion7());
        _groupRepoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () =>
            await _handler.Handle(
                new AddGroupMemberCommand(group.Id, Guid.CreateVersion7(), GroupMemberRole.Student, new DateOnly(2025, 9, 1)),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    [Test]
    public async Task GivenJoinedAtBeforeGroupStartDate_WhenHandling_ThenShouldThrowArgumentException()
    {
        var group = CreateGroup();
        _groupRepoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () =>
            await _handler.Handle(
                new AddGroupMemberCommand(
                    group.Id,
                    Guid.CreateVersion7(),
                    GroupMemberRole.Student,
                    new DateOnly(2025, 1, 1)
                ),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task GivenProfileNotOrgMember_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var group = CreateGroup();
        var profileId = Guid.CreateVersion7();
        _groupRepoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _memberRepoMock
            .Setup(r =>
                r.GetActiveMemberRoleIdAsync(
                    _organizationId,
                    profileId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((Guid?)null);

        var act = async () =>
            await _handler.Handle(
                new AddGroupMemberCommand(group.Id, profileId, GroupMemberRole.Student, new DateOnly(2025, 9, 1)),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<NotFoundException>();
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
            10,
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 6, 30)
        );
        group.Id = Guid.CreateVersion7();
        return group;
    }
}
