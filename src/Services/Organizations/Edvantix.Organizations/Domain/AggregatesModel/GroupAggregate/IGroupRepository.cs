namespace Edvantix.Organizations.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Repository for <see cref="Group"/> aggregate roots.
/// Use <see cref="IRepository{T}.UnitOfWork"/> to save changes.
/// </summary>
public interface IGroupRepository : IRepository<Group>
{
    /// <summary>Finds a group by its identifier.</summary>
    Task<Group?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns groups matching the given specification. Use <see cref="GroupSpecification"/> for ordered list of all active groups.</summary>
    Task<List<Group>> ListAsync(Specification<Group> spec, CancellationToken cancellationToken = default);

    /// <summary>Returns the first group matching the specification, or null. Use <see cref="GroupByIdSpecification"/> to load with optional eager includes.</summary>
    Task<Group?> FindAsync(Specification<Group> spec, CancellationToken cancellationToken = default);

    /// <summary>Adds a new group to the context. Call <see cref="IUnitOfWork.SaveEntitiesAsync"/> to persist.</summary>
    void Add(Group group);

    /// <summary>Marks a group for deletion. Call <see cref="IUnitOfWork.SaveEntitiesAsync"/> to persist.</summary>
    void Remove(Group group);
}
