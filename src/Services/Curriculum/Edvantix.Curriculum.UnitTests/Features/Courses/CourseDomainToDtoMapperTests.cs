namespace Edvantix.Curriculum.UnitTests.Features.Courses;

public sealed class CourseDomainToDtoMapperTests
{
    private readonly CourseDomainToDtoMapper _mapper = new();
    private readonly CourseDetailDomainToDtoMapper _detailMapper = new();

    [Test]
    public void GivenCourse_WhenMapping_ThenShouldMapAllFields()
    {
        var course = CurriculumTestData.CreateCourse();

        var dto = _mapper.Map(course);

        dto.Id.ShouldBe(course.Id);
        dto.Code.ShouldBe(course.Code);
        dto.Name.ShouldBe(course.Name);
        dto.Subject.ShouldBe(course.Subject);
        dto.Level.ShouldBe(course.Level);
        dto.DurationWeeks.ShouldBe(course.DurationWeeks);
        dto.CoverInitials.ShouldBe(course.CoverInitials);
        dto.Status.ShouldBe(course.Status);
        dto.TotalLessons.ShouldBe(0);
    }

    [Test]
    public void GivenCourseWithLessons_WhenMapping_ThenTotalLessonsShouldReflectCount()
    {
        var course = CurriculumTestData.CreateCourseWithLesson(out _, out _);

        var dto = _mapper.Map(course);

        dto.TotalLessons.ShouldBe(1);
    }

    [Test]
    public void GivenCourse_WhenMappingDetail_ThenShouldMapAllScalarFields()
    {
        var course = CurriculumTestData.CreateCourse();

        var dto = _detailMapper.Map(course);

        dto.Id.ShouldBe(course.Id);
        dto.Code.ShouldBe(course.Code);
        dto.Name.ShouldBe(course.Name);
        dto.Subject.ShouldBe(course.Subject);
        dto.Level.ShouldBe(course.Level);
        dto.DurationWeeks.ShouldBe(course.DurationWeeks);
        dto.Description.ShouldBe(course.Description);
        dto.CoverInitials.ShouldBe(course.CoverInitials);
        dto.Status.ShouldBe(course.Status);
        dto.OwnerMemberId.ShouldBe(course.OwnerMemberId);
    }

    [Test]
    public void GivenCourseWithNoModules_WhenMappingDetail_ThenModulesShouldBeEmpty()
    {
        var course = CurriculumTestData.CreateCourse();

        var dto = _detailMapper.Map(course);

        dto.Modules.ShouldBeEmpty();
        dto.Goals.ShouldBeEmpty();
    }

    [Test]
    public void GivenCourseWithModuleAndLesson_WhenMappingDetail_ThenShouldMapModulesAndLessons()
    {
        var course = CurriculumTestData.CreateCourseWithLesson(out var module, out var lesson);

        var dto = _detailMapper.Map(course);

        dto.Modules.ShouldHaveSingleItem();
        var moduleDto = dto.Modules[0];
        moduleDto.Id.ShouldBe(module.Id);
        moduleDto.Name.ShouldBe(module.Name);
        moduleDto.Position.ShouldBe(module.Position);
        moduleDto.Weeks.ShouldBe(module.Weeks);
        moduleDto.Lessons.ShouldHaveSingleItem();
        moduleDto.Lessons[0].Id.ShouldBe(lesson.Id);
        moduleDto.Lessons[0].Title.ShouldBe(lesson.Title);
        moduleDto.Lessons[0].Type.ShouldBe(lesson.Type);
        moduleDto.Lessons[0].Status.ShouldBe(lesson.Status);
        moduleDto.Lessons[0].Minutes.ShouldBe(lesson.Minutes);
    }
}
