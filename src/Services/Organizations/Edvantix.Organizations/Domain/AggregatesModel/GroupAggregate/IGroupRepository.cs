namespace Edvantix.Organizations.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Repository for <see cref="Group"/> aggregate roots.
/// Use <see cref="IRepository{T}.UnitOfWork"/> to save changes.
/// </summary>
public interface IGroupRepository : IRepository<Group>
{
    /// <summary>Finds a group by its identifier.</summary>
    Task<Group?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns all active (non-deleted) groups for the current tenant. Tenant filter is applied by DbContext.</summary>
    Task<List<Group>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds a new group to the context. Call <see cref="IUnitOfWork.SaveEntitiesAsync"/> to persist.</summary>
    void Add(Group group);

    /// <summary>Marks a group for deletion. Call <see cref="IUnitOfWork.SaveEntitiesAsync"/> to persist.</summary>
    void Remove(Group group);
}
