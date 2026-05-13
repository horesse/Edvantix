namespace Edvantix.Organizational.UnitTests.Features.Groups.Create;

public sealed class CreateGroupCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Mock<ICurriculumService> _curriculumMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly CreateGroupCommandHandler _handler;

    public CreateGroupCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _curriculumMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldAddGroup()
    {
        var command = BuildCommand();
        SetupCurriculumFound(command.CourseId);
        SetupRepoPersist();

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<Group>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldSaveChanges()
    {
        var command = BuildCommand();
        SetupCurriculumFound(command.CourseId);
        SetupRepoPersist();

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenOnlineFormat_WhenHandling_ThenShouldSucceedWithPlatform()
    {
        var command = BuildCommand(
            format: GroupFormat.Online,
            roomId: null,
            platform: OnlinePlatform.Zoom
        );
        SetupCurriculumFound(command.CourseId);
        SetupRepoPersist();

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.ShouldNotThrowAsync();
    }

    [Test]
    public async Task GivenOfflineFormatWithoutRoom_WhenHandling_ThenShouldThrow()
    {
        var command = BuildCommand(format: GroupFormat.Offline, roomId: null, platform: null);
        SetupCurriculumFound(command.CourseId);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.ShouldThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task GivenCourseNotFound_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var command = BuildCommand();
        _curriculumMock
            .Setup(s =>
                s.GetCourseByIdAsync(command.CourseId.ToString(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((CourseInfo?)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenCourseFromDifferentOrganization_WhenHandling_ThenShouldThrowForbiddenException()
    {
        var command = BuildCommand();
        _curriculumMock
            .Setup(s =>
                s.GetCourseByIdAsync(command.CourseId.ToString(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new CourseInfo { OrganizationId = Guid.CreateVersion7().ToString() });

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    private void SetupCurriculumFound(Guid courseId) =>
        _curriculumMock
            .Setup(s => s.GetCourseByIdAsync(courseId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CourseInfo { OrganizationId = _organizationId.ToString() });

    private void SetupRepoPersist()
    {
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Group>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private static CreateGroupCommand BuildCommand(
        GroupFormat format = GroupFormat.Online,
        Guid? roomId = null,
        OnlinePlatform? platform = OnlinePlatform.GoogleMeet
    ) =>
        new(
            Code: "B1-01",
            Name: "Английский B1",
            Description: "Группа уровня B1",
            Level: GroupLevel.B1,
            CourseId: Guid.CreateVersion7(),
            TeacherMemberId: Guid.CreateVersion7(),
            Format: format,
            RoomId: roomId,
            Platform: platform,
            Capacity: 12,
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30)
        );
}
