using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Curriculum.Domain.Events;

/// <summary>Событие публикации курса (переход в статус Active).</summary>
public sealed class CoursePublishedDomainEvent(Guid courseId, Guid organizationId) : DomainEvent
{
    public Guid CourseId { get; } = courseId;
    public Guid OrganizationId { get; } = organizationId;
}
