using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует кешированный пустой набор разрешений, если он был записан до добавления участника.
/// </summary>
internal sealed class OrganizationMemberCreatedDomainEventHandler(IHybridCache cache)
    : INotificationHandler<OrganizationMemberCreatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationMemberCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var key =
            $"perm:org:{notification.OrganizationId}:profile:{notification.ProfileId}";
        await cache.RemoveAsync(key, cancellationToken);
    }
}
