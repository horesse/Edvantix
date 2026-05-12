using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Curriculum.Domain.Events;

/// <summary>Событие публикации урока.</summary>
public sealed class LessonPublishedDomainEvent(Guid courseId, Guid moduleId, Guid lessonId)
    : DomainEvent
{
    public Guid CourseId { get; } = courseId;
    public Guid ModuleId { get; } = moduleId;
    public Guid LessonId { get; } = lessonId;
}
