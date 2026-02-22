using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.EventBus.Dispatcher;

public interface IEventMapper
{
    IntegrationEvent MapToIntegrationEvent(DomainEvent @event);
}
