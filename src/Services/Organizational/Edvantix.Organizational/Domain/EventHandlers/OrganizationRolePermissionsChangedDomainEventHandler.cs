using Edvantix.Organizational.Domain.Events;
using Edvantix.Organizational.Pipelines;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует кеш разрешений конкретной роли после изменения её набора разрешений.
/// Затрагивает только участников с данной ролью, не трогая кеш остальных ролей организации.
/// </summary>
internal sealed class OrganizationRolePermissionsChangedDomainEventHandler(IHybridCache cache)
    : INotificationHandler<OrganizationRolePermissionsChangedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationRolePermissionsChangedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        await cache.RemoveByTagAsync(
            AuthorizationCacheKeys.RolePerms(notification.RoleId),
            cancellationToken
        );
    }
}
