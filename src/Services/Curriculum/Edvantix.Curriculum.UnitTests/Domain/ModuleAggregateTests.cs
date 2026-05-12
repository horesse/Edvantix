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

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenAddingModule_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var course = CreateValidCourse();

        var act = () => course.AddModule(name!, null, weeks: 2);

        act.ShouldThrow<ArgumentException>();
    }

    // ─── Module.Update (direct) ───────────────────────────────────────────────

    [Test]
    public void GivenValidData_WhenUpdatingModule_ThenShouldUpdateFields()
    {
        var course = CreateValidCourse();
        var module = course.AddModule("Старое название", "Старое описание", weeks: 2);

        module.Update("Новое название", "Новое описание", 4);

        module.Name.ShouldBe("Новое название");
        module.Summary.ShouldBe("Новое описание");
        module.Weeks.ShouldBe((short)4);
    }

    [Test]
    public void GivenNullSummary_WhenUpdatingModule_ThenSummaryShouldBeNull()
    {
        var course = CreateValidCourse();
        var module = course.AddModule("Название", "Описание", weeks: 2);

        module.Update("Название", null, 3);

        module.Summary.ShouldBeNull();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenUpdatingModule_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var course = CreateValidCourse();
        var module = course.AddModule("Название", null, weeks: 2);

        var act = () => module.Update(name!, null, 2);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    public void GivenZeroOrNegativeWeeks_WhenUpdatingModule_ThenShouldThrowArgumentException(
        short weeks
    )
    {
        var course = CreateValidCourse();
        var module = course.AddModule("Название", null, weeks: 2);

        var act = () => module.Update("Название", null, weeks);

        act.ShouldThrow<ArgumentException>();
    }

    // ─── Module.SetPosition (direct) ─────────────────────────────────────────

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    public void GivenZeroOrNegativePosition_WhenSettingModulePosition_ThenShouldThrowArgumentException(
        short position
    )
    {
        var course = CreateValidCourse();
        var module = course.AddModule("Название", null, weeks: 2);

        var act = () => module.SetPosition(position);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidPosition_WhenSettingModulePosition_ThenPositionShouldUpdate()
    {
        var course = CreateValidCourse();
        var module = course.AddModule("Название", null, weeks: 2);

        module.SetPosition(5);

        module.Position.ShouldBe((short)5);
    }
}
