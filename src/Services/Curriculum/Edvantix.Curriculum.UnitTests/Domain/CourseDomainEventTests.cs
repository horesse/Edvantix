namespace Edvantix.Curriculum.UnitTests.Domain;

public sealed class CourseDomainEventTests
{
    [Test]
    public void GivenValidData_WhenCreatingCourse_ThenShouldRegisterCourseCreatedDomainEvent()
    {
        var course = CurriculumTestData.CreateCourse();

        var @event = course.DomainEvents.Single().ShouldBeOfType<CourseCreatedDomainEvent>();
        @event.CourseId.ShouldBe(course.Id);
        @event.OrganizationId.ShouldBe(course.OrganizationId);
        @event.OwnerMemberId.ShouldBe(course.OwnerMemberId);
    }

    [Test]
    public void GivenDraftCourse_WhenPublishing_ThenShouldRegisterCoursePublishedDomainEvent()
    {
        var course = CurriculumTestData.CreateCourse();
        course.ClearDomainEvents();

        course.Publish();

        var @event = course.DomainEvents.Single().ShouldBeOfType<CoursePublishedDomainEvent>();
        @event.CourseId.ShouldBe(course.Id);
        @event.OrganizationId.ShouldBe(course.OrganizationId);
    }

    [Test]
    public void GivenDraftCourse_WhenArchiving_ThenShouldRegisterCourseArchivedDomainEvent()
    {
        var course = CurriculumTestData.CreateCourse();
        course.ClearDomainEvents();

        course.Archive();

        var @event = course.DomainEvents.Single().ShouldBeOfType<CourseArchivedDomainEvent>();
        @event.CourseId.ShouldBe(course.Id);
        @event.OrganizationId.ShouldBe(course.OrganizationId);
    }

    [Test]
    public void GivenDraftLesson_WhenPublishingLesson_ThenShouldRegisterLessonPublishedDomainEvent()
    {
        var course = CurriculumTestData.CreateCourseWithLesson(out var module, out var lesson);
        course.ClearDomainEvents();

        course.PublishLesson(lesson.Id);

        var @event = course.DomainEvents.Single().ShouldBeOfType<LessonPublishedDomainEvent>();
        @event.CourseId.ShouldBe(course.Id);
        @event.ModuleId.ShouldBe(module.Id);
        @event.LessonId.ShouldBe(lesson.Id);
    }
}
