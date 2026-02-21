namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

public interface IGroupRepository : IRepository<Group>
{
    Task<Group?> FindAsync(ISpecification<Group> spec, CancellationToken ct = default);
    Task<IReadOnlyList<Group>> ListAsync(
        ISpecification<Group> spec,
        CancellationToken ct = default
    );
    Task<int> CountAsync(ISpecification<Group> spec, CancellationToken ct = default);
    Task<Group?> FindByIdAsync(ulong id, CancellationToken ct = default);
    Task<Group> AddAsync(Group entity, CancellationToken ct = default);
}
