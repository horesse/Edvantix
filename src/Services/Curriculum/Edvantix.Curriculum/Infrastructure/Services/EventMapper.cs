using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Curriculum.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IntegrationEvent MapToIntegrationEvent(DomainEvent @event) =>
        throw new ArgumentOutOfRangeException(nameof(@event), @event, null);
}
