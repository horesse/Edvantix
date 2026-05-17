namespace Edvantix.Organizational.UnitTests.Features.Groups.Create;

public sealed class CreateGroupValidatorTests
{
    private readonly Mock<ILevelRepository> _levelRepoMock = new();
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock = new();
    private readonly Mock<IRoomRepository> _roomRepoMock = new();
    private readonly Mock<ICurriculumService> _curriculumMock = new();
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly Guid _validLevelId = Guid.CreateVersion7();
    private readonly CreateGroupValidator _validator;

    public CreateGroupValidatorTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);

        _levelRepoMock
            .Setup(r =>
                r.ExistsAsync(_validLevelId, _organizationId, true, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        _memberRepoMock
            .Setup(r =>
                r.ExistsAsync(It.IsAny<Guid>(), _organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        _roomRepoMock
            .Setup(r =>
                r.ExistsAsync(It.IsAny<Guid>(), _organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        _curriculumMock
            .Setup(s => s.GetCourseByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CourseInfo { OrganizationId = _organizationId.ToString() });

        _validator = new(
            _levelRepoMock.Object,
            _memberRepoMock.Object,
            _roomRepoMock.Object,
            _curriculumMock.Object,
            _tenantMock.Object
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = await _validator.TestValidateAsync(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task GivenEmptyCode_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Code = string.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Test]
    public async Task GivenInvalidCodeFormat_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Code = "b1-01",
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Test]
    public async Task GivenEmptyName_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Name = string.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task GivenEmptyLevelId_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                LevelId = Guid.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.LevelId);
    }

    [Test]
    public async Task GivenNonExistentLevelId_WhenValidating_ThenShouldHaveError()
    {
        var unknownLevelId = Guid.CreateVersion7();
        _levelRepoMock
            .Setup(r =>
                r.ExistsAsync(
                    unknownLevelId,
                    _organizationId,
                    true,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with { LevelId = unknownLevelId }
        );

        result.ShouldHaveValidationErrorFor(x => x.LevelId);
    }

    [Test]
    public async Task GivenInactiveLevelId_WhenValidating_ThenShouldHaveError()
    {
        var inactiveLevelId = Guid.CreateVersion7();
        _levelRepoMock
            .Setup(r =>
                r.ExistsAsync(
                    inactiveLevelId,
                    _organizationId,
                    true,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with { LevelId = inactiveLevelId }
        );

        result.ShouldHaveValidationErrorFor(x => x.LevelId);
    }

    [Test]
    public async Task GivenLevelFromAnotherOrganization_WhenValidating_ThenShouldHaveError()
    {
        var foreignLevelId = Guid.CreateVersion7();
        _levelRepoMock
            .Setup(r =>
                r.ExistsAsync(
                    foreignLevelId,
                    _organizationId,
                    true,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with { LevelId = foreignLevelId }
        );

        result.ShouldHaveValidationErrorFor(x => x.LevelId);
    }

    [Test]
    public async Task GivenEmptyCourseId_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                CourseId = Guid.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.CourseId);
    }

    [Test]
    public async Task GivenNonExistentCourseId_WhenValidating_ThenShouldHaveError()
    {
        var unknownCourseId = Guid.CreateVersion7();
        _curriculumMock
            .Setup(s =>
                s.GetCourseByIdAsync(
                    unknownCourseId.ToString(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((CourseInfo?)null);

        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with { CourseId = unknownCourseId }
        );

        result.ShouldHaveValidationErrorFor(x => x.CourseId);
    }

    [Test]
    public async Task GivenCourseFromAnotherOrganization_WhenValidating_ThenShouldHaveError()
    {
        var foreignCourseId = Guid.CreateVersion7();
        _curriculumMock
            .Setup(s =>
                s.GetCourseByIdAsync(
                    foreignCourseId.ToString(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new CourseInfo { OrganizationId = Guid.CreateVersion7().ToString() });

        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with { CourseId = foreignCourseId }
        );

        result.ShouldHaveValidationErrorFor(x => x.CourseId);
    }

    [Test]
    public async Task GivenEmptyTeacherMemberId_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                TeacherMemberId = Guid.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.TeacherMemberId);
    }

    [Test]
    public async Task GivenTeacherMemberFromAnotherOrganization_WhenValidating_ThenShouldHaveError()
    {
        var foreignMemberId = Guid.CreateVersion7();
        _memberRepoMock
            .Setup(r =>
                r.ExistsAsync(foreignMemberId, _organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with { TeacherMemberId = foreignMemberId }
        );

        result.ShouldHaveValidationErrorFor(x => x.TeacherMemberId);
    }

    [Test]
    public async Task GivenCapacityBelowOne_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(BuildValidCommand() with { Capacity = 0 });

        result.ShouldHaveValidationErrorFor(x => x.Capacity);
    }

    [Test]
    public async Task GivenCapacityAbove50_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(BuildValidCommand() with { Capacity = 51 });

        result.ShouldHaveValidationErrorFor(x => x.Capacity);
    }

    [Test]
    public async Task GivenEndDateBeforeStartDate_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                EndDate = new DateOnly(2025, 1, 1),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Test]
    public async Task GivenOfflineFormatWithoutRoom_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Format = GroupFormat.Offline,
                RoomId = null,
                Platform = null,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.RoomId);
    }

    [Test]
    public async Task GivenOfflineFormatWithRoomFromAnotherOrganization_WhenValidating_ThenShouldHaveError()
    {
        var foreignRoomId = Guid.CreateVersion7();
        _roomRepoMock
            .Setup(r =>
                r.ExistsAsync(foreignRoomId, _organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Format = GroupFormat.Offline,
                RoomId = foreignRoomId,
                Platform = null,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.RoomId);
    }

    [Test]
    public async Task GivenOnlineFormatWithNullRoom_WhenValidating_ThenShouldNotHaveRoomError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Format = GroupFormat.Online,
                RoomId = null,
                Platform = OnlinePlatform.Zoom,
            }
        );

        result.ShouldNotHaveValidationErrorFor(x => x.RoomId);
    }

    [Test]
    public async Task GivenOnlineFormatWithoutPlatform_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Format = GroupFormat.Online,
                Platform = null,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Platform);
    }

    [Test]
    public async Task GivenEmptyDescription_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Description = string.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public async Task GivenCodeExceedingMaxLength_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Code = new string('A', 33),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Test]
    public async Task GivenMixedFormatWithoutRoom_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Format = GroupFormat.Mixed,
                RoomId = null,
                Platform = OnlinePlatform.Zoom,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.RoomId);
    }

    [Test]
    public async Task GivenMixedFormatWithoutPlatform_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Format = GroupFormat.Mixed,
                RoomId = Guid.CreateVersion7(),
                Platform = null,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Platform);
    }

    [Test]
    public async Task GivenMixedFormatWithRoomAndPlatform_WhenValidating_ThenShouldNotHaveErrors()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Format = GroupFormat.Mixed,
                RoomId = Guid.CreateVersion7(),
                Platform = OnlinePlatform.Zoom,
            }
        );

        result.ShouldNotHaveValidationErrorFor(x => x.RoomId);
        result.ShouldNotHaveValidationErrorFor(x => x.Platform);
    }

    private CreateGroupCommand BuildValidCommand() =>
        new(
            Code: "B1-01",
            Name: "Английский B1",
            Description: "Описание группы",
            LevelId: _validLevelId,
            CourseId: Guid.CreateVersion7(),
            TeacherMemberId: Guid.CreateVersion7(),
            Format: GroupFormat.Online,
            RoomId: null,
            Platform: OnlinePlatform.Zoom,
            Capacity: 12,
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30)
        );
}
