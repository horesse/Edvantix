using Edvantix.Contracts;
using Edvantix.Identity.IntegrationEvents.EventHandlers;

namespace Edvantix.Identity.UnitTests.IntegrationEvents.EventHandlers;

public sealed class DisableKeycloakUserIntegrationEventHandlerTests
{
    private readonly Mock<IKeycloakAdminService> _keycloakMock = new();
    private readonly Mock<GlobalLogBuffer> _logBufferMock = new();
    private readonly DisableKeycloakUserIntegrationEventHandler _handler;

    public DisableKeycloakUserIntegrationEventHandlerTests()
    {
        _handler = new(
            _keycloakMock.Object,
            Mock.Of<ILogger<DisableKeycloakUserIntegrationEventHandler>>(),
            _logBufferMock.Object
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenConsuming_ThenShouldCallDisableUserAsync()
    {
        var accountId = Guid.CreateVersion7();
        var @event = new DisableKeycloakUserIntegrationEvent(accountId);
        var context = CreateConsumeContext(@event);

        await _handler.Consume(context.Object);

        _keycloakMock.Verify(
            k => k.DisableUserAsync(accountId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenKeycloakThrows_WhenConsuming_ThenShouldFlushLogBufferAndRethrow()
    {
        var @event = new DisableKeycloakUserIntegrationEvent(Guid.CreateVersion7());
        var context = CreateConsumeContext(@event);

        _keycloakMock
            .Setup(k => k.DisableUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Keycloak unavailable"));

        await Should.ThrowAsync<HttpRequestException>(() => _handler.Consume(context.Object));

        _logBufferMock.Verify(b => b.Flush(), Times.Once);
    }

    private static Mock<ConsumeContext<DisableKeycloakUserIntegrationEvent>> CreateConsumeContext(
        DisableKeycloakUserIntegrationEvent @event
    )
    {
        var mock = new Mock<ConsumeContext<DisableKeycloakUserIntegrationEvent>>();
        mock.Setup(c => c.Message).Returns(@event);
        mock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
        return mock;
    }
}
