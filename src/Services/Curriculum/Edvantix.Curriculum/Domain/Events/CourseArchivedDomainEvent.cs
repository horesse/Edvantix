using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Curriculum.Domain.Events;

/// <summary>
/// Событие архивации курса.
/// Используется для уведомления Organizational-сервиса о том,
/// что курс больше недоступен для привязки к новым группам.
/// </summary>
public sealed class CourseArchivedDomainEvent(Guid courseId, Guid organizationId) : DomainEvent
{
    public Guid CourseId { get; } = courseId;
    public Guid OrganizationId { get; } = organizationId;
}
