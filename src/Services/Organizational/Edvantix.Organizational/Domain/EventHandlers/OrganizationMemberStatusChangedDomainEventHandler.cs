using Edvantix.Organizational.Domain.Events;
using Edvantix.Organizational.Pipelines;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует кеш связки «участник → роль» после деактивации или удаления участника.
/// При следующем обращении запрос вернёт null, что вызовет ForbiddenException.
/// </summary>
internal sealed class OrganizationMemberStatusChangedDomainEventHandler(IHybridCache cache)
    : INotificationHandler<OrganizationMemberStatusChangedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationMemberStatusChangedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        await cache.RemoveAsync(
            AuthorizationCacheKeys.MemberRole(notification.OrganizationId, notification.ProfileId),
            cancellationToken
        );
    }
}
