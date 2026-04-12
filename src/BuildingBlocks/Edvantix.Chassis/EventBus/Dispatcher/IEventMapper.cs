using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.EventBus.Dispatcher;

public interface IEventMapper
{
    /// <summary>
    ///     Maps a domain event to its corresponding integration event representation.
    /// </summary>
    /// <param name="event">The domain event to map.</param>
    /// <returns>The mapped integration event.</returns>
    IntegrationEvent MapToIntegrationEvent(DomainEvent @event);
}
