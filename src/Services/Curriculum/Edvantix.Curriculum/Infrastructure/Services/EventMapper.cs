using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Contracts;
using Edvantix.Curriculum.Domain.Events;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Curriculum.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IntegrationEvent MapToIntegrationEvent(DomainEvent @event) =>
        @event switch
        {
            CourseArchivedDomainEvent e => new CourseArchivedIntegrationEvent
            {
                CourseId = e.CourseId,
                OrganizationId = e.OrganizationId,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null),
        };
}
