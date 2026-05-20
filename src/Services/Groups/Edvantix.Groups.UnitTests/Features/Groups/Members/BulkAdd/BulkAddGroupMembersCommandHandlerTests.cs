namespace Edvantix.Groups.UnitTests.Features.Groups.Members.BulkAdd;

public sealed class BulkAddGroupMembersCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _groupRepoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly BulkAddGroupMembersCommandHandler _handler;

    public BulkAddGroupMembersCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _groupRepoMock.Object);
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
            // JoinedAt before StartDate — invalid
            new BulkAddItem(invalidProfileId, GroupMemberRole.Student, new DateOnly(2025, 8, 31)),
        };

        SetupGroupRepo(group);

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

        var result = await _handler.Handle(
            new BulkAddGroupMembersCommand(
                group.Id,
                // JoinedAt before StartDate — invalid
                [new BulkAddItem(profileId, GroupMemberRole.Student, new DateOnly(2025, 8, 31))]
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

    [Test]
    public async Task GivenGroupOfDifferentOrganization_WhenHandling_ThenShouldThrowForbiddenException()
    {
        var group = CreateGroup(orgId: Guid.CreateVersion7());
        SetupGroupRepo(group);

        var act = async () =>
            await _handler.Handle(
                new BulkAddGroupMembersCommand(
                    group.Id,
                    [
                        new BulkAddItem(
                            Guid.CreateVersion7(),
                            GroupMemberRole.Student,
                            new DateOnly(2025, 9, 1)
                        ),
                    ]
                ),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    [Test]
    public async Task GivenJoinedAtBeforeGroupStartDate_WhenHandling_ThenShouldAddToFailed()
    {
        var group = CreateGroup();
        var profileId = Guid.CreateVersion7();

        SetupGroupRepo(group);

        var result = await _handler.Handle(
            new BulkAddGroupMembersCommand(
                group.Id,
                [new BulkAddItem(profileId, GroupMemberRole.Student, new DateOnly(2025, 8, 31))]
            ),
            CancellationToken.None
        );

        result.Added.ShouldBeEmpty();
        result.Failed.Count.ShouldBe(1);
        result.Failed[0].ProfileId.ShouldBe(profileId);
    }

    [Test]
    public async Task GivenDuplicateActiveMember_WhenHandling_ThenShouldAddToFailed()
    {
        var group = CreateGroup();
        var profileId = Guid.CreateVersion7();
        var joinedAt = new DateOnly(2025, 9, 1);

        var existing = new GroupMember(
            _organizationId,
            group.Id,
            profileId,
            GroupMemberRole.Student,
            joinedAt
        );
        group.AddMember(existing);

        SetupGroupRepo(group);

        var result = await _handler.Handle(
            new BulkAddGroupMembersCommand(
                group.Id,
                [new BulkAddItem(profileId, GroupMemberRole.Student, joinedAt)]
            ),
            CancellationToken.None
        );

        result.Added.ShouldBeEmpty();
        result.Failed.Count.ShouldBe(1);
        result.Failed[0].ProfileId.ShouldBe(profileId);
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

    private Group CreateGroup(Guid? orgId = null)
    {
        var group = new Group(
            orgId ?? _organizationId,
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
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 6, 30)
        );
        group.Id = Guid.CreateVersion7();
        return group;
    }
}
