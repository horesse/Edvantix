using Edvantix.Organizations.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizations.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IPermissionRepository"/>.
/// Permissions are global (no tenant filter) so queries run against the full catalogue.
/// </summary>
public sealed class PermissionRepository(OrganizationsDbContext context) : IPermissionRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<List<Permission>> GetByNamesAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken = default
    ) =>
        await context.Permissions.Where(p => names.Contains(p.Name)).ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<List<Permission>> GetAllAsync(
        CancellationToken cancellationToken = default
    ) => await context.Permissions.OrderBy(p => p.Name).ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task UpsertAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken = default
    )
    {
        // Materialise names to avoid multiple enumerations across the two operations.
        var nameList = names.ToList();

        var existingNames = await context
            .Permissions.Where(p => nameList.Contains(p.Name))
            .Select(p => p.Name)
            .ToListAsync(cancellationToken);

        // Only insert permissions that are not already in the catalogue.
        var newNames = nameList.Except(existingNames);

        foreach (var name in newNames)
        {
            await context.Permissions.AddAsync(new Permission(name), cancellationToken);
        }
    }
}
