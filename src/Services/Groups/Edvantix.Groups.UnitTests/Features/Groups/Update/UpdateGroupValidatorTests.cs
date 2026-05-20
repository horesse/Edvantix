namespace Edvantix.Groups.UnitTests.Features.Groups.Update;

public sealed class UpdateGroupValidatorTests
{
    private readonly Mock<ILevelRepository> _levelRepoMock = new();
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly Guid _validLevelId = Guid.CreateVersion7();
    private readonly UpdateGroupValidator _validator;

    public UpdateGroupValidatorTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);

        _levelRepoMock
            .Setup(r => r.GetByIdAsync(_validLevelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new Level(
                    _organizationId,
                    LevelCode.From("B1"),
                    "B1 Level",
                    null,
                    LevelTone.Blue,
                    1
                )
            );

        _validator = new(_levelRepoMock.Object, _tenantMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = await _validator.TestValidateAsync(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task GivenEmptyId_WhenValidating_ThenShouldHaveError()
    {
        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                Id = Guid.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Id);
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
            .Setup(r => r.GetByIdAsync(unknownLevelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Level?)null);

        var result = await _validator.TestValidateAsync(
            BuildValidCommand() with
            {
                LevelId = unknownLevelId,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.LevelId);
    }

    [Test]
    public async Task GivenInactiveLevelId_WhenValidating_ThenShouldHaveError()
    {
        var inactiveLevelId = Guid.CreateVersion7();
        var inactiveLevel = new Level(
            _organizationId,
            LevelCode.From("B1"),
            "B1 Level",
            null,
            LevelTone.Blue,
            1
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
            LevelCode.From("B1"),
            "B1 Level",
            null,
            LevelTone.Blue,
            1
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

    private UpdateGroupCommand BuildValidCommand() =>
        new(
            Id: Guid.CreateVersion7(),
            Name: "Английский B1",
            Description: "Описание группы",
            LevelId: _validLevelId,
            CourseId: Guid.CreateVersion7(),
            TeacherMemberId: Guid.CreateVersion7(),
            Format: GroupFormat.Online,
            RoomId: null,
            Platform: OnlinePlatform.Zoom,
            Capacity: 12,
            EndDate: new DateOnly(2026, 6, 30)
        );
}
