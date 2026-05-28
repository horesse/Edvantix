using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.Events;
using ZiggyCreatures.Caching.Fusion;

namespace Edvantix.Organizational.Domain.EventHandlers.OrganizationEventHandlers;

/// <summary>
/// Инвалидирует запись организации в кэше после удаления.
/// </summary>
internal sealed class RemoveCacheOnDelete(IFusionCache cache)
    : INotificationHandler<OrganizationDeletedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationDeletedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var key = $"{nameof(Organization).ToLowerInvariant()}:{notification.OrganizationId}";
        await cache.RemoveAsync(key, token: cancellationToken);
    }
}
