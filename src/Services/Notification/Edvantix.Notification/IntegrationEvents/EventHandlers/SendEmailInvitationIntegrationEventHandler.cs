using Edvantix.Notification.Domain.Models;
using ISender = Edvantix.Notification.Infrastructure.Senders.ISender;

namespace Edvantix.Notification.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает <see cref="SendEmailInvitationIntegrationEvent"/>:
/// рендерит MJML-шаблон приглашения и отправляет письмо через <see cref="ISender"/>.
/// Ссылки принятия/отклонения содержат plaintext-токен и формируются на основе конфигурации фронтенда.
/// </summary>
public static class SendEmailInvitationIntegrationEventHandler
{
    public static async Task Handle(
        SendEmailInvitationIntegrationEvent @event,
        ILogger logger,
        GlobalLogBuffer logBuffer,
        IRenderer renderer,
        ISender sender,
        IConfiguration configuration,
        CancellationToken cancellationToken
    )
    {
        try
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

            logger.LogInformation(
                "Email-приглашение {InvitationId} отправлено на {Email}",
                @event.InvitationId,
                @event.Email
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ошибка отправки email-приглашения {InvitationId} на {Email}",
                @event.InvitationId,
                @event.Email
            );
            logBuffer.Flush();
            throw;
        }
    }
}
