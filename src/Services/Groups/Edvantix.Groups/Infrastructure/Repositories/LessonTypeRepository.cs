using Edvantix.Groups.Domain.LessonTypeAggregate;

namespace Edvantix.Groups.Infrastructure.Repositories;

internal sealed class LessonTypeRepository(GroupsDbContext context) : ILessonTypeRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task<LessonType?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .LessonTypes.AsTracking()
            .FirstOrDefaultAsync(lt => lt.Id == id, cancellationToken);

    public async Task AddAsync(LessonType lessonType, CancellationToken cancellationToken = default) =>
        await context.LessonTypes.AddAsync(lessonType, cancellationToken);

    public async Task<bool> NameExistsAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.LessonTypes.Where(lt =>
            lt.OrganizationId == organizationId
            && !lt.IsArchived
            && lt.Name == name
        );

        if (excludeId.HasValue)
            query = query.Where(lt => lt.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(
        Guid organizationId,
        string code,
        Guid? excludeId,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.LessonTypes.Where(lt =>
            lt.OrganizationId == organizationId
            && !lt.IsArchived
            && lt.Code == code
        );

        if (excludeId.HasValue)
            query = query.Where(lt => lt.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<LessonType> Items, int TotalCount)> ListAsync(
        Guid organizationId,
        bool includeArchived,
        string? search,
        int offset,
        int limit,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.LessonTypes.Where(lt => lt.OrganizationId == organizationId);

        if (!includeArchived)
            query = query.Where(lt => !lt.IsArchived);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(lt => lt.Name.Contains(search));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(lt => lt.Order)
            .ThenBy(lt => lt.Name)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(int ActiveCount, int ArchivedCount, DateTime? LastModifiedAt)> GetStatsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await context
            .LessonTypes.Where(lt => lt.OrganizationId == organizationId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                ActiveCount = g.Count(lt => !lt.IsArchived),
                ArchivedCount = g.Count(lt => lt.IsArchived),
                LastModifiedAt = g.Max(lt => lt.LastModifiedAt),
            })
            .FirstOrDefaultAsync(cancellationToken);

        return data is null
            ? (0, 0, null)
            : (data.ActiveCount, data.ArchivedCount, data.LastModifiedAt);
    }
}
