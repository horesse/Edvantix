using Edvantix.Constants.Other;
using Edvantix.Contracts;
using Edvantix.Notification.Infrastructure.Senders.InApp;
using Edvantix.Notification.IntegrationEvents.EventHandlers;

namespace Edvantix.Notification.UnitTests.Handlers;

public sealed class SendInAppNotificationIntegrationEventHandlerTests
{
    private readonly Mock<IInAppSender> _senderMock = new();

    [Test]
    public async Task GivenNotificationEvent_WhenHandling_ThenShouldSendMappedInAppMessage()
    {
        var integrationEvent = CreateEvent();
        InAppNotificationMessage? sentMessage = null;

        _senderMock
            .Setup(x =>
                x.SendAsync(It.IsAny<InAppNotificationMessage>(), It.IsAny<CancellationToken>())
            )
            .Callback<InAppNotificationMessage, CancellationToken>(
                (message, _) => sentMessage = message
            )
            .Returns(Task.CompletedTask);

        await SendInAppNotificationIntegrationEventHandler.Handle(
            integrationEvent,
            _senderMock.Object,
            CancellationToken.None
        );

        sentMessage.ShouldNotBeNull();
        sentMessage.ProfileId.ShouldBe(integrationEvent.ProfileId);
        sentMessage.Type.ShouldBe(NotificationType.Invitation);
        sentMessage.Title.ShouldBe(integrationEvent.Title);
        sentMessage.Message.ShouldBe(integrationEvent.MessageText);
        sentMessage.Metadata.ShouldBe(integrationEvent.Metadata);
        _senderMock.Verify(
            x => x.SendAsync(It.IsAny<InAppNotificationMessage>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenSenderThrows_WhenHandling_ThenShouldFlushLogBufferAndRethrow()
    {
        var integrationEvent = CreateEvent();
        var expectedException = new InvalidOperationException("Storage unavailable");

        _senderMock
            .Setup(x =>
                x.SendAsync(It.IsAny<InAppNotificationMessage>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(expectedException);

        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            SendInAppNotificationIntegrationEventHandler.Handle(
                integrationEvent,
                _senderMock.Object,
                CancellationToken.None
            )
        );

        exception.ShouldBeSameAs(expectedException);
    }

    private static SendInAppNotificationIntegrationEvent CreateEvent() =>
        new()
        {
            ProfileId = Guid.NewGuid(),
            Type = (int)NotificationType.Invitation,
            Title = "Новое приглашение",
            MessageText = "Вас пригласили в организацию",
            Metadata = """{"invitationId":"test-invitation"}""",
        };
}
