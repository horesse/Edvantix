namespace Edvantix.Organizational.UnitTests.Features.Groups.Create;

public sealed class CreateGroupValidatorTests
{
    private readonly Mock<ILevelRepository> _levelRepoMock = new();
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly Guid _validLevelId = Guid.CreateVersion7();
    private readonly CreateGroupValidator _validator;

    public CreateGroupValidatorTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _validator = new(_levelRepoMock.Object, _tenantMock.Object);
        SetupValidLevel(_validLevelId);
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

    [Test]
    public async Task GivenInactiveLevelId_WhenValidating_ThenShouldHaveError()
    {
        var inactiveLevelId = Guid.CreateVersion7();
        var inactiveLevel = new Level(
            _organizationId,
            LevelCode.From("A1"),
            "Начальный",
            null,
            LevelTone.Blue,
            10
        );
        inactiveLevel.Deactivate();
        _levelRepoMock
            .Setup(r => r.GetByIdAsync(inactiveLevelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveLevel);

        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                LevelId = inactiveLevelId,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.LevelId);
    }

    [Test]
    public async Task GivenLevelFromAnotherOrganization_WhenValidating_ThenShouldHaveError()
    {
        var foreignLevelId = Guid.CreateVersion7();
        var foreignLevel = new Level(
            Guid.CreateVersion7(),
            LevelCode.From("B2"),
            "Выше среднего",
            null,
            LevelTone.Blue,
            40
        );
        _levelRepoMock
            .Setup(r => r.GetByIdAsync(foreignLevelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(foreignLevel);

        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                LevelId = foreignLevelId,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.LevelId);
    }

    private void SetupValidLevel(Guid levelId)
    {
        var level = new Level(
            _organizationId,
            LevelCode.From("B1"),
            "Средний",
            null,
            LevelTone.Blue,
            30
        );
        _levelRepoMock
            .Setup(r => r.GetByIdAsync(levelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(level);
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
