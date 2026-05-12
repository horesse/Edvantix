namespace Edvantix.Curriculum.UnitTests.Domain;

public sealed class LessonAggregateTests
{
    private static Course CreateCourseWithModule(out Module module)
    {
        var course = new Course(
            Guid.CreateVersion7(),
            "EN-GEN-B1",
            "English General B1",
            CourseSubject.English,
            "B1",
            durationWeeks: 12,
            Guid.CreateVersion7()
        );

        module = course.AddModule("Модуль 1", null, weeks: 2);
        return course;
    }

    [Test]
    public void GivenValidParameters_WhenAddingLesson_ThenLessonShouldBeCreatedWithDraftStatus()
    {
        var course = CreateCourseWithModule(out var module);

        var lesson = course.AddLesson(
            module.Id,
            "Введение в тему",
            LessonType.Lecture,
            minutes: 60,
            ["Понять основы", "Выполнить упражнения"]
        );

        lesson.Id.ShouldNotBe(Guid.Empty);
        lesson.Title.ShouldBe("Введение в тему");
        lesson.Type.ShouldBe(LessonType.Lecture);
        lesson.Status.ShouldBe(LessonStatus.Draft);
        lesson.Minutes.ShouldBe((short)60);
        lesson.Objectives.Length.ShouldBe(2);
        lesson.Position.ShouldBe((short)1);
        module.Lessons.Count.ShouldBe(1);
    }

    [Test]
    public void GivenMultipleLessonsAdded_WhenInspectingPositions_ThenPositionsShouldBeSequential()
    {
        var course = CreateCourseWithModule(out var module);

        course.AddLesson(module.Id, "Урок 1", LessonType.Lecture, minutes: 45, []);
        course.AddLesson(module.Id, "Урок 2", LessonType.Practice, minutes: 60, []);
        course.AddLesson(module.Id, "Урок 3", LessonType.Test, minutes: 90, []);

        module.Lessons[0].Position.ShouldBe((short)1);
        module.Lessons[1].Position.ShouldBe((short)2);
        module.Lessons[2].Position.ShouldBe((short)3);
    }

    [Test]
    public void GivenUnknownModuleId_WhenAddingLesson_ThenShouldThrowNotFoundException()
    {
        var course = CreateCourseWithModule(out _);

        var act = () =>
            course.AddLesson(Guid.CreateVersion7(), "Урок", LessonType.Lecture, minutes: 45, []);

        act.ShouldThrow<Exception>();
    }

    [Test]
    public void GivenDraftLesson_WhenPublishing_ThenStatusShouldBePublished()
    {
        var course = CreateCourseWithModule(out var module);
        var lesson = course.AddLesson(module.Id, "Урок", LessonType.Speaking, minutes: 60, []);

        course.PublishLesson(lesson.Id);

        lesson.Status.ShouldBe(LessonStatus.Published);
    }

