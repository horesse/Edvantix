namespace Edvantix.Curriculum.UnitTests.Features.Lessons.Update;

public sealed class UpdateLessonValidatorTests
{
    private readonly UpdateLessonValidator _validator = new();

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldPass()
    {
        var command = new UpdateLessonCommand(
            Guid.CreateVersion7(),
            "Title",
            LessonType.Lecture,
            45,
            ["Objective"]
        );

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task GivenEmptyLessonId_WhenValidating_ThenShouldFail()
    {
        var command = new UpdateLessonCommand(Guid.Empty, "Title", LessonType.Lecture, 45, []);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(UpdateLessonCommand.LessonId));
    }

    [Test]
    public async Task GivenEmptyTitle_WhenValidating_ThenShouldFail()
    {
        var command = new UpdateLessonCommand(
            Guid.CreateVersion7(),
            string.Empty,
            LessonType.Lecture,
            45,
            []
        );

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(UpdateLessonCommand.Title));
    }

    [Test]
    public async Task GivenZeroMinutes_WhenValidating_ThenShouldFail()
    {
        var command = new UpdateLessonCommand(
            Guid.CreateVersion7(),
            "Title",
            LessonType.Lecture,
            0,
            []
        );

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(UpdateLessonCommand.Minutes));
    }
}
