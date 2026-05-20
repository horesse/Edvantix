using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Groups.Domain.Events;

/// <summary>Вызывается при создании новой учебной группы.</summary>
public sealed class GroupCreatedDomainEvent(Guid groupId, Guid organizationId, DateOnly startDate)
    : DomainEvent
{
    public Guid GroupId { get; } = groupId;
    public Guid OrganizationId { get; } = organizationId;
    public DateOnly StartDate { get; } = startDate;
}
