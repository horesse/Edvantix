using Edvantix.Audit.Domain.Enums;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Audit.Domain.Events;

/// <summary>
/// Доменное событие, публикуемое при создании новой записи в журнале аудита.
/// </summary>
public sealed class AuditEntryCreatedDomainEvent(
    Guid auditEntryId,
    Guid organizationId,
    Guid actorId,
    AuditAction action,
    AuditEntityType entityType
) : DomainEvent
{
    public Guid AuditEntryId { get; } = auditEntryId;
    public Guid OrganizationId { get; } = organizationId;
    public Guid ActorId { get; } = actorId;
    public AuditAction Action { get; } = action;
    public AuditEntityType EntityType { get; } = entityType;
}
