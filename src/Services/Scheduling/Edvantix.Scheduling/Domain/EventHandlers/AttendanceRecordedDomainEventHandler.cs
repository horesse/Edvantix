using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Scheduling.Infrastructure.EventServices.Events;

namespace Edvantix.Scheduling.Domain.EventHandlers;

/// <summary>
/// Handles <see cref="AttendanceRecordedEvent"/> domain events raised by <see cref="AttendanceRecord"/>.
/// Delegates to <see cref="IEventDispatcher"/> which maps each domain event to an
/// <see cref="Edvantix.Contracts.AttendanceRecordedIntegrationEvent"/> and publishes it
/// via the MassTransit transactional outbox.
/// </summary>
public sealed class AttendanceRecordedDomainEventHandler(IEventDispatcher dispatcher)
    : INotificationHandler<AttendanceRecordedEvent>
{
    /// <inheritdoc/>
    public async ValueTask Handle(
        AttendanceRecordedEvent notification,
        CancellationToken cancellationToken
    ) => await dispatcher.DispatchAsync(notification, cancellationToken);
}
