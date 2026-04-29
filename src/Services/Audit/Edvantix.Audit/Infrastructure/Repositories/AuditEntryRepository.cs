using Edvantix.Audit.Domain.AggregatesModel.AuditEntryAggregate;
using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Audit.Infrastructure.Repositories;

internal sealed class AuditEntryRepository(AuditDbContext context) : IAuditEntryRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<AuditEntry?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .AuditEntries.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<AuditEntry>> ListAsync(
        ISpecification<AuditEntry> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.AuditEntries.AsQueryable(), specification)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(
        ISpecification<AuditEntry> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.AuditEntries.AsQueryable(), specification)
            .CountAsync(cancellationToken);

    public async Task AddAsync(AuditEntry entry, CancellationToken cancellationToken = default) =>
        await context.AuditEntries.AddAsync(entry, cancellationToken);
}
