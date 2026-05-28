using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers.OrganizationEventHandlers;

internal sealed class SeedStudentStatuses(IStudentStatusRepository repository)
    : INotificationHandler<OrganizationCreatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var organizationId = notification.OrganizationId;
        var data = new StudentStatusData(organizationId);

        await repository.AddRangeAsync(data, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
