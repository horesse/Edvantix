using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Groups.Infrastructure.Repositories;

internal sealed class LevelRepository(GroupsDbContext context) : ILevelRepository
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

    /// <summary>
    /// Placeholder: всегда возвращает <c>false</c> до тех пор, пока агрегат Group
    /// не получит FK-ссылку на Level (Group.LevelId).
    /// </summary>
    public Task<bool> IsUsedByGroupsAsync(
        Guid levelId,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(false);

    public async Task AddAsync(Level level, CancellationToken cancellationToken = default) =>
        await context.Levels.AddAsync(level, cancellationToken);

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
        CancellationToken cancellationToken = default
    )
    {
        var normalizedCode = LevelCode.From(code).Value;

        // Загружаем только коды и проверяем в памяти — уровней в org мало, запрос лёгкий.
        var codes = await context
            .Levels.Where(l => l.OrganizationId == organizationId && !l.IsDeleted)
            .Select(l => l.Code)
            .ToListAsync(cancellationToken);

        return codes.Any(c => c.Value == normalizedCode);
    }
}
