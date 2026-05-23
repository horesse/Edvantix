using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;

namespace Edvantix.Groups.Infrastructure.Repositories;

internal sealed class SubjectRepository(GroupsDbContext context) : ISubjectRepository
{
    public IUnitOfWork UnitOfWork => context;

    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    public async Task<Subject?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context.Subjects.AsTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task AddAsync(Subject subject, CancellationToken cancellationToken = default) =>
        await context.Subjects.AddAsync(subject, cancellationToken);

    public async Task<IReadOnlyList<Subject>> ListAsync(
        ISpecification<Subject> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Spec
            .GetQuery(context.Subjects.AsNoTracking(), specification)
            .ToListAsync(cancellationToken);

    public async Task<long> CountAsync(
        ISpecification<Subject> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Spec
            .GetQuery(context.Subjects.AsNoTracking(), specification)
            .LongCountAsync(cancellationToken);

    public async Task<bool> AnyAsync(
        ISpecification<Subject> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Spec
            .GetQuery(context.Subjects.AsNoTracking(), specification)
            .AnyAsync(cancellationToken);

    public async Task<bool> ExistsWithCodeAsync(
        Guid organizationId,
        SubjectCode code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    )
    {
        // Сравнение выполняется в памяти: SubjectCode хранится через value-конвертер
        // и не транслируется напрямую в SQL-предикат (аналогичный подход в LevelRepository).
        var entries = await context
            .Subjects.Where(s => s.OrganizationId == organizationId && !s.IsArchived)
            .Select(s => new { s.Id, s.Code })
            .ToListAsync(cancellationToken);

        return entries.Any(e => e.Code == code && (!excludeId.HasValue || e.Id != excludeId.Value));
    }

    public async Task<(int ActiveCount, int ArchivedCount, DateTime? LastModifiedAt)> GetStatsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    )
    {
        var stats = await context
            .Subjects.Where(s => s.OrganizationId == organizationId)
            .Select(s => new { s.IsArchived, s.LastModifiedAt })
            .ToListAsync(cancellationToken);

        var activeCount = stats.Count(s => !s.IsArchived);
        var archivedCount = stats.Count(s => s.IsArchived);
        var lastModifiedAt = stats
            .Where(s => s.LastModifiedAt.HasValue)
            .Max(s => s.LastModifiedAt);

        return (activeCount, archivedCount, lastModifiedAt);
    }
}
