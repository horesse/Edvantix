using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;

namespace Edvantix.Groups.Infrastructure.Repositories;

internal sealed class SubjectRepository(GroupsDbContext context) : ISubjectRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task<Subject?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Subjects.AsTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task AddAsync(Subject subject, CancellationToken cancellationToken = default) =>
        await context.Subjects.AddAsync(subject, cancellationToken);

    public async Task<IReadOnlyList<Subject>> ListAsync(
        Guid organizationId,
        string? search,
        bool includeArchived,
        int offset,
        int size,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Subjects.Where(s => s.OrganizationId == organizationId);

        if (!includeArchived)
            query = query.Where(s => !s.IsArchived);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Name.Contains(search));

        return await query
            .OrderBy(s => s.Order)
            .ThenBy(s => s.Name)
            .Skip(offset)
            .Take(size)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> CountAsync(
        Guid organizationId,
        string? search,
        bool includeArchived,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Subjects.Where(s => s.OrganizationId == organizationId);

        if (!includeArchived)
            query = query.Where(s => !s.IsArchived);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Name.Contains(search));

        return await query.LongCountAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithCodeAsync(
        Guid organizationId,
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedCode = SubjectCode.From(code).Value;

        // Загружаем коды активных предметов в память для сравнения (предметов мало, запрос лёгкий)
        var entries = await context
            .Subjects.Where(s => s.OrganizationId == organizationId && !s.IsArchived)
            .Select(s => new { s.Id, s.Code })
            .ToListAsync(cancellationToken);

        return entries.Any(e =>
            e.Code.Value == normalizedCode && (!excludeId.HasValue || e.Id != excludeId.Value)
        );
    }

    public async Task<bool> ExistsWithNameAsync(
        Guid organizationId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    )
    {
        var trimmed = name.Trim();

        var query = context.Subjects.Where(s =>
            s.OrganizationId == organizationId
            && !s.IsArchived
            && s.Name == trimmed
        );

        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<(int ActiveCount, int ArchivedCount, DateTime? LastModifiedAt)> GetStatsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    )
    {
        var subjects = await context
            .Subjects.Where(s => s.OrganizationId == organizationId)
            .Select(s => new { s.IsArchived, s.LastModifiedAt })
            .ToListAsync(cancellationToken);

        var activeCount = subjects.Count(s => !s.IsArchived);
        var archivedCount = subjects.Count(s => s.IsArchived);
        var lastModifiedAt = subjects
            .Where(s => s.LastModifiedAt.HasValue)
            .Max(s => s.LastModifiedAt);

        return (activeCount, archivedCount, lastModifiedAt);
    }
}
