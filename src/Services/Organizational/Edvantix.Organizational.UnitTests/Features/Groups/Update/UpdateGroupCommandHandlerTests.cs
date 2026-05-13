namespace Edvantix.Organizational.UnitTests.Features.Groups.Update;

public sealed class UpdateGroupCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly UpdateGroupCommandHandler _handler;

    public UpdateGroupCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingGroup_WhenUpdating_ThenShouldUpdateFieldsAndSave()
    {
        var group = CreateGroup();
        var command = BuildCommand(group.Id);
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        group.Name.ShouldBe(command.Name);
        group.Capacity.ShouldBe(command.Capacity);
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenGroupNotFound_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var act = async () => await _handler.Handle(BuildCommand(id), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenGroupOfDifferentOrganization_WhenUpdating_ThenShouldThrowForbiddenException()
    {
        var group = CreateGroup(organizationId: Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () =>
            await _handler.Handle(BuildCommand(group.Id), CancellationToken.None);

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    [Test]
    public async Task GivenArchivedGroup_WhenUpdating_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateGroup();
        group.Archive();
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () =>
            await _handler.Handle(BuildCommand(group.Id), CancellationToken.None);

        await act.ShouldThrowAsync<InvalidOperationException>();
    }

    private static UpdateGroupCommand BuildCommand(Guid id) =>
        new(
            Id: id,
            Name: "Английский B1 — обновлённый",
            Description: "Обновлённое описание",
            Level: GroupLevel.B1,
            CourseId: Guid.CreateVersion7(),
            TeacherMemberId: Guid.CreateVersion7(),
            Format: GroupFormat.Online,
            RoomId: null,
            Platform: OnlinePlatform.Zoom,
            Capacity: 15,
            EndDate: new DateOnly(2026, 12, 31)
        );

    private Group CreateGroup(Guid? organizationId = null) =>
        new(
            organizationId ?? _organizationId,
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
}
