namespace Edvantix.Curriculum.UnitTests.Domain;

public sealed class CourseAggregateTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();
    private static readonly Guid ValidOwnerId = Guid.CreateVersion7();

    private static Course CreateValidCourse() =>
        new(
            ValidOrgId,
            "en-gen-b1",
            "Английский B1 — General",
            CourseSubject.English,
            "B1",
            durationWeeks: 12,
            ValidOwnerId,
            "Общий курс английского языка уровня B1"
        );

    [Test]
    public void GivenValidData_WhenCreatingCourse_ThenShouldInitializePropertiesCorrectly()
    {
        var course = CreateValidCourse();

        course.OrganizationId.ShouldBe(ValidOrgId);
        course.Code.ShouldBe("EN-GEN-B1");
        course.Name.ShouldBe("Английский B1 — General");
        course.Subject.ShouldBe(CourseSubject.English);
        course.Level.ShouldBe("B1");
        course.DurationWeeks.ShouldBe((short)12);
        course.OwnerMemberId.ShouldBe(ValidOwnerId);
        course.Description.ShouldBe("Общий курс английского языка уровня B1");
        course.Status.ShouldBe(CourseStatus.Draft);
        course.IsDeleted.ShouldBeFalse();
        course.Id.ShouldNotBe(Guid.Empty);
    }

    [Test]
    public void GivenCodeWithSpacesAndLowercase_WhenCreatingCourse_ThenCodeShouldBeTrimmedAndUppercased()
    {
        var course = new Course(
            ValidOrgId,
            "  en-gen-b1  ",
            "Курс",
            CourseSubject.English,
            "B1",
            durationWeeks: 8,
            ValidOwnerId
        );

        course.Code.ShouldBe("EN-GEN-B1");
    }

    [Test]
    public void GivenNameWithSpaces_WhenCreatingCourse_ThenNameShouldBeTrimmed()
    {
        var course = new Course(
            ValidOrgId,
            "EN-B1",
            "  Английский  ",
            CourseSubject.English,
            "B1",
            durationWeeks: 8,
            ValidOwnerId
        );

        course.Name.ShouldBe("Английский");
    }

    [Test]
    public void GivenDescriptionWithSpaces_WhenCreatingCourse_ThenDescriptionShouldBeTrimmed()
    {
        var course = new Course(
            ValidOrgId,
            "EN-B1",
            "Курс",
            CourseSubject.English,
            "B1",
            durationWeeks: 8,
            ValidOwnerId,
            "  Описание  "
        );

        course.Description.ShouldBe("Описание");
    }

    [Test]
    public void GivenNullDescription_WhenCreatingCourse_ThenDescriptionShouldBeNull()
    {
        var course = new Course(
            ValidOrgId,
            "EN-B1",
            "Курс",
            CourseSubject.English,
            "B1",
            durationWeeks: 8,
            ValidOwnerId
        );

        course.Description.ShouldBeNull();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingCourse_ThenShouldThrowArgumentException()
    {
        var act = () => new Course(
            Guid.Empty,
            "EN-B1",
            "Курс",
            CourseSubject.English,
            "B1",
            durationWeeks: 8,
            ValidOwnerId
        );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyOwnerMemberId_WhenCreatingCourse_ThenShouldThrowArgumentException()
    {
        var act = () => new Course(
            ValidOrgId,
            "EN-B1",
            "Курс",
            CourseSubject.English,
            "B1",
            durationWeeks: 8,
            Guid.Empty
        );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceCode_WhenCreatingCourse_ThenShouldThrowArgumentException(
        string? code
    )
    {
        var act = () => new Course(
            ValidOrgId,
            code!,
            "Курс",
            CourseSubject.English,
            "B1",
            durationWeeks: 8,
            ValidOwnerId
        );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenCreatingCourse_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var act = () => new Course(
            ValidOrgId,
            "EN-B1",
            name!,
            CourseSubject.English,
            "B1",
            durationWeeks: 8,
            ValidOwnerId
        );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceLevel_WhenCreatingCourse_ThenShouldThrowArgumentException(
        string? level
    )
    {
        var act = () => new Course(
            ValidOrgId,
            "EN-B1",
            "Курс",
            CourseSubject.English,
            level!,
            durationWeeks: 8,
            ValidOwnerId
        );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    [Arguments((short)-100)]
    public void GivenZeroOrNegativeDurationWeeks_WhenCreatingCourse_ThenShouldThrowArgumentException(
        short durationWeeks
    )
    {
        var act = () => new Course(
            ValidOrgId,
            "EN-B1",
            "Курс",
            CourseSubject.English,
            "B1",
            durationWeeks,
            ValidOwnerId
        );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenDraftCourse_WhenPublishing_ThenStatusShouldBeActive()
    {
        var course = CreateValidCourse();

        course.Publish();

        course.Status.ShouldBe(CourseStatus.Active);
        course.IsDeleted.ShouldBeFalse();
    }

    [Test]
    public void GivenDeletedCourse_WhenPublishing_ThenShouldThrowInvalidOperationException()
    {
        var course = CreateValidCourse();
        course.Delete();

        var act = () => course.Publish();

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenActiveCourse_WhenPublishingAgain_ThenStatusShouldRemainActive()
    {
        var course = CreateValidCourse();
        course.Publish();

        course.Publish();

        course.Status.ShouldBe(CourseStatus.Active);
    }

    [Test]
    public void GivenActiveCourse_WhenDeleting_ThenShouldMarkAsDeletedAndSetArchivedStatus()
    {
        var course = CreateValidCourse();
        course.Publish();

        course.Delete();

        course.IsDeleted.ShouldBeTrue();
        course.Status.ShouldBe(CourseStatus.Archived);
    }

    [Test]
    public void GivenDraftCourse_WhenDeleting_ThenShouldMarkAsDeletedAndSetArchivedStatus()
    {
        var course = CreateValidCourse();

        course.Delete();

        course.IsDeleted.ShouldBeTrue();
        course.Status.ShouldBe(CourseStatus.Archived);
    }

    [Test]
    public void GivenNewCourse_WhenCreated_ThenCreatedAtAndUpdatedAtShouldBeSet()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var course = CreateValidCourse();

        course.CreatedAt.ShouldBeGreaterThan(before);
        course.UpdatedAt.ShouldBe(course.CreatedAt);
    }

    [Test]
    public void GivenPublishedCourse_WhenPublishing_ThenUpdatedAtShouldBeRefreshed()
    {
        var course = CreateValidCourse();
        var createdAt = course.CreatedAt;

        course.Publish();

        course.UpdatedAt.ShouldBeGreaterThanOrEqualTo(createdAt);
    }

    [Test]
    public void GivenDeletedCourse_WhenDeleting_ThenUpdatedAtShouldBeRefreshed()
    {
        var course = CreateValidCourse();
        var createdAt = course.CreatedAt;

        course.Delete();

        course.UpdatedAt.ShouldBeGreaterThanOrEqualTo(createdAt);
    }

    [Test]
    [Arguments(CourseSubject.English)]
    [Arguments(CourseSubject.Math)]
    [Arguments(CourseSubject.Kids)]
    [Arguments(CourseSubject.Exam)]
    [Arguments(CourseSubject.Speaking)]
    public void GivenAnySubject_WhenCreatingCourse_ThenSubjectShouldBeSet(CourseSubject subject)
    {
        var course = new Course(
            ValidOrgId,
            "SUBJ-01",
            "Курс",
            subject,
            "A1",
            durationWeeks: 4,
            ValidOwnerId
        );

        course.Subject.ShouldBe(subject);
    }
}
