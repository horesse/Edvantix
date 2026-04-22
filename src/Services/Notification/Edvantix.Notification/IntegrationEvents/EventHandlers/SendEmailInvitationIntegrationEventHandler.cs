using Edvantix.Notification.Domain.Models;
using ISender = Edvantix.Notification.Infrastructure.Senders.ISender;

namespace Edvantix.Notification.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает <see cref="SendEmailInvitationIntegrationEvent"/>:
/// рендерит MJML-шаблон приглашения и отправляет письмо через <see cref="ISender"/>.
/// Ссылки принятия/отклонения содержат plaintext-токен и формируются на основе конфигурации фронтенда.
/// </summary>
public sealed class SendEmailInvitationIntegrationEventHandler(
    ILogger<SendEmailInvitationIntegrationEventHandler> logger,
    GlobalLogBuffer logBuffer,
    IRenderer renderer,
    ISender sender,
    IConfiguration configuration
) : IConsumer<SendEmailInvitationIntegrationEvent>
{
    public async Task Consume(ConsumeContext<SendEmailInvitationIntegrationEvent> context)
    {
        try
        {
            var frontendBaseUrl =
                configuration["Frontend:BaseUrl"]
                ?? throw new InvalidOperationException("Конфигурация Frontend:BaseUrl не задана.");

            var model = new InvitationEmailModel
            {
                Email = context.Message.Email,
                AcceptUrl = $"{frontendBaseUrl}/invitations/{context.Message.Token}/accept",
                DeclineUrl = $"{frontendBaseUrl}/invitations/{context.Message.Token}/decline",
                ExpiresAt = context.Message.ExpiresAt,
            };

            var html = await renderer.RenderAsync(
                model,
                "Invitation/InvitationEmail",
                context.CancellationToken
            );

            var message = WelcomeMimeMessageBuilder
                .Initialize()
                .WithTo(null, context.Message.Email)
                .WithSubject("Вас пригласили в организацию — Edvantix")
                .WithBody(html)
                .Build();

            await sender.SendAsync(message, context.CancellationToken);

            logger.LogInformation(
                "Email-приглашение {InvitationId} отправлено на {Email}",
                context.Message.InvitationId,
                context.Message.Email
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ошибка отправки email-приглашения {InvitationId} на {Email}",
                context.Message.InvitationId,
                context.Message.Email
            );
            logBuffer.Flush();
            throw;
        }
    }
}

[ExcludeFromCodeCoverage]
public sealed class SendEmailInvitationIntegrationEventHandlerDefinition
    : ConsumerDefinition<SendEmailInvitationIntegrationEventHandler>
{
    public SendEmailInvitationIntegrationEventHandlerDefinition()
    {
        Endpoint(x => x.Name = "notification-send-email-invitation");
        ConcurrentMessageLimit = 5;
    }
}
