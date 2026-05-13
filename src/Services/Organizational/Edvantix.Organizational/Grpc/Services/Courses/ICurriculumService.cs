using Edvantix.Curriculum.Grpc.Services;

namespace Edvantix.Organizational.Grpc.Services.Courses;

public interface ICurriculumService
{
    /// <summary>
    /// Validates that a course exists and belongs to the given organization.
    /// Returns <c>null</c> if the course is not found.
    /// </summary>
    Task<CourseInfo?> GetCourseByIdAsync(
        [StringSyntax(StringSyntaxAttribute.GuidFormat)] string courseId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Returns lightweight course options for the given organization (for dropdowns).
    /// </summary>
    Task<IReadOnlyList<CourseOption>> GetCoursesForOrganizationAsync(
        [StringSyntax(StringSyntaxAttribute.GuidFormat)] string organizationId,
        string? search = null,
        CancellationToken cancellationToken = default
    );
}
