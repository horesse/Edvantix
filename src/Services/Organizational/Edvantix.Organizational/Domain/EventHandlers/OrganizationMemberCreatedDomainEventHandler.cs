using Edvantix.Organizational.Domain.Events;
using Edvantix.Organizational.Pipelines;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует кеш связки «участник → роль», если он был записан до добавления участника.
/// </summary>
internal sealed class OrganizationMemberCreatedDomainEventHandler(IHybridCache cache)
    : INotificationHandler<OrganizationMemberCreatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationMemberCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        await cache.RemoveAsync(
            AuthorizationCacheKeys.MemberRole(notification.OrganizationId, notification.ProfileId),
            cancellationToken
        );
    }
}
