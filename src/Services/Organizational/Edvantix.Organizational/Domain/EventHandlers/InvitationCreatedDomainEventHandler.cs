using System.Text.Json;
using Edvantix.Constants.Other;
using Edvantix.Contracts;
using Edvantix.Organizational.Domain.Enums;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Отправляет уведомление о приглашении сразу после его создания.
/// Email-приглашения — через интеграционное событие <see cref="SendEmailInvitationIntegrationEvent"/>;
/// In-app — через <see cref="SendInAppNotificationIntegrationEvent"/>.
/// </summary>
internal sealed class InvitationCreatedDomainEventHandler(IBus bus)
    : INotificationHandler<InvitationCreatedDomainEvent>
{
    public async ValueTask Handle(
        InvitationCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        if (notification.Type == InvitationType.Email)
        {
            await bus.Publish(
                new SendEmailInvitationIntegrationEvent
                {
                    InvitationId = notification.InvitationId,
                    Email = notification.Email!,
                    Token = notification.Token,
                    OrganizationId = notification.OrganizationId,
                    ExpiresAt = notification.ExpiresAt,
                },
                cancellationToken
            );
        }
        else
        {
            var metadata = JsonSerializer.Serialize(
                new { InvitationId = notification.InvitationId, RoleId = notification.RoleId }
            );

            await bus.Publish(
                new SendInAppNotificationIntegrationEvent(
                    ProfileId: notification.InviteeProfileId!.Value,
                    Type: NotificationType.Invitation,
                    Title: "Приглашение в организацию",
                    MessageText: $"Вас пригласили вступить в организацию. Примите или отклоните приглашение в разделе уведомлений.",
                    Metadata: metadata
                ),
                cancellationToken
            );
        }
    }
}
