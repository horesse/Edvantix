namespace Edvantix.Organizational.Domain.Events;

/// <summary>
/// Обрабатывает доменное событие отклонения приглашения.
/// В будущем: публикация интеграционного события в шину (MassTransit).
/// </summary>
public sealed class InvitationDeclinedEventHandler(ILogger<InvitationDeclinedEventHandler> logger)
    : INotificationHandler<InvitationDeclinedEvent>
{
    public ValueTask Handle(
        InvitationDeclinedEvent notification,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Invitation declined: InvitationId={InvitationId}, OrganizationId={OrganizationId}",
            notification.InvitationId,
            notification.OrganizationId
        );

        return ValueTask.CompletedTask;
    }
}
