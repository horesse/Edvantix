namespace Edvantix.Organizations.Domain.AggregatesModel.PermissionAggregate;

/// <summary>
/// Repository for the global <see cref="Permission"/> catalogue.
/// Permissions are not tenant-scoped — all schools share the same permission strings.
/// </summary>
public interface IPermissionRepository : IRepository<Permission>
{
    /// <summary>Returns permissions whose names are in the provided set.</summary>
    Task<List<Permission>> GetByNamesAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken = default
    );

    /// <summary>Returns all permissions ordered by name.</summary>
    Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts permissions for names that do not already exist in the catalogue.
    /// Existing names are skipped — this operation is idempotent.
    /// </summary>
    Task UpsertAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);
}
