using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.LessonTypeAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class LessonTypeRepository(OrganizationalDbContext context) : ILessonTypeRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<LessonType?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .LessonTypes.AsTracking()
            .FirstOrDefaultAsync(lt => lt.Id == id, cancellationToken);

    public async Task AddAsync(
        LessonType lessonType,
        CancellationToken cancellationToken = default
    ) => await context.LessonTypes.AddAsync(lessonType, cancellationToken);

    public async Task<IReadOnlyList<LessonType>> ListAsync(
        ISpecification<LessonType> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.LessonTypes.AsQueryable(), specification)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(
        ISpecification<LessonType> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.LessonTypes.AsQueryable(), specification)
            .CountAsync(cancellationToken);

    public async Task<bool> AnyAsync(
        ISpecification<LessonType> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.LessonTypes.AsQueryable(), specification)
            .AnyAsync(cancellationToken);

    public async Task<(int ActiveCount, int ArchivedCount, DateTime? LastModifiedAt)> GetStatsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    )
    {
        // IgnoreQueryFilters нужен, чтобы включить удалённые записи в подсчёт архива.
        var data = await context
            .LessonTypes.IgnoreQueryFilters()
            .Where(lt => lt.OrganizationId == organizationId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                ActiveCount = g.Count(lt => !lt.IsDeleted),
                ArchivedCount = g.Count(lt => lt.IsDeleted),
                LastModifiedAt = g.Max(lt => lt.LastModifiedAt),
            })
            .FirstOrDefaultAsync(cancellationToken);

        return data is null
            ? (0, 0, null)
            : (data.ActiveCount, data.ArchivedCount, data.LastModifiedAt);
    }
}
