using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Contracts;
using Edvantix.Groups.Domain.Events;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Groups.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IntegrationEvent MapToIntegrationEvent(DomainEvent @event) =>
        @event switch
        {
            GroupCreatedDomainEvent e => new GroupCreatedIntegrationEvent
            {
                GroupId = e.GroupId,
                OrganizationId = e.OrganizationId,
                StartDate = e.StartDate,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null),
        };
}
