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
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Mock<IRenderer> _rendererMock = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenInvitationEvent_WhenHandling_ThenShouldRenderInvitationTemplateAndSendEmail()
    {
        var integrationEvent = CreateEvent();
        InvitationEmailModel? renderedModel = null;
        MimeMessage? sentMessage = null;

        _configurationMock.Setup(x => x["Frontend:BaseUrl"]).Returns("https://app.edvantix.test");
        _rendererMock
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
        _senderMock
            .Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .Callback<MimeMessage, CancellationToken>((message, _) => sentMessage = message)
            .Returns(Task.CompletedTask);

        await SendEmailInvitationIntegrationEventHandler.Handle(
            integrationEvent,
            _rendererMock.Object,
            _senderMock.Object,
            _configurationMock.Object,
            CancellationToken.None
        );

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
        _senderMock.Verify(
            x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenMissingFrontendBaseUrl_WhenHandling_ThenShouldFlushLogBufferAndThrow()
    {
        var integrationEvent = CreateEvent();

        _configurationMock.Setup(x => x["Frontend:BaseUrl"]).Returns((string?)null);

        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            SendEmailInvitationIntegrationEventHandler.Handle(
                integrationEvent,
                _rendererMock.Object,
                _senderMock.Object,
                _configurationMock.Object,
                CancellationToken.None
            )
        );

        exception.Message.ShouldBe("Конфигурация Frontend:BaseUrl не задана.");
        _rendererMock.Verify(
            x =>
                x.RenderAsync(
                    It.IsAny<InvitationEmailModel>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
        _senderMock.Verify(
            x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenSenderThrows_WhenHandling_ThenShouldFlushLogBufferAndRethrow()
    {
        var integrationEvent = CreateEvent();
        var expectedException = new InvalidOperationException("SMTP unavailable");

        _configurationMock.Setup(x => x["Frontend:BaseUrl"]).Returns("https://app.edvantix.test");
        _rendererMock
            .Setup(x =>
                x.RenderAsync(
                    It.IsAny<InvitationEmailModel>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync("<html>Invitation body</html>");
        _senderMock
            .Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            SendEmailInvitationIntegrationEventHandler.Handle(
                integrationEvent,
                _rendererMock.Object,
                _senderMock.Object,
                _configurationMock.Object,
                CancellationToken.None
            )
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
