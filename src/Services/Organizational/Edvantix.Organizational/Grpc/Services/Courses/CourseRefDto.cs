namespace Edvantix.Organizational.Grpc.Services.Courses;

/// <summary>Краткая информация о курсе, возвращаемая batch-эндпойнтом GetCoursesByIds.</summary>
/// <param name="Id">Идентификатор курса.</param>
/// <param name="Code">Уникальный код курса (напр. <c>EN-GEN-B1</c>).</param>
/// <param name="Name">Наименование курса.</param>
public sealed record CourseRefDto(Guid Id, string Code, string Name);
