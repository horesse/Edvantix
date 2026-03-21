using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Contracts;
using Edvantix.Scheduling.Infrastructure.EventServices.Events;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Scheduling.Infrastructure.EventServices;

/// <summary>
/// Maps Scheduling domain events to integration events for the MassTransit outbox.
/// Each mapping produces an <see cref="AttendanceRecordedIntegrationEvent"/> that
/// downstream services (e.g. Payments in Phase 5) subscribe to for attendance processing.
/// </summary>
internal sealed class EventMapper : IEventMapper
{
    /// <inheritdoc/>
    public IntegrationEvent MapToIntegrationEvent(DomainEvent @event)
    {
        return @event switch
        {
            // Attendance marked or updated — publish to attendance-recorded-integration-event topic (D-11).
            // e.Status is already a string (converted via Status.ToString() in AttendanceRecord per D-12).
            // e.MarkedAt maps to Timestamp in the integration event contract.
            AttendanceRecordedEvent e => new AttendanceRecordedIntegrationEvent(
                e.StudentId,
                e.LessonSlotId,
                e.SchoolId,
                e.Status,
                e.MarkedAt,
                e.CorrelationId
            ),

            _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null),
        };
    }
}
