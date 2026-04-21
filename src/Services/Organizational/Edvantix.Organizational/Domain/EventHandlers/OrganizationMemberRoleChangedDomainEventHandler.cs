using Edvantix.Organizational.Domain.Events;
using Edvantix.Organizational.Pipelines;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует кеш связки «участник → роль» после смены роли.
/// Кеш разрешений самой роли остаётся актуальным — участник просто получит новую роль при следующем обращении.
/// </summary>
internal sealed class OrganizationMemberRoleChangedDomainEventHandler(IHybridCache cache)
    : INotificationHandler<OrganizationMemberRoleChangedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationMemberRoleChangedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        await cache.RemoveAsync(
            AuthorizationCacheKeys.MemberRole(notification.OrganizationId, notification.ProfileId),
            cancellationToken
        );
    }
}
