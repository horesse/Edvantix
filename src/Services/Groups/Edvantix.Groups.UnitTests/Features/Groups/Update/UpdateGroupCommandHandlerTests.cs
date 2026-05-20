namespace Edvantix.Groups.UnitTests.Features.Groups.Update;

public sealed class UpdateGroupCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Mock<ICurriculumService> _curriculumMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly UpdateGroupCommandHandler _handler;

    public UpdateGroupCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _curriculumMock.Object);
    }

    [Test]
    public async Task GivenExistingGroup_WhenUpdating_ThenShouldUpdateFieldsAndSave()
    {
        var group = CreateGroup();
        var command = BuildCommand(group.Id);
        SetupCurriculumFound(command.CourseId);
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

        var act = async () => await _handler.Handle(BuildCommand(group.Id), CancellationToken.None);

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    [Test]
    public async Task GivenArchivedGroup_WhenUpdating_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateGroup();
        group.Archive();
        var command = BuildCommand(group.Id);
        SetupCurriculumFound(command.CourseId);
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.ShouldThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task GivenChangedCourseNotFound_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var group = CreateGroup();
        var command = BuildCommand(group.Id);
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _curriculumMock
            .Setup(s =>
                s.GetCourseByIdAsync(command.CourseId.ToString(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((CourseInfo?)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenChangedCourseFromDifferentOrganization_WhenUpdating_ThenShouldThrowForbiddenException()
    {
        var group = CreateGroup();
        var command = BuildCommand(group.Id);
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _curriculumMock
            .Setup(s =>
                s.GetCourseByIdAsync(command.CourseId.ToString(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new CourseInfo { OrganizationId = Guid.CreateVersion7().ToString() });

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    [Test]
    public async Task GivenSameCourseId_WhenUpdating_ThenShouldNotCallCurriculumService()
    {
        var group = CreateGroup();
        var command = BuildCommand(group.Id, courseId: group.CourseId);
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        _curriculumMock.Verify(
            s => s.GetCourseByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    private void SetupCurriculumFound(Guid courseId) =>
        _curriculumMock
            .Setup(s => s.GetCourseByIdAsync(courseId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CourseInfo { OrganizationId = _organizationId.ToString() });

    private static UpdateGroupCommand BuildCommand(Guid id, Guid? courseId = null) =>
        new(
            Id: id,
            Name: "Английский B1 — обновлённый",
            Description: "Обновлённое описание",
            LevelId: Guid.CreateVersion7(),
            CourseId: courseId ?? Guid.CreateVersion7(),
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
