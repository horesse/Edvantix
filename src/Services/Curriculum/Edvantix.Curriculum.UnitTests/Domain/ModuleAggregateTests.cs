namespace Edvantix.Curriculum.UnitTests.Domain;

public sealed class ModuleAggregateTests
{
    private static readonly Guid ValidCourseId = Guid.CreateVersion7();

    private static Course CreateValidCourse() =>
        new(
            Guid.CreateVersion7(),
            "EN-GEN-B1",
            "English General B1",
            CourseSubject.English,
            "B1",
            durationWeeks: 12,
            Guid.CreateVersion7()
        );

    [Test]
    public void GivenValidParameters_WhenAddingModule_ThenModuleShouldBeCreatedWithPosition1()
    {
        var course = CreateValidCourse();

        var module = course.AddModule("Введение", "Базовый раздел", weeks: 2);

        module.Id.ShouldNotBe(Guid.Empty);
        module.Name.ShouldBe("Введение");
        module.Summary.ShouldBe("Базовый раздел");
        module.Weeks.ShouldBe((short)2);
        module.Position.ShouldBe((short)1);
        module.CourseId.ShouldBe(course.Id);
        course.Modules.Count.ShouldBe(1);
    }

    [Test]
    public void GivenMultipleModulesAdded_WhenInspectingPositions_ThenPositionsShouldBeSequential()
    {
        var course = CreateValidCourse();

        course.AddModule("Модуль 1", null, weeks: 2);
        course.AddModule("Модуль 2", null, weeks: 3);
        course.AddModule("Модуль 3", null, weeks: 4);

        course.Modules[0].Position.ShouldBe((short)1);
        course.Modules[1].Position.ShouldBe((short)2);
        course.Modules[2].Position.ShouldBe((short)3);
    }

    [Test]
    public void GivenDeletedCourse_WhenAddingModule_ThenShouldThrowInvalidOperationException()
    {
        var course = CreateValidCourse();
        course.Delete();

        var act = () => course.AddModule("Модуль", null, weeks: 2);

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-5)]
    public void GivenInvalidWeeks_WhenCreatingModule_ThenShouldThrowArgumentException(short weeks)
    {
        var course = CreateValidCourse();

        var act = () => course.AddModule("Модуль", null, weeks);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidOrderedIds_WhenReorderingModules_ThenPositionsShouldUpdate()
    {
        var course = CreateValidCourse();
        var m1 = course.AddModule("Модуль 1", null, weeks: 2);
        var m2 = course.AddModule("Модуль 2", null, weeks: 3);
        var m3 = course.AddModule("Модуль 3", null, weeks: 4);

        course.ReorderModules([m3.Id, m1.Id, m2.Id]);

        course.Modules.First(m => m.Id == m3.Id).Position.ShouldBe((short)1);
        course.Modules.First(m => m.Id == m1.Id).Position.ShouldBe((short)2);
        course.Modules.First(m => m.Id == m2.Id).Position.ShouldBe((short)3);
    }

    [Test]
    public void GivenMismatchedCount_WhenReorderingModules_ThenShouldThrowArgumentException()
    {
        var course = CreateValidCourse();
        var m1 = course.AddModule("Модуль 1", null, weeks: 2);
        course.AddModule("Модуль 2", null, weeks: 3);

        var act = () => course.ReorderModules([m1.Id]);

        act.ShouldThrow<ArgumentException>();
    }
}
