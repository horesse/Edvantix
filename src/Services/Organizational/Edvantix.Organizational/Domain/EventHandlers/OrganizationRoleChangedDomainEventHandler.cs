using Edvantix.Organizational.Domain.Events;
using Edvantix.Organizational.Pipelines;
using ZiggyCreatures.Caching.Fusion;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует кеш связки «участник → роль» после смены роли.
/// Кеш разрешений самой роли остаётся актуальным — участник просто получит новую роль при следующем обращении.
/// </summary>
internal sealed class OrganizationRoleChangedDomainEventHandler(IFusionCache cache)
    : INotificationHandler<OrganizationRoleChangedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationRoleChangedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        await cache.RemoveAsync(
            AuthorizationCacheKeys.MemberRole(notification.OrganizationId, notification.ProfileId),
            token: cancellationToken
        );
    }
}
