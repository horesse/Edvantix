using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.EventBus.Dispatcher;

public interface IEventDispatcher
{
    Task DispatchAsync(DomainEvent @event, CancellationToken cancellationToken = default);
}
