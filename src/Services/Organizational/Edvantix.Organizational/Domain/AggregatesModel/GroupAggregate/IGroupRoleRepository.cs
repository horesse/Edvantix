namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

public interface IGroupRoleRepository : IRepository<GroupRole>
{
    Task AddRangeAsync(
        IReadOnlyList<GroupRole> roles,
        CancellationToken cancellationToken = default
    );
}
