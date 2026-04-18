using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует кеш разрешений конкретного участника после смены его роли.
/// </summary>
internal sealed class OrganizationMemberRoleChangedDomainEventHandler(IHybridCache cache)
    : INotificationHandler<OrganizationMemberRoleChangedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationMemberRoleChangedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var key = $"perm:org:{notification.OrganizationId}:profile:{notification.ProfileId}";
        await cache.RemoveAsync(key, cancellationToken);
    }
}
