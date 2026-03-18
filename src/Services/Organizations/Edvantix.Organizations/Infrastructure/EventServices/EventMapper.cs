using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizations.Infrastructure.EventServices;

/// <summary>
/// Maps Organizations domain events to integration events for the event bus.
/// Populated in Plan 02 when domain events are introduced.
/// </summary>
internal sealed class EventMapper : IEventMapper
{
    /// <inheritdoc/>
    public IntegrationEvent MapToIntegrationEvent(DomainEvent @event)
    {
        // No domain events defined yet — extended in Plan 02.
        throw new ArgumentOutOfRangeException(
            nameof(@event),
            @event,
            "No integration event mapping defined."
        );
    }
}
