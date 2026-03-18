using Edvantix.Organizations.Domain.AggregatesModel.RoleAggregate;

namespace Edvantix.Organizations.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IRoleRepository"/>.
/// Eagerly loads the <see cref="Role.Permissions"/> navigation on every find so that
/// permission assignment operations are immediately available on the returned aggregate.
/// </summary>
public sealed class RoleRepository(OrganizationsDbContext context) : IRoleRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<Role?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    /// <inheritdoc/>
    public async Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Roles.OrderBy(r => r.Name).ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Role> AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        var entry = await context.Roles.AddAsync(role, cancellationToken);
        return entry.Entity;
    }

    /// <inheritdoc/>
    public void Remove(Role role) => context.Roles.Remove(role);
}
