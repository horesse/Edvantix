using Edvantix.Organizational.Domain.AggregatesModel.GroupMembershipHistoryAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

public sealed class CreateGroupMemberDomainEventHandler(
    IGroupMembershipHistoryRepository repository
) : INotificationHandler<CreateGroupMemberDomainEvent>
{
    public async ValueTask Handle(
        CreateGroupMemberDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        await repository.AddAsync(
            new GroupMembershipHistory(notification.Member.Id, notification.JoinedAt),
            cancellationToken
        );
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
