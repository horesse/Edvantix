namespace Edvantix.Groups.UnitTests.Features.Groups.ChangeStatus;

public sealed class ChangeGroupStatusCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly ChangeGroupStatusCommandHandler _handler;

    public ChangeGroupStatusCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    [Arguments(GroupStatus.Active)]
    [Arguments(GroupStatus.Paused)]
    [Arguments(GroupStatus.Finished)]
    public async Task GivenActiveGroup_WhenChangingToValidStatus_ThenShouldUpdateAndSave(
        GroupStatus newStatus
    )
    {
        var group = CreateGroup();
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(
            new ChangeGroupStatusCommand(group.Id, newStatus),
            CancellationToken.None
        );

        group.Status.ShouldBe(newStatus);
    }

    [Test]
    public async Task GivenArchivedGroup_WhenChangingStatus_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateGroup();
        group.Archive();

        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () =>
            await _handler.Handle(
                new ChangeGroupStatusCommand(group.Id, GroupStatus.Active),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task GivenGroupNotFound_WhenChangingStatus_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var act = async () =>
            await _handler.Handle(
                new ChangeGroupStatusCommand(id, GroupStatus.Active),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<NotFoundException>();
    }

    private Group CreateGroup() =>
        new(
            _organizationId,
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
}
