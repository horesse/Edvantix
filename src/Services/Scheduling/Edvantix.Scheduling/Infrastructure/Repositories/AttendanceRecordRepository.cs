using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

namespace Edvantix.Scheduling.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAttendanceRecordRepository"/>.
/// The tenant query filter is applied by <c>SchedulingDbContext</c> via HasQueryFilter,
/// so all queries here automatically scope to the current tenant.
/// Auto-discovered by the Scrutor assembly scan in <c>AddRepositories</c> — no manual DI needed.
/// Queries are applied via <see cref="SpecificationEvaluator"/> to avoid ad-hoc LINQ predicates.
/// </summary>
public sealed class AttendanceRecordRepository(SchedulingDbContext context)
    : IAttendanceRecordRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<AttendanceRecord?> FirstOrDefaultAsync(
        Specification<AttendanceRecord> spec,
        CancellationToken ct
    ) => await Spec.GetQuery(context.AttendanceRecords, spec).FirstOrDefaultAsync(ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<AttendanceRecord>> ListAsync(
        Specification<AttendanceRecord> spec,
        CancellationToken ct
    ) => await Spec.GetQuery(context.AttendanceRecords, spec).ToListAsync(ct);

    /// <inheritdoc/>
    public void Add(AttendanceRecord record) => context.AttendanceRecords.Add(record);
}
