using Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

namespace Edvantix.Scheduling.Features.Attendance.MarkAttendance;

/// <summary>
/// Handles <see cref="MarkAttendanceCommand"/> with upsert semantics.
/// <para>
/// Upsert pattern uses EF change tracking (NOT ExecuteUpdate) so that
/// <c>EventDispatchInterceptor</c> can capture the <c>AttendanceRecordedEvent</c>
/// domain event after <c>SaveChanges</c> and route it to the MassTransit outbox.
/// Using <c>ExecuteUpdate</c> would bypass change tracking and the domain event
/// would NEVER reach the outbox.
/// </para>
/// </summary>
public sealed class MarkAttendanceCommandHandler(
    IAttendanceRecordRepository repository,
    ITenantContext tenantContext
) : ICommandHandler<MarkAttendanceCommand, Unit>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        MarkAttendanceCommand command,
        CancellationToken cancellationToken
    )
    {
        var existing = await repository.FindBySlotAndStudentAsync(
            command.SlotId,
            command.StudentId,
            cancellationToken
        );

        if (existing is null)
        {
            // New record: create and add so EF tracks the entity and fires domain events.
            var record = new AttendanceRecord(
                tenantContext.SchoolId,
                command.SlotId,
                command.StudentId,
                command.Status
            );

            repository.Add(record);
        }
        else
        {
            // Existing record: update status via the aggregate method.
            // EF change tracking detects the modification and fires the domain event.
            existing.UpdateStatus(command.Status);
        }

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
