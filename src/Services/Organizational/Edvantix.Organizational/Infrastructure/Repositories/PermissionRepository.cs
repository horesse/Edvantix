using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class PermissionRepository(OrganizationalDbContext context) : IPermissionRepository
{
    public IUnitOfWork UnitOfWork => context;

    public Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default) =>
        context.Permissions.ToListAsync(cancellationToken);

    public void Add(Permission permission) => context.Permissions.Add(permission);

    public void Remove(Permission permission) => context.Permissions.Remove(permission);
}
