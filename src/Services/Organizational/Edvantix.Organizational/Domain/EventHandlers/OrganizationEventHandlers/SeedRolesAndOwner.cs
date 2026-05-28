using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers.OrganizationEventHandlers;

internal sealed class SeedRolesAndOwner(IPermissionRepository permissionRepository)
    : INotificationHandler<OrganizationCreatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var organizationId = notification.OrganizationId;
        var permissions = await permissionRepository.GetAllAsync(cancellationToken);
    }
}
