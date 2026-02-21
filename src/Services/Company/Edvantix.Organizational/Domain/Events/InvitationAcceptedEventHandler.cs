namespace Edvantix.Organizational.Domain.Events;

/// <summary>
/// Обрабатывает доменное событие принятия приглашения.
/// В будущем: публикация интеграционного события в шину (MassTransit)
/// для добавления участника в связанные сервисы.
/// </summary>
public sealed class InvitationAcceptedEventHandler(ILogger<InvitationAcceptedEventHandler> logger)
    : INotificationHandler<InvitationAcceptedEvent>
{
    public ValueTask Handle(
        InvitationAcceptedEvent notification,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Invitation accepted: InvitationId={InvitationId}, OrganizationId={OrganizationId}, ProfileId={ProfileId}",
            notification.InvitationId,
            notification.OrganizationId,
            notification.ProfileId
        );

        return ValueTask.CompletedTask;
    }
}
