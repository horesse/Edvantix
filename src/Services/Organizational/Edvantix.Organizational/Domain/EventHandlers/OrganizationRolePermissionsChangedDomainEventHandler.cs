using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует кеш разрешений всех участников организации после изменения набора разрешений роли.
/// </summary>
internal sealed class OrganizationRolePermissionsChangedDomainEventHandler(IHybridCache cache)
    : INotificationHandler<OrganizationRolePermissionsChangedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationRolePermissionsChangedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        await cache.RemoveByTagAsync($"org-perms:{notification.OrganizationId}", cancellationToken);
    }
}
