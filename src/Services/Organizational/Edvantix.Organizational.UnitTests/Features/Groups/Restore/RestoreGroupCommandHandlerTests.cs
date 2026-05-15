namespace Edvantix.Organizational.UnitTests.Features.Groups.Restore;

public sealed class RestoreGroupCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly RestoreGroupCommandHandler _handler;

    public RestoreGroupCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenArchivedGroup_WhenRestoring_ThenShouldRestoreToRecruitingAndSave()
    {
        var group = CreateArchivedGroup();
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(new RestoreGroupCommand(group.Id), CancellationToken.None);

        group.Status.ShouldBe(GroupStatus.Recruiting);
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenGroupNotFound_WhenRestoring_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var act = async () =>
            await _handler.Handle(new RestoreGroupCommand(id), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenGroupOfDifferentOrganization_WhenRestoring_ThenShouldThrowForbiddenException()
    {
        var group = CreateArchivedGroup(organizationId: Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () =>
            await _handler.Handle(new RestoreGroupCommand(group.Id), CancellationToken.None);

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    private Group CreateArchivedGroup(Guid? organizationId = null)
    {
        var group = new Group(
            organizationId ?? _organizationId,
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

        group.Archive();
        return group;
    }
}
