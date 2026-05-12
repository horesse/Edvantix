namespace Edvantix.Curriculum.UnitTests.Domain;

public sealed class CourseGoalTests
{
    private static readonly Guid ValidCourseId = Guid.CreateVersion7();

    // ─── Constructor ──────────────────────────────────────────────────────────

    [Test]
    public void GivenEmptyCourseId_WhenCreatingCourseGoal_ThenShouldThrowArgumentException()
    {
        var act = () => new CourseGoal(Guid.Empty, 1, "Научиться писать");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    public void GivenInvalidPosition_WhenCreatingCourseGoal_ThenShouldThrowArgumentException(
        short position
    )
    {
        var act = () => new CourseGoal(ValidCourseId, position, "Текст цели");

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceText_WhenCreatingCourseGoal_ThenShouldThrowArgumentException(
        string? text
    )
    {
        var act = () => new CourseGoal(ValidCourseId, 1, text!);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidParameters_WhenCreatingCourseGoal_ThenShouldInitializePropertiesCorrectly()
    {
        var goal = new CourseGoal(ValidCourseId, 2, "  Понять грамматику  ");

        goal.Id.ShouldNotBe(Guid.Empty);
        goal.CourseId.ShouldBe(ValidCourseId);
        goal.Position.ShouldBe((short)2);
        goal.Text.ShouldBe("Понять грамматику");
    }

    // ─── Update ───────────────────────────────────────────────────────────────

    [Test]
    public void GivenValidText_WhenUpdatingCourseGoal_ThenShouldUpdateAndTrimText()
    {
        var goal = new CourseGoal(ValidCourseId, 1, "Старый текст");

        goal.Update("  Новый текст  ");

        goal.Text.ShouldBe("Новый текст");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceText_WhenUpdatingCourseGoal_ThenShouldThrowArgumentException(
        string? text
    )
    {
        var goal = new CourseGoal(ValidCourseId, 1, "Текст");

        var act = () => goal.Update(text!);

        act.ShouldThrow<ArgumentException>();
    }
}
