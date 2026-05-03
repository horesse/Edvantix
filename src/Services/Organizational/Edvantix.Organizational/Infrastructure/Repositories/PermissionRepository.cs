using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class PermissionRepository(OrganizationalDbContext context) : IPermissionRepository
{
    public IUnitOfWork UnitOfWork => context;

    public Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default) =>
        context.Permissions.AsTracking().ToListAsync(cancellationToken);

    public Task<List<Permission>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default
    ) =>
        context
            .Permissions.AsTracking()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);

    public Task<List<Permission>> GetAllWithFeaturesAsync(
        CancellationToken cancellationToken = default
    ) => context.Permissions.AsNoTracking().Include(p => p.Feature).ToListAsync(cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        context.Permissions.AsNoTracking().CountAsync(cancellationToken);

    public void Add(Permission permission) => context.Permissions.Add(permission);

    public void Remove(Permission permission) => context.Permissions.Remove(permission);
}
