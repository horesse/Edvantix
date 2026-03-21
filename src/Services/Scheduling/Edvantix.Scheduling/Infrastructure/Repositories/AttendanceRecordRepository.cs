using Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

namespace Edvantix.Scheduling.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAttendanceRecordRepository"/>.
/// The tenant query filter is applied by <c>SchedulingDbContext</c> via HasQueryFilter,
/// so all queries here automatically scope to the current tenant.
/// Auto-discovered by the Scrutor assembly scan in <c>AddRepositories</c> — no manual DI needed.
/// </summary>
public sealed class AttendanceRecordRepository(SchedulingDbContext context)
    : IAttendanceRecordRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<AttendanceRecord?> FindBySlotAndStudentAsync(
        Guid slotId,
        Guid studentId,
        CancellationToken ct
    ) =>
        await context.AttendanceRecords.FirstOrDefaultAsync(
            a => a.LessonSlotId == slotId && a.StudentId == studentId,
            ct
        );

    /// <inheritdoc/>
    public async Task<IReadOnlyList<AttendanceRecord>> GetBySlotAsync(
        Guid slotId,
        CancellationToken ct
    ) => await context.AttendanceRecords.Where(a => a.LessonSlotId == slotId).ToListAsync(ct);

    /// <inheritdoc/>
    public void Add(AttendanceRecord record) => context.AttendanceRecords.Add(record);
}
