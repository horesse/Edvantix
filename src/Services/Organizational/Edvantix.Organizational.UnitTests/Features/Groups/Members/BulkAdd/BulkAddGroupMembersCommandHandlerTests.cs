namespace Edvantix.Organizational.UnitTests.Features.Groups.Members.BulkAdd;

public sealed class BulkAddGroupMembersCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _groupRepoMock = new();
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly BulkAddGroupMembersCommandHandler _handler;

    public BulkAddGroupMembersCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _groupRepoMock.Object, _memberRepoMock.Object);
    }

    [Test]
    public async Task GivenAllValidItems_WhenHandling_ThenShouldReturnAllAdded()
    {
        var group = CreateGroup();
        var profileId1 = Guid.CreateVersion7();
        var profileId2 = Guid.CreateVersion7();
        var items = new[]
        {
            new BulkAddItem(profileId1, GroupMemberRole.Student, new DateOnly(2025, 9, 1)),
            new BulkAddItem(profileId2, GroupMemberRole.Teacher, new DateOnly(2025, 9, 1)),
        };

        SetupGroupRepo(group);
        SetupMemberRepo(profileId1, Guid.CreateVersion7());
        SetupMemberRepo(profileId2, Guid.CreateVersion7());

        var result = await _handler.Handle(
            new BulkAddGroupMembersCommand(group.Id, items),
            CancellationToken.None
        );

        result.Added.Count.ShouldBe(2);
        result.Failed.ShouldBeEmpty();
        _groupRepoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenOneInvalidItem_WhenHandling_ThenShouldReturnPartialSuccess()
    {
        var group = CreateGroup();
        var validProfileId = Guid.CreateVersion7();
        var invalidProfileId = Guid.CreateVersion7();
        var items = new[]
        {
            new BulkAddItem(validProfileId, GroupMemberRole.Student, new DateOnly(2025, 9, 1)),
            new BulkAddItem(invalidProfileId, GroupMemberRole.Student, new DateOnly(2025, 9, 1)),
        };

        SetupGroupRepo(group);
        SetupMemberRepo(validProfileId, Guid.CreateVersion7());
        _memberRepoMock
            .Setup(r =>
                r.GetActiveMemberRoleIdAsync(
                    _organizationId,
                    invalidProfileId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((Guid?)null);

        var result = await _handler.Handle(
            new BulkAddGroupMembersCommand(group.Id, items),
            CancellationToken.None
        );

        result.Added.Count.ShouldBe(1);
        result.Failed.Count.ShouldBe(1);
        result.Failed[0].ProfileId.ShouldBe(invalidProfileId);
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
                new BulkAddGroupMembersCommand(
                    id,
                    [new(Guid.CreateVersion7(), GroupMemberRole.Student, new DateOnly(2025, 9, 1))]
                ),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenAllItemsInvalid_WhenHandling_ThenShouldNotSave()
    {
        var group = CreateGroup();
        var profileId = Guid.CreateVersion7();

        SetupGroupRepo(group);
        _memberRepoMock
            .Setup(r =>
                r.GetActiveMemberRoleIdAsync(
                    _organizationId,
                    profileId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((Guid?)null);

        var result = await _handler.Handle(
            new BulkAddGroupMembersCommand(
                group.Id,
                [new BulkAddItem(profileId, GroupMemberRole.Student, new DateOnly(2025, 9, 1))]
            ),
            CancellationToken.None
        );

        result.Added.ShouldBeEmpty();
        result.Failed.Count.ShouldBe(1);
        _groupRepoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    private void SetupGroupRepo(Group group)
    {
        _groupRepoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _groupRepoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private void SetupMemberRepo(Guid profileId, Guid roleId)
    {
        _memberRepoMock
            .Setup(r =>
                r.GetActiveMemberRoleIdAsync(
                    _organizationId,
                    profileId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(roleId);
    }

    private Group CreateGroup()
    {
        var group = new Group(
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
        group.Id = Guid.CreateVersion7();
        return group;
    }
}
