using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует запись организации в кэше после обновления реквизитов.
/// </summary>
internal sealed class OrganizationUpdatedDomainEventHandler(IHybridCache cache)
    : INotificationHandler<OrganizationUpdatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationUpdatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var key = $"{nameof(Organization).ToLowerInvariant()}:{notification.OrganizationId}";
        await cache.RemoveAsync(key, cancellationToken);
    }
}
