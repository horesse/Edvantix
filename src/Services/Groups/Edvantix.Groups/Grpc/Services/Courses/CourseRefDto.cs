namespace Edvantix.Groups.Grpc.Services.Courses;

/// <summary>Краткая информация о курсе, возвращаемая batch-эндпойнтом GetCoursesByIds.</summary>
public sealed record CourseRefDto(Guid Id, string Code, string Name);
