using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Schedule.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IntegrationEvent MapToIntegrationEvent(DomainEvent @event) =>
        throw new ArgumentOutOfRangeException(nameof(@event), @event, null);
}