    [Test]
    public void GivenDeletedCourse_WhenPublishingLesson_ThenShouldThrowInvalidOperationException()
    {
        var course = CreateCourseWithModule(out var module);
        var lesson = course.AddLesson(module.Id, "Урок", LessonType.Lecture, minutes: 45, []);
        course.Delete();

        var act = () => course.PublishLesson(lesson.Id);

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenUnknownLessonId_WhenPublishingLesson_ThenShouldThrowNotFoundException()
    {
        var course = CreateCourseWithModule(out _);

        var act = () => course.PublishLesson(Guid.CreateVersion7());

        act.ShouldThrow<Exception>();
    }

    [Test]
    public void GivenCourseWithModulesAndLessons_WhenCheckingTotalLessons_ThenCountShouldBeCorrect()
    {
        var course = CreateCourseWithModule(out var module1);
        var module2 = course.AddModule("Модуль 2", null, weeks: 3);

        course.AddLesson(module1.Id, "Урок 1", LessonType.Lecture, minutes: 45, []);
        course.AddLesson(module1.Id, "Урок 2", LessonType.Practice, minutes: 60, []);
        course.AddLesson(module2.Id, "Урок 3", LessonType.Test, minutes: 90, []);

        course.TotalLessons.ShouldBe(3);
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    public void GivenInvalidMinutes_WhenCreatingLesson_ThenShouldThrowArgumentException(
        short minutes
    )
    {
        var course = CreateCourseWithModule(out var module);

        var act = () => course.AddLesson(module.Id, "Урок", LessonType.Lecture, minutes, []);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceTitle_WhenAddingLesson_ThenShouldThrowArgumentException(
        string? title
    )
    {
        var course = CreateCourseWithModule(out var module);

        var act = () => course.AddLesson(module.Id, title!, LessonType.Lecture, minutes: 45, []);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNullObjectives_WhenAddingLesson_ThenObjectivesShouldBeEmptyArray()
    {
        var course = CreateCourseWithModule(out var module);

        var lesson = course.AddLesson(module.Id, "Урок", LessonType.Lecture, minutes: 45, null!);

        lesson.Objectives.ShouldBeEmpty();
    }

    // ─── Lesson constructor (direct) ──────────────────────────────────────────

    [Test]
    public void GivenEmptyModuleId_WhenCreatingLesson_ThenShouldThrowArgumentException()
    {
        var act = () => new Lesson(Guid.Empty, 1, "Урок", LessonType.Lecture, 45, []);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    public void GivenInvalidPosition_WhenCreatingLesson_ThenShouldThrowArgumentException(
        short position
    )
    {
        var act = () =>
            new Lesson(Guid.CreateVersion7(), position, "Урок", LessonType.Lecture, 45, []);

        act.ShouldThrow<ArgumentException>();
    }

    // ─── Lesson.Move (direct) ─────────────────────────────────────────────────

    [Test]
    public void GivenValidPosition_WhenMovingLesson_ThenPositionShouldUpdate()
    {
        var course = CreateCourseWithModule(out var module);
        var lesson = course.AddLesson(module.Id, "Урок", LessonType.Lecture, minutes: 45, []);

        lesson.Move(3);

        lesson.Position.ShouldBe((short)3);
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    public void GivenZeroOrNegativePosition_WhenMovingLesson_ThenShouldThrowArgumentException(
        short position
    )
    {
        var course = CreateCourseWithModule(out var module);
        var lesson = course.AddLesson(module.Id, "Урок", LessonType.Lecture, minutes: 45, []);

        var act = () => lesson.Move(position);

        act.ShouldThrow<ArgumentException>();
    }

    // ─── Lesson.Update (direct) ───────────────────────────────────────────────

    [Test]
    public void GivenValidData_WhenUpdatingLesson_ThenShouldUpdateFields()
    {
        var course = CreateCourseWithModule(out var module);
        var lesson = course.AddLesson(module.Id, "Старое название", LessonType.Lecture, 45, []);

        lesson.Update("  Новое название  ", LessonType.Practice, 90, ["Цель 1"]);

        lesson.Title.ShouldBe("Новое название");
        lesson.Type.ShouldBe(LessonType.Practice);
        lesson.Minutes.ShouldBe((short)90);
        lesson.Objectives.Length.ShouldBe(1);
    }

    [Test]
    public void GivenNullObjectives_WhenUpdatingLesson_ThenObjectivesShouldBeEmptyArray()
    {
        var course = CreateCourseWithModule(out var module);
        var lesson = course.AddLesson(module.Id, "Урок", LessonType.Lecture, 45, ["Цель"]);

        lesson.Update("Урок", LessonType.Lecture, 45, null!);

        lesson.Objectives.ShouldBeEmpty();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceTitle_WhenUpdatingLesson_ThenShouldThrowArgumentException(
        string? title
    )
    {
        var course = CreateCourseWithModule(out var module);
        var lesson = course.AddLesson(module.Id, "Урок", LessonType.Lecture, 45, []);

        var act = () => lesson.Update(title!, LessonType.Lecture, 45, []);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    public void GivenZeroOrNegativeMinutes_WhenUpdatingLesson_ThenShouldThrowArgumentException(
        short minutes
    )
    {
        var course = CreateCourseWithModule(out var module);
        var lesson = course.AddLesson(module.Id, "Урок", LessonType.Lecture, 45, []);

        var act = () => lesson.Update("Урок", LessonType.Lecture, minutes, []);

        act.ShouldThrow<ArgumentException>();
    }
}
