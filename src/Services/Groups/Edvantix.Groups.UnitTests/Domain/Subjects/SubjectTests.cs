using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;

namespace Edvantix.Groups.UnitTests.Domain.Subjects;

public sealed class SubjectTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();
    private static readonly SubjectCode ValidCode = SubjectCode.From("MATH");

    private static Subject CreateValidSubject() =>
        new(ValidOrgId, "Математика", ValidCode, "#6366F1", null);

    [Test]
    public void GivenValidParameters_WhenConstructing_ThenSubjectIsCreated()
    {
        var subject = new Subject(
            ValidOrgId,
            "Математика",
            ValidCode,
            "#6366F1",
            "Описание предмета"
        );

        subject.OrganizationId.ShouldBe(ValidOrgId);
        subject.Name.ShouldBe("Математика");
        subject.Code.ShouldBe(ValidCode);
        subject.Color.ShouldBe("#6366F1");
        subject.Description.ShouldBe("Описание предмета");
        subject.IsArchived.ShouldBeFalse();
        subject.Order.ShouldBe(0);
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenConstructing_ThenThrowsArgumentException()
    {
        var act = () => new Subject(Guid.Empty, "Математика", ValidCode, "#6366F1", null);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrEmptyName_WhenConstructing_ThenThrowsArgumentException(string? name)
    {
        var act = () => new Subject(ValidOrgId, name!, ValidCode, "#6366F1", null);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenTooLongName_WhenConstructing_ThenThrowsArgumentException()
    {
        var name = new string('A', 121); // 121 chars, max is 120

        var act = () => new Subject(ValidOrgId, name, ValidCode, "#6366F1", null);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments("#GGGGGG")]
    [Arguments("6366F1")]
    [Arguments("#6366F")]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenInvalidColor_WhenConstructing_ThenThrowsArgumentException(string color)
    {
        var act = () => new Subject(ValidOrgId, "Математика", ValidCode, color, null);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenLowercaseHexColor_WhenConstructing_ThenNormalizesToUppercase()
    {
        var subject = new Subject(ValidOrgId, "Математика", ValidCode, "#6366f1", null);

        subject.Color.ShouldBe("#6366F1");
    }

    [Test]
    public void GivenTooLongDescription_WhenConstructing_ThenThrowsArgumentException()
    {
        var description = new string('X', 501); // 501 chars, max is 500

        var act = () => new Subject(ValidOrgId, "Математика", ValidCode, "#6366F1", description);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNameWithWhitespace_WhenConstructing_ThenNameIsTrimmed()
    {
        var subject = new Subject(ValidOrgId, "  Математика  ", ValidCode, "#6366F1", null);

        subject.Name.ShouldBe("Математика");
    }

    [Test]
    public void GivenActiveSubject_WhenArchiving_ThenIsArchivedTrue()
    {
        var subject = CreateValidSubject();

        subject.Archive(Guid.Empty);

        subject.IsArchived.ShouldBeTrue();
    }

    [Test]
    public void GivenAlreadyArchivedSubject_WhenArchivingAgain_ThenNoOp()
    {
        var subject = CreateValidSubject();
        subject.Archive(Guid.Empty);
        var modifiedAt = subject.LastModifiedAt;

        subject.Archive(Guid.Empty); // повторный вызов

        subject.IsArchived.ShouldBeTrue();
        subject.LastModifiedAt.ShouldBe(modifiedAt);
    }

    [Test]
    public void GivenArchivedSubject_WhenRestoring_ThenIsArchivedFalse()
    {
        var subject = CreateValidSubject();
        subject.Archive(Guid.Empty);

        subject.Restore(Guid.Empty);

        subject.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenActiveSubject_WhenRestoringAgain_ThenNoOp()
    {
        var subject = CreateValidSubject();
        var modifiedAt = subject.LastModifiedAt;

        subject.Restore(Guid.Empty); // повторный вызов на активном

        subject.IsArchived.ShouldBeFalse();
        subject.LastModifiedAt.ShouldBe(modifiedAt);
    }

    [Test]
    public void GivenValidData_WhenUpdating_ThenAllFieldsChanged()
    {
        var subject = CreateValidSubject();
        var newCode = SubjectCode.From("PHYS");

        subject.Update("Физика", newCode, "#EF4444", "Описание физики", order: 5, by: Guid.Empty);

        subject.Name.ShouldBe("Физика");
        subject.Code.ShouldBe(newCode);
        subject.Color.ShouldBe("#EF4444");
        subject.Description.ShouldBe("Описание физики");
        subject.Order.ShouldBe(5);
    }

    [Test]
    public void GivenUpdate_WhenSameName_ThenLastModifiedAtStillUpdated()
    {
        var subject = CreateValidSubject();
        var beforeUpdate = subject.LastModifiedAt;

        subject.Update("Математика", ValidCode, "#6366F1", null, 0, by: Guid.Empty);

        // Touch() должен всегда вызываться в Update
        subject.LastModifiedAt.ShouldNotBe(beforeUpdate);
    }

    [Test]
    public void GivenUpdate_WhenInvalidColor_ThenThrowsArgumentException()
    {
        var subject = CreateValidSubject();

        var act = () => subject.Update("Математика", ValidCode, "bad-color", null, 0, Guid.Empty);

        act.ShouldThrow<ArgumentException>();
    }
}
