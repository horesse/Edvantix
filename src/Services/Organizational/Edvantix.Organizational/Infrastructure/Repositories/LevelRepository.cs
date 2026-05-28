using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class LevelRepository(OrganizationalDbContext context) : ILevelRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task<Level?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Levels.AsTracking()
            .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Levels.AnyAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);

    public async Task<bool> ExistsAsync(
        Guid id,
        Guid organizationId,
        bool requireActive,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Levels.Where(l =>
            l.Id == id && l.OrganizationId == organizationId && !l.IsDeleted
        );

        if (requireActive)
            query = query.Where(l => l.IsActive);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Level>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default
    )
    {
        var idList = ids.ToList();

        return await context
            .Levels.AsTracking()
            .Where(l => idList.Contains(l.Id) && !l.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Level level, CancellationToken cancellationToken = default) =>
        await context.Levels.AddAsync(level, cancellationToken);

    public Task AddRange(List<Level> levels, CancellationToken cancellationToken = default) =>
        context.Levels.AddRangeAsync(levels, cancellationToken);

    public async Task<IReadOnlyCollection<Level>> ListByOrganizationAsync(
        Guid organizationId,
        bool includeInactive = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Levels.Where(l => l.OrganizationId == organizationId && !l.IsDeleted);

        if (!includeInactive)
            query = query.Where(l => l.IsActive);

        return await query.OrderBy(l => l.SortOrder).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithCodeAsync(
        Guid organizationId,
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedCode = LevelCode.From(code).Value;

        // Загружаем коды в память — уровней в org мало, запрос лёгкий.
        var query = context.Levels.Where(l => l.OrganizationId == organizationId && !l.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(l => l.Id != excludeId.Value);

        var codes = await query
            .Select(l => l.Code)
            .ToListAsync(cancellationToken);

        return codes.Any(c => c.Value == normalizedCode);
    }

    public async Task<bool> ExistsWithNameAsync(
        Guid organizationId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    )
    {
        var trimmed = name.Trim();
        var query = context.Levels.Where(l =>
            l.OrganizationId == organizationId && !l.IsDeleted && l.IsActive && l.Name == trimmed
        );

        if (excludeId.HasValue)
            query = query.Where(l => l.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Level> Items, int Total)> ListForDirectoryAsync(
        Guid organizationId,
        bool includeInactive,
        string? search,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Levels.Where(l => l.OrganizationId == organizationId && !l.IsDeleted);

        if (!includeInactive)
            query = query.Where(l => l.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(l => l.Name.Contains(search.Trim()));

        var total = await query.CountAsync(cancellationToken);
        var offset = (pageIndex - 1) * pageSize;
        var items = await query
            .OrderBy(l => l.SortOrder)
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ([.. items], total);
    }

    public async Task<(int Active, int Archived)> GetStatsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    )
    {
        var activeCount = await context.Levels.CountAsync(
            l => l.OrganizationId == organizationId && !l.IsDeleted && l.IsActive,
            cancellationToken
        );

        var archivedCount = await context.Levels.CountAsync(
            l => l.OrganizationId == organizationId && !l.IsDeleted && !l.IsActive,
            cancellationToken
        );

        return (activeCount, archivedCount);
    }
}
