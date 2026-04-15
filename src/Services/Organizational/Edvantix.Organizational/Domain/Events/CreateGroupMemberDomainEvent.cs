using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

public sealed class CreateGroupMemberDomainEvent(GroupMember member, DateOnly joinedAt)
    : DomainEvent
{
    public GroupMember Member { get; } = member;
    public DateOnly JoinedAt { get; } = joinedAt;
}
