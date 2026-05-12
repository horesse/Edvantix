namespace Edvantix.Curriculum.UnitTests.TestData;

internal static class CurriculumTestData
{
    public static readonly Guid OrganizationId = Guid.CreateVersion7();
    public static readonly Guid OtherOrganizationId = Guid.CreateVersion7();
    public static readonly Guid OwnerMemberId = Guid.CreateVersion7();

    public static Course CreateCourse(Guid? organizationId = null) =>
        new(
            organizationId ?? OrganizationId,
            "EN-GEN-B1",
            "English General B1",
            CourseSubject.English,
            "B1",
            durationWeeks: 12,
            OwnerMemberId,
            "General English course"
        );

    public static Course CreateCourseWithModule(out Module module, Guid? organizationId = null)
    {
        var course = CreateCourse(organizationId);
        module = course.AddModule("Module 1", "Summary", weeks: 2);
        return course;
    }

    public static Course CreateCourseWithLesson(
        out Module module,
        out Lesson lesson,
        Guid? organizationId = null
    )
    {
        var course = CreateCourseWithModule(out module, organizationId);
        lesson = course.AddLesson(module.Id, "Lesson 1", LessonType.Lecture, minutes: 45, []);
        return course;
    }
}
