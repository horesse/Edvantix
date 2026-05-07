using Edvantix.Notification.Domain.Models;
using ISender = Edvantix.Notification.Infrastructure.Senders.ISender;

namespace Edvantix.Notification.IntegrationEvents.EventHandlers;

public sealed class SendEmailInvitationIntegrationEventHandler(
    IRenderer renderer,
    ISender sender,
    IConfiguration configuration
)
{
    public async Task Handle(
        SendEmailInvitationIntegrationEvent @event,
        CancellationToken cancellationToken
    )
    {
        var frontendBaseUrl =
            configuration["Frontend:BaseUrl"]
            ?? throw new InvalidOperationException("Конфигурация Frontend:BaseUrl не задана.");

        var model = new InvitationEmailModel
        {
            Email = @event.Email,
            AcceptUrl = $"{frontendBaseUrl}/invitations/{@event.Token}/accept",
            DeclineUrl = $"{frontendBaseUrl}/invitations/{@event.Token}/decline",
            ExpiresAt = @event.ExpiresAt,
        };

        var html = await renderer.RenderAsync(
            model,
            "Invitation/InvitationEmail",
            cancellationToken
        );

        var message = WelcomeMimeMessageBuilder
            .Initialize()
            .WithTo(null, @event.Email)
            .WithSubject("Вас пригласили в организацию — Edvantix")
            .WithBody(html)
            .Build();

        await sender.SendAsync(message, cancellationToken);
    }
}
