namespace Edvantix.Organizations.Domain.AggregatesModel.RoleAggregate;

/// <summary>
/// Repository for <see cref="Role"/> aggregate roots.
/// Use <see cref="IRepository{T}.UnitOfWork"/> to save changes.
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    /// <summary>Finds a role by its identifier, including its assigned permissions.</summary>
    Task<Role?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all active (non-deleted) roles for the given school, each with permissions loaded.
    /// Bypasses the tenant query filter — used from gRPC paths without ambient tenant context.
    /// </summary>
    Task<List<Role>> GetBySchoolAsync(Guid schoolId, CancellationToken cancellationToken = default);

    /// <summary>Returns all roles ordered by name. Tenant filter is applied by DbContext.</summary>
    Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds a new role to the context. Call <see cref="IUnitOfWork.SaveEntitiesAsync"/> to persist.</summary>
    Task<Role> AddAsync(Role role, CancellationToken cancellationToken = default);

    /// <summary>Marks a role for deletion. Call <see cref="IUnitOfWork.SaveEntitiesAsync"/> to persist.</summary>
    void Remove(Role role);
}
