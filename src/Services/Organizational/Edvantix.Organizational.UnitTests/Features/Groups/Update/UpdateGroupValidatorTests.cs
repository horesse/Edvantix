namespace Edvantix.Organizational.UnitTests.Features.Groups.Update;

public sealed class UpdateGroupValidatorTests
{
    private readonly UpdateGroupValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Id = Guid.Empty });

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Test]
    public void GivenEmptyName_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Name = string.Empty });

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void GivenEmptyDescription_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Description = string.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void GivenEmptyCourseId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { CourseId = Guid.Empty });

        result.ShouldHaveValidationErrorFor(x => x.CourseId);
    }

    [Test]
    public void GivenEmptyTeacherMemberId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                TeacherMemberId = Guid.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.TeacherMemberId);
    }

    [Test]
    public void GivenCapacityBelowOne_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Capacity = 0 });

        result.ShouldHaveValidationErrorFor(x => x.Capacity);
    }

    [Test]
    public void GivenCapacityAbove50_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Capacity = 51 });

        result.ShouldHaveValidationErrorFor(x => x.Capacity);
    }

    [Test]
    public void GivenOfflineFormatWithoutRoom_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
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
    public void GivenOnlineFormatWithoutPlatform_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Format = GroupFormat.Online,
                Platform = null,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Platform);
    }

    [Test]
    public void GivenMixedFormatWithoutRoom_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
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
    public void GivenMixedFormatWithoutPlatform_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
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
    public void GivenMixedFormatWithRoomAndPlatform_WhenValidating_ThenShouldNotHaveErrors()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Format = GroupFormat.Mixed,
                RoomId = Guid.CreateVersion7(),
                Platform = OnlinePlatform.Zoom,
            }
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateGroupCommand BuildValidCommand() =>
        new(
            Id: Guid.CreateVersion7(),
            Name: "Английский B1",
            Description: "Описание группы",
            Level: GroupLevel.B1,
            CourseId: Guid.CreateVersion7(),
            TeacherMemberId: Guid.CreateVersion7(),
            Format: GroupFormat.Online,
            RoomId: null,
            Platform: OnlinePlatform.Zoom,
            Capacity: 12,
            EndDate: new DateOnly(2026, 6, 30)
        );
}
