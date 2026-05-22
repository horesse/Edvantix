using Edvantix.Groups.Domain.LessonTypeAggregate;

namespace Edvantix.Groups.UnitTests.Domain.LessonTypes;

public sealed class LessonTypeTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();

    private static LessonType CreateValid() =>
        new(ValidOrgId, "Урок", "LESSON", 45, "#3B82F6", "CalendarDays");

    [Test]
    public void GivenValidParameters_WhenConstructing_ThenLessonTypeIsCreated()
    {
        var lt = new LessonType(
            ValidOrgId,
            "Урок",
            "LESSON",
            45,
            "#3B82F6",
            "CalendarDays",
            order: 1
        );

        lt.OrganizationId.ShouldBe(ValidOrgId);
        lt.Name.ShouldBe("Урок");
        lt.Code.ShouldBe("LESSON");
        lt.DefaultDurationMinutes.ShouldBe(45);
        lt.Color.ShouldBe("#3B82F6");
        lt.Icon.ShouldBe("CalendarDays");
        lt.Order.ShouldBe(1);
        lt.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenConstructing_ThenThrowsArgumentException()
    {
        var act = () => new LessonType(Guid.Empty, "Урок", "LESSON", 45, "#3B82F6", null);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrEmptyName_WhenConstructing_ThenThrowsArgumentException(string? name)
    {
        var act = () => new LessonType(ValidOrgId, name!, "LESSON", 45, "#3B82F6", null);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenTooLongName_WhenConstructing_ThenThrowsArgumentException()
    {
        var name = new string('А', 121);

        var act = () => new LessonType(ValidOrgId, name, "LESSON", 45, "#3B82F6", null);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrEmptyCode_WhenConstructing_ThenThrowsArgumentException(string? code)
    {
        var act = () => new LessonType(ValidOrgId, "Урок", code!, 45, "#3B82F6", null);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments("LESSON TYPE")] // пробел — недопустимый символ
    [Arguments("ABCDEFGHIJKLMNOPQRSTU")] // 21 символ — превышает максимум
    public void GivenInvalidCode_WhenConstructing_ThenThrowsArgumentException(string code)
    {
        var act = () => new LessonType(ValidOrgId, "Урок", code, 45, "#3B82F6", null);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenLowercaseCode_WhenConstructing_ThenCodeIsNormalizedToUppercase()
    {
        var lt = new LessonType(ValidOrgId, "Урок", "lesson", 45, "#3B82F6", null);

        lt.Code.ShouldBe("LESSON");
    }

    [Test]
    [Arguments(4)] // < 5
    [Arguments(601)] // > 600
    public void GivenOutOfRangeDuration_WhenConstructing_ThenThrowsArgumentException(int duration)
    {
        var act = () => new LessonType(ValidOrgId, "Урок", "LESSON", duration, "#3B82F6", null);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(5)]
    [Arguments(300)]
    [Arguments(600)]
    public void GivenValidDuration_WhenConstructing_ThenSucceeds(int duration)
    {
        var act = () => new LessonType(ValidOrgId, "Урок", "LESSON", duration, "#3B82F6", null);

        act.ShouldNotThrow();
    }

    [Test]
    [Arguments("3B82F6")] // без #
    [Arguments("#3B82F")] // 5 символов
    [Arguments("#ZZZZZZ")] // невалидные символы
    [Arguments("")]
    public void GivenInvalidColor_WhenConstructing_ThenThrowsArgumentException(string color)
    {
        var act = () => new LessonType(ValidOrgId, "Урок", "LESSON", 45, color, null);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenTooLongIcon_WhenConstructing_ThenThrowsArgumentException()
    {
        var icon = new string('X', 41); // > 40

        var act = () => new LessonType(ValidOrgId, "Урок", "LESSON", 45, "#3B82F6", icon);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNullIcon_WhenConstructing_ThenIconIsNull()
    {
        var lt = new LessonType(ValidOrgId, "Урок", "LESSON", 45, "#3B82F6", null);

        lt.Icon.ShouldBeNull();
    }

    [Test]
    public void GivenNameWithWhitespace_WhenConstructing_ThenNameIsTrimmed()
    {
        var lt = new LessonType(ValidOrgId, "  Урок  ", "LESSON", 45, "#3B82F6", null);

        lt.Name.ShouldBe("Урок");
    }

    [Test]
    public void GivenLowercaseCode_WhenConstructing_ThenCodeIsUppercased()
    {
        var lt = new LessonType(ValidOrgId, "Урок", "lesson", 45, "#3B82F6", null);

        lt.Code.ShouldBe("LESSON");
    }

    [Test]
    public void GivenLowercaseColor_WhenConstructing_ThenColorIsUppercased()
    {
        var lt = new LessonType(ValidOrgId, "Урок", "LESSON", 45, "#3b82f6", null);

        lt.Color.ShouldBe("#3B82F6");
    }

    [Test]
    public void GivenActiveLessonType_WhenArchiving_ThenIsArchivedTrue()
    {
        var lt = CreateValid();

        lt.Archive(Guid.Empty);

        lt.IsArchived.ShouldBeTrue();
    }

    [Test]
    public void GivenAlreadyArchivedLessonType_WhenArchivingAgain_ThenIdempotent()
    {
        var lt = CreateValid();
        lt.Archive(Guid.Empty);

        lt.Archive(Guid.Empty); // повторный вызов — no-op

        lt.IsArchived.ShouldBeTrue();
    }

    [Test]
    public void GivenArchivedLessonType_WhenRestoring_ThenIsArchivedFalse()
    {
        var lt = CreateValid();
        lt.Archive(Guid.Empty);

        lt.Restore(Guid.Empty);

        lt.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenActiveLessonType_WhenRestoringAgain_ThenIdempotent()
    {
        var lt = CreateValid();

        lt.Restore(Guid.Empty); // повторный вызов на активной — no-op

        lt.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenValidData_WhenUpdating_ThenAllPropertiesChanged()
    {
        var lt = CreateValid();

        lt.Update("Консультация", "CONSULT", 90, "#EF4444", "MessageSquare", Guid.Empty);

        lt.Name.ShouldBe("Консультация");
        lt.Code.ShouldBe("CONSULT");
        lt.DefaultDurationMinutes.ShouldBe(90);
        lt.Color.ShouldBe("#EF4444");
        lt.Icon.ShouldBe("MessageSquare");
    }

    [Test]
    public void GivenUpdate_WhenCalled_ThenLastModifiedAtIsSet()
    {
        var lt = CreateValid();

        lt.Update("Консультация", "CONSULT", 90, "#EF4444", null, Guid.Empty);

        lt.LastModifiedAt.ShouldNotBeNull();
    }
}
