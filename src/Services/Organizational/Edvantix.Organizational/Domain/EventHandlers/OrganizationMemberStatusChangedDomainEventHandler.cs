using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует кеш разрешений участника после его деактивации или удаления.
/// </summary>
internal sealed class OrganizationMemberStatusChangedDomainEventHandler(IHybridCache cache)
    : INotificationHandler<OrganizationMemberStatusChangedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationMemberStatusChangedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var key = $"perm:org:{notification.OrganizationId}:profile:{notification.ProfileId}";
        await cache.RemoveAsync(key, cancellationToken);
    }
}
