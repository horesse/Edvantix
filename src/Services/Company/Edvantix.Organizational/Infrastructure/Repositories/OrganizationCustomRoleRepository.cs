using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория для кастомных ролей организации на основе EF Core.
/// </summary>
public sealed class OrganizationCustomRoleRepository(OrganizationalDbContext context)
    : IOrganizationCustomRoleRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<OrganizationCustomRole?> FindByIdAsync(
        Guid id,
        Guid organizationId,
        CancellationToken ct = default
    ) =>
        await context
            .Set<OrganizationCustomRole>()
            .FirstOrDefaultAsync(
                r => r.Id == id && r.OrganizationId == organizationId && !r.IsDeleted,
                ct
            );

    /// <inheritdoc/>
    public async Task<OrganizationCustomRole?> FindByCodeAsync(
        Guid organizationId,
        string code,
        CancellationToken ct = default
    ) =>
        await context
            .Set<OrganizationCustomRole>()
            .FirstOrDefaultAsync(
                r => r.OrganizationId == organizationId && r.Code == code && !r.IsDeleted,
                ct
            );

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrganizationCustomRole>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken ct = default
    ) =>
        await context
            .Set<OrganizationCustomRole>()
            .Where(r => r.OrganizationId == organizationId && !r.IsDeleted)
            .OrderBy(r => r.Code)
            .ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<OrganizationCustomRole> AddAsync(
        OrganizationCustomRole role,
        CancellationToken ct = default
    )
    {
        var entry = await context.Set<OrganizationCustomRole>().AddAsync(role, ct);
        return entry.Entity;
    }

    /// <inheritdoc/>
    public Task<int> GetAssignedMembersCountAsync(Guid roleId, CancellationToken ct = default)
    {
        // TODO: Реализовать после создания таблицы назначений ролей участникам (следующая задача).
        // Сейчас возвращает 0, так как механизм назначения кастомных ролей ещё не реализован.
        return Task.FromResult(0);
    }
}
