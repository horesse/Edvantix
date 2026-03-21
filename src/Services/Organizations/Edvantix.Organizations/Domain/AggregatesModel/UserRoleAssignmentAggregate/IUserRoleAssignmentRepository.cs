namespace Edvantix.Organizations.Domain.AggregatesModel.UserRoleAssignmentAggregate;

/// <summary>
/// Repository for <see cref="UserRoleAssignment"/> aggregate roots.
/// Tenant filter is applied by DbContext — only assignments for the current school are returned.
/// </summary>
public interface IUserRoleAssignmentRepository : IRepository<UserRoleAssignment>
{
    /// <summary>Finds an assignment for the given profile and role combination.</summary>
    Task<UserRoleAssignment?> FindAsync(
        Guid profileId,
        Guid roleId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Returns all role assignments for the specified profile within the current tenant.</summary>
    Task<List<UserRoleAssignment>> GetByProfileIdAsync(
        Guid profileId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Returns all role assignments for the given profile in the specified school,
    /// bypassing the tenant query filter. Used for gRPC calls that do not pass through
    /// TenantMiddleware and therefore have no ambient tenant context.
    /// </summary>
    Task<List<UserRoleAssignment>> GetByProfileAndSchoolAsync(
        Guid profileId,
        Guid schoolId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Returns all role assignments for the given role across any school,
    /// bypassing the tenant query filter. Used to enumerate affected users when
    /// a role's permissions change, so their caches can be invalidated.
    /// </summary>
    Task<List<UserRoleAssignment>> GetAllByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Returns true if any assignment references the given role — used before role deletion.</summary>
    Task<bool> ExistsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new assignment to the context. Call <see cref="IUnitOfWork.SaveEntitiesAsync"/> to persist.</summary>
    Task<UserRoleAssignment> AddAsync(
        UserRoleAssignment assignment,
        CancellationToken cancellationToken = default
    );

    /// <summary>Marks an assignment for deletion. Call <see cref="IUnitOfWork.SaveEntitiesAsync"/> to persist.</summary>
    void Remove(UserRoleAssignment assignment);
}
