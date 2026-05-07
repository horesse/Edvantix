using Edvantix.Contracts;
using Edvantix.Notification.Domain.Models;
using Edvantix.Notification.Infrastructure.Render;
using Edvantix.Notification.Infrastructure.Senders;
using Edvantix.Notification.IntegrationEvents.EventHandlers;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Edvantix.Notification.UnitTests.Handlers;

public sealed class SendEmailInvitationIntegrationEventHandlerTests
{
    [Test]
    public async Task GivenInvitationEvent_WhenHandling_ThenShouldRenderInvitationTemplateAndSendEmail()
    {
        var configurationMock = new Mock<IConfiguration>();
        var rendererMock = new Mock<IRenderer>();
        var senderMock = new Mock<ISender>();
        var integrationEvent = CreateEvent();
        InvitationEmailModel? renderedModel = null;
        MimeMessage? sentMessage = null;

        configurationMock.Setup(x => x["Frontend:BaseUrl"]).Returns("https://app.edvantix.test");
        rendererMock
            .Setup(x =>
                x.RenderAsync(
                    It.IsAny<InvitationEmailModel>(),
                    "Invitation/InvitationEmail",
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<InvitationEmailModel, string, CancellationToken>(
                (model, _, _) => renderedModel = model
            )
            .ReturnsAsync("<html>Invitation body</html>");
        senderMock
            .Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .Callback<MimeMessage, CancellationToken>((message, _) => sentMessage = message)
            .Returns(Task.CompletedTask);

        await new SendEmailInvitationIntegrationEventHandler(
            rendererMock.Object,
            senderMock.Object,
            configurationMock.Object
        ).Handle(integrationEvent, CancellationToken.None);

        renderedModel.ShouldNotBeNull();
        renderedModel.Email.ShouldBe(integrationEvent.Email);
        renderedModel.AcceptUrl.ShouldBe(
            $"https://app.edvantix.test/invitations/{integrationEvent.Token}/accept"
        );
        renderedModel.DeclineUrl.ShouldBe(
            $"https://app.edvantix.test/invitations/{integrationEvent.Token}/decline"
        );
        renderedModel.ExpiresAt.ShouldBe(integrationEvent.ExpiresAt);

        sentMessage.ShouldNotBeNull();
        sentMessage.To.Mailboxes.Single().Address.ShouldBe(integrationEvent.Email);
        sentMessage.Subject.ShouldBe("Вас пригласили в организацию — Edvantix");
        sentMessage.HtmlBody.ShouldBe("<html>Invitation body</html>");
        senderMock.Verify(
            x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenMissingFrontendBaseUrl_WhenHandling_ThenShouldFlushLogBufferAndThrow()
    {
        var configurationMock = new Mock<IConfiguration>();
        var rendererMock = new Mock<IRenderer>();
        var senderMock = new Mock<ISender>();
        var integrationEvent = CreateEvent();

        configurationMock.Setup(x => x["Frontend:BaseUrl"]).Returns((string?)null);

        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            new SendEmailInvitationIntegrationEventHandler(
                rendererMock.Object,
                senderMock.Object,
                configurationMock.Object
            ).Handle(integrationEvent, CancellationToken.None)
        );

        exception.Message.ShouldBe("Конфигурация Frontend:BaseUrl не задана.");
        rendererMock.Verify(
            x =>
                x.RenderAsync(
                    It.IsAny<InvitationEmailModel>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
        senderMock.Verify(
            x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenSenderThrows_WhenHandling_ThenShouldFlushLogBufferAndRethrow()
    {
        var configurationMock = new Mock<IConfiguration>();
        var rendererMock = new Mock<IRenderer>();
        var senderMock = new Mock<ISender>();
        var integrationEvent = CreateEvent();
        var expectedException = new InvalidOperationException("SMTP unavailable");

        configurationMock.Setup(x => x["Frontend:BaseUrl"]).Returns("https://app.edvantix.test");
        rendererMock
            .Setup(x =>
                x.RenderAsync(
                    It.IsAny<InvitationEmailModel>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync("<html>Invitation body</html>");
        senderMock
            .Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            new SendEmailInvitationIntegrationEventHandler(
                rendererMock.Object,
                senderMock.Object,
                configurationMock.Object
            ).Handle(integrationEvent, CancellationToken.None)
        );

        exception.ShouldBeSameAs(expectedException);
    }

    private static SendEmailInvitationIntegrationEvent CreateEvent() =>
        new()
        {
            InvitationId = Guid.NewGuid(),
            Email = "invitee@test.com",
            Token = "plain-token",
            OrganizationId = Guid.NewGuid(),
            ExpiresAt = new DateTime(2026, 5, 3, 12, 0, 0, DateTimeKind.Utc),
        };
}
