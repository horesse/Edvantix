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
        var act = () =>
            new Course(
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
        var act = () =>
            new Course(
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
        var act = () =>
            new Course(
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
        var act = () =>
            new Course(
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
        var act = () =>
            new Course(
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
        var act = () =>
            new Course(
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
        course.LastModifiedAt.ShouldBe(course.CreatedAt);
    }

    [Test]
    public void GivenPublishedCourse_WhenPublishing_ThenLastModifiedAtShouldBeRefreshed()
    {
        var course = CreateValidCourse();
        var createdAt = course.CreatedAt;

        course.Publish();

        course.LastModifiedAt!.Value.ShouldBeGreaterThanOrEqualTo(createdAt);
    }

    [Test]
    public void GivenDeletedCourse_WhenDeleting_ThenLastModifiedAtShouldBeRefreshed()
    {
        var course = CreateValidCourse();
        var createdAt = course.CreatedAt;

        course.Delete();

        course.LastModifiedAt!.Value.ShouldBeGreaterThanOrEqualTo(createdAt);
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

    // ─── Archive ──────────────────────────────────────────────────────────────

    [Test]
    public void GivenActiveCourse_WhenArchiving_ThenStatusShouldBeArchived()
    {
        var course = CreateValidCourse();
        course.Publish();

        course.Archive();

        course.Status.ShouldBe(CourseStatus.Archived);
        course.IsDeleted.ShouldBeFalse();
    }

    [Test]
    public void GivenDeletedCourse_WhenArchiving_ThenShouldThrowInvalidOperationException()
    {
        var course = CreateValidCourse();
        course.Delete();

        var act = () => course.Archive();

        act.ShouldThrow<InvalidOperationException>();
    }

    // ─── Update ───────────────────────────────────────────────────────────────

    [Test]
    public void GivenValidData_WhenUpdatingCourse_ThenShouldUpdateAllFields()
    {
        var course = CreateValidCourse();

        course.Update("Новое название", "Новое описание", "B2", 16, "EN");

        course.Name.ShouldBe("Новое название");
        course.Description.ShouldBe("Новое описание");
        course.Level.ShouldBe("B2");
        course.DurationWeeks.ShouldBe((short)16);
        course.CoverInitials.ShouldBe("EN");
    }

    [Test]
    public void GivenNullDescription_WhenUpdatingCourse_ThenDescriptionShouldBeNull()
    {
        var course = CreateValidCourse();

        course.Update("Название", null, "B1", 12);

        course.Description.ShouldBeNull();
    }

    [Test]
    public void GivenNullCoverInitials_WhenUpdatingCourse_ThenCoverInitialsShouldBeNull()
    {
        var course = CreateValidCourse();

        course.Update("Название", null, "B1", 12, null);

        course.CoverInitials.ShouldBeNull();
    }

    [Test]
    public void GivenDeletedCourse_WhenUpdating_ThenShouldThrowInvalidOperationException()
    {
        var course = CreateValidCourse();
        course.Delete();

        var act = () => course.Update("Название", null, "B1", 12);

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenUpdatingCourse_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var course = CreateValidCourse();

        var act = () => course.Update(name!, null, "B1", 12);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceLevel_WhenUpdatingCourse_ThenShouldThrowArgumentException(
        string? level
    )
    {
        var course = CreateValidCourse();

        var act = () => course.Update("Название", null, level!, 12);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    public void GivenZeroOrNegativeDurationWeeks_WhenUpdatingCourse_ThenShouldThrowArgumentException(
        short durationWeeks
    )
    {
        var course = CreateValidCourse();

        var act = () => course.Update("Название", null, "B1", durationWeeks);

        act.ShouldThrow<ArgumentException>();
    }

    // ─── AddGoal ──────────────────────────────────────────────────────────────

    [Test]
    public void GivenValidCourse_WhenAddingGoal_ThenGoalShouldBeCreatedWithCorrectPosition()
    {
        var course = CreateValidCourse();

        var goal1 = course.AddGoal("Первая цель");
        var goal2 = course.AddGoal("Вторая цель");

        course.Goals.Count.ShouldBe(2);
        goal1.Position.ShouldBe((short)1);
        goal1.Text.ShouldBe("Первая цель");
        goal2.Position.ShouldBe((short)2);
    }

    [Test]
    public void GivenDeletedCourse_WhenAddingGoal_ThenShouldThrowInvalidOperationException()
    {
        var course = CreateValidCourse();
        course.Delete();

        var act = () => course.AddGoal("Цель");

        act.ShouldThrow<InvalidOperationException>();
    }

    // ─── ReorderModules (additional) ──────────────────────────────────────────

    [Test]
    public void GivenDeletedCourse_WhenReorderingModules_ThenShouldThrowInvalidOperationException()
    {
        var course = CreateValidCourse();
        var m1 = course.AddModule("Модуль 1", null, weeks: 2);
        course.Delete();

        var act = () => course.ReorderModules([m1.Id]);

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenUnknownModuleId_WhenReorderingModules_ThenShouldThrowArgumentException()
    {
        var course = CreateValidCourse();
        course.AddModule("Модуль 1", null, weeks: 2);

        var act = () => course.ReorderModules([Guid.CreateVersion7()]);

        act.ShouldThrow<ArgumentException>();
    }
}
