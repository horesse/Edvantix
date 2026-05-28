using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers.OrganizationEventHandlers;

internal sealed class SeedLevels(ILevelRepository repository)
    : INotificationHandler<OrganizationCreatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var organizationId = notification.OrganizationId;

        var data = new LevelData(organizationId);
        await repository.AddRange(data, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
