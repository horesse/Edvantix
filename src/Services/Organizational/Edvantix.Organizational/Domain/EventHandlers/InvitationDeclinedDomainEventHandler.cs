using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Заглушка для события отклонения приглашения.
/// Реальная логика (например, уведомление отправителя) добавляется при необходимости.
/// </summary>
internal sealed class InvitationDeclinedDomainEventHandler
    : INotificationHandler<InvitationDeclinedDomainEvent>
{
    public ValueTask Handle(
        InvitationDeclinedDomainEvent notification,
        CancellationToken cancellationToken
    ) => ValueTask.CompletedTask;
}
