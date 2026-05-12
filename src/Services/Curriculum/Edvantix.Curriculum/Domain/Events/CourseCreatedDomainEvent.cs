using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Curriculum.Domain.Events;

/// <summary>Событие создания нового курса.</summary>
public sealed class CourseCreatedDomainEvent(Guid courseId, Guid organizationId, Guid ownerMemberId)
    : DomainEvent
{
    public Guid CourseId { get; } = courseId;
    public Guid OrganizationId { get; } = organizationId;
    public Guid OwnerMemberId { get; } = ownerMemberId;
}
