namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

public interface IGroupMemberRepository : IRepository<GroupMember>
{
    Task<GroupMember?> FindAsync(ISpecification<GroupMember> spec, CancellationToken ct = default);
    Task<IReadOnlyList<GroupMember>> ListAsync(
        ISpecification<GroupMember> spec,
        CancellationToken ct = default
    );
    Task<int> CountAsync(ISpecification<GroupMember> spec, CancellationToken ct = default);
    Task<GroupMember?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<GroupMember> AddAsync(GroupMember entity, CancellationToken ct = default);
}
