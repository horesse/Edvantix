namespace Edvantix.Organizational.Domain.AggregatesModel.GroupMembershipHistoryAggregate;

public interface IGroupMembershipHistoryRepository : IRepository<GroupMembershipHistory>
{
    Task AddAsync(GroupMembershipHistory groupMembershipHistory, CancellationToken token = default);
}
