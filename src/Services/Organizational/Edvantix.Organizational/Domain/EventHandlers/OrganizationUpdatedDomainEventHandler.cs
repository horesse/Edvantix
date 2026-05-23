using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.Events;
using ZiggyCreatures.Caching.Fusion;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует запись организации в кэше после обновления реквизитов.
/// </summary>
internal sealed class OrganizationUpdatedDomainEventHandler(IFusionCache cache)
    : INotificationHandler<OrganizationUpdatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationUpdatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var id = notification.OrganizationId;
        var detailKey = $"{nameof(Organization).ToLowerInvariant()}:{id}";
        var summaryKey = $"org:{id}:summary";

        await cache.RemoveAsync(detailKey, token: cancellationToken);
        await cache.RemoveAsync(summaryKey, token: cancellationToken);
    }
}
