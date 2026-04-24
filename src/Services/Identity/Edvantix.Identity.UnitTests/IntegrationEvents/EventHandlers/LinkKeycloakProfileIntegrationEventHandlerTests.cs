using Edvantix.Contracts;
using Edvantix.Identity.IntegrationEvents.EventHandlers;

namespace Edvantix.Identity.UnitTests.IntegrationEvents.EventHandlers;

public sealed class LinkKeycloakProfileIntegrationEventHandlerTests
{
    private readonly Mock<IKeycloakAdminService> _keycloakMock = new();
    private readonly Mock<GlobalLogBuffer> _logBufferMock = new();
    private readonly LinkKeycloakProfileIntegrationEventHandler _handler;

    public LinkKeycloakProfileIntegrationEventHandlerTests()
    {
        _handler = new(
            _keycloakMock.Object,
            Mock.Of<ILogger<LinkKeycloakProfileIntegrationEventHandler>>(),
            _logBufferMock.Object
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenConsuming_ThenShouldCallSetProfileIdAsync()
    {
        var accountId = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        var @event = new LinkKeycloakProfileIntegrationEvent(accountId, profileId);
        var context = CreateConsumeContext(@event);

        await _handler.Consume(context.Object);

        _keycloakMock.Verify(
            k => k.SetProfileIdAsync(accountId, profileId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenKeycloakThrows_WhenConsuming_ThenShouldFlushLogBufferAndRethrow()
    {
        var @event = new LinkKeycloakProfileIntegrationEvent(
            Guid.CreateVersion7(),
            Guid.CreateVersion7()
        );
        var context = CreateConsumeContext(@event);

        _keycloakMock
            .Setup(k =>
                k.SetProfileIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new HttpRequestException("Keycloak unavailable"));

        await Should.ThrowAsync<HttpRequestException>(() => _handler.Consume(context.Object));

        _logBufferMock.Verify(b => b.Flush(), Times.Once);
    }

    private static Mock<ConsumeContext<LinkKeycloakProfileIntegrationEvent>> CreateConsumeContext(
        LinkKeycloakProfileIntegrationEvent @event
    )
    {
        var mock = new Mock<ConsumeContext<LinkKeycloakProfileIntegrationEvent>>();
        mock.Setup(c => c.Message).Returns(@event);
        mock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
        return mock;
    }
}
