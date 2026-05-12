using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

namespace Edvantix.Curriculum.Features.Courses;

internal sealed class CourseDomainToDtoMapper : Mapper<Course, CourseDto>
{
    public override CourseDto Map(Course source) =>
        new(
            source.Id,
            source.Code,
            source.Name,
            source.Subject,
            source.Level,
            source.DurationWeeks,
            source.CoverInitials,
            source.Status,
            source.TotalLessons
        );
}

internal sealed class CourseDetailDomainToDtoMapper : Mapper<Course, CourseDetailDto>
{
    public override CourseDetailDto Map(Course source) =>
        new(
            source.Id,
            source.Code,
            source.Name,
            source.Subject,
            source.Level,
            source.DurationWeeks,
            source.Description,
            source.CoverInitials,
            source.Status,
            source.OwnerMemberId,
            [.. source.Goals.Select(g => new CourseGoalDto(g.Id, g.Position, g.Text))],
            [
                .. source.Modules.Select(m => new ModuleDetailDto(
                    m.Id,
                    m.Position,
                    m.Name,
                    m.Summary,
                    m.Weeks,
                    [
                        .. m.Lessons.Select(l => new LessonDto(
                            l.Id,
                            l.Position,
                            l.Title,
                            l.Type,
                            l.Status,
                            l.Minutes
                        )),
                    ]
                )),
            ]
        );
}
