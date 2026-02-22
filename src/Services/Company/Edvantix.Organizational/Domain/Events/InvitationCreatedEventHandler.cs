namespace Edvantix.Organizational.Domain.Events;

/// <summary>
/// Обрабатывает доменное событие создания приглашения.
/// В будущем: отправка email-уведомления через внешний сервис.
/// </summary>
public sealed class InvitationCreatedEventHandler(ILogger<InvitationCreatedEventHandler> logger)
    : INotificationHandler<InvitationCreatedEvent>
{
    public ValueTask Handle(
        InvitationCreatedEvent notification,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Invitation created: InvitationId={InvitationId}, OrganizationId={OrganizationId}, InviteeEmail={InviteeEmail}",
            notification.InvitationId,
            notification.OrganizationId,
            notification.InviteeEmail
        );

        return ValueTask.CompletedTask;
    }
}
