using Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

namespace Edvantix.Scheduling.Features.Attendance.GetSlotAttendance;

/// <summary>
/// Handles <see cref="GetSlotAttendanceQuery"/> by fetching all attendance records
/// for the requested slot from <see cref="IAttendanceRecordRepository"/> and mapping
/// each domain entity to <see cref="AttendanceRecordDto"/>.
/// <para>
/// Tenant isolation is applied automatically via the HasQueryFilter registered
/// in <c>SchedulingDbContext.OnModelCreating</c> (same pattern as LessonSlot, Phase 04-01).
/// </para>
/// </summary>
public sealed class GetSlotAttendanceQueryHandler(IAttendanceRecordRepository repository)
    : IQueryHandler<GetSlotAttendanceQuery, IReadOnlyList<AttendanceRecordDto>>
{
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<AttendanceRecordDto>> Handle(
        GetSlotAttendanceQuery query,
        CancellationToken cancellationToken
    )
    {
        var records = await repository.ListAsync(
            new AttendanceBySlotSpecification(query.SlotId),
            cancellationToken
        );

        // Map each domain entity to DTO. Status is converted to string per D-07 response shape.
        return records
            .Select(r => new AttendanceRecordDto(
                r.StudentId,
                r.Status.ToString(),
                r.CorrelationId,
                r.MarkedAt
            ))
            .ToList();
    }
}
