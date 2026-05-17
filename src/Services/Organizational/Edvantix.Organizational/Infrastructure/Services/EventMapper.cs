using System.Text.Json;
using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Constants.Other;
using Edvantix.Contracts;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Domain.Events;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Infrastructure.Services;

public class EventMapper : IEventMapper
{
    public IntegrationEvent MapToIntegrationEvent(DomainEvent @event)
    {
        return @event switch
        {
            InvitationCreatedDomainEvent invitationCreatedDomainEvent =>
                SendEmailInvitationIntegrationEvent(invitationCreatedDomainEvent),
            GroupCreatedDomainEvent groupCreatedDomainEvent => MapGroupCreatedIntegrationEvent(
                groupCreatedDomainEvent
            ),
            OrganizationCreatedDomainEvent organizationCreatedDomainEvent =>
                MapOrganizationCreatedIntegrationEvent(organizationCreatedDomainEvent),
            _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null),
        };
    }

    private static IntegrationEvent SendEmailInvitationIntegrationEvent(
        InvitationCreatedDomainEvent @event
    )
    {
        return @event.Type switch
        {
            InvitationType.Email => new SendEmailInvitationIntegrationEvent
            {
                InvitationId = @event.InvitationId,
                Email = @event.Email!,
                Token = @event.Token,
                OrganizationId = @event.OrganizationId,
                ExpiresAt = @event.ExpiresAt,
            },
            InvitationType.InApp => CreateInAppEvent(@event),
            _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null),
        };
    }

    private static GroupCreatedIntegrationEvent MapGroupCreatedIntegrationEvent(
        GroupCreatedDomainEvent @event
    ) =>
        new()
        {
            GroupId = @event.GroupId,
            OrganizationId = @event.OrganizationId,
            StartDate = @event.StartDate,
        };

    private static OrganizationCreatedIntegrationEvent MapOrganizationCreatedIntegrationEvent(
        OrganizationCreatedDomainEvent @event
    ) => new() { OrganizationId = @event.OrganizationId, OwnerProfileId = @event.OwnerProfileId };

    private static SendInAppNotificationIntegrationEvent CreateInAppEvent(
        InvitationCreatedDomainEvent @event
    )
    {
        var metadata = JsonSerializer.Serialize(new { @event.InvitationId, @event.RoleId });

        return new SendInAppNotificationIntegrationEvent(
            ProfileId: @event.InviteeProfileId!.Value,
            Type: NotificationType.Invitation,
            Title: "Приглашение в организацию",
            MessageText: "Вас пригласили вступить в организацию. Примите или отклоните приглашение в разделе уведомлений.",
            Metadata: metadata
        );
    }
}
