using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Заглушка для события отзыва приглашения.
/// Реальная логика (например, уведомление адресата) добавляется при необходимости.
/// </summary>
internal sealed class InvitationRevokedDomainEventHandler
    : INotificationHandler<InvitationRevokedDomainEvent>
{
    public ValueTask Handle(
        InvitationRevokedDomainEvent notification,
        CancellationToken cancellationToken
    ) => ValueTask.CompletedTask;
}
