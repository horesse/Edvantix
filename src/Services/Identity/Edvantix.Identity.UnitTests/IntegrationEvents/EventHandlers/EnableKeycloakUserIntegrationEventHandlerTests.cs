using Edvantix.Chassis.Logging;
using Edvantix.Contracts;
using Edvantix.Identity.IntegrationEvents.EventHandlers;

namespace Edvantix.Identity.UnitTests.IntegrationEvents.EventHandlers;

public sealed class EnableKeycloakUserIntegrationEventHandlerTests
{
    private readonly Mock<IKeycloakAdminService> _keycloakMock = new();
    private readonly Mock<GlobalLogBuffer> _logBufferMock = new();
    private readonly EnableKeycloakUserIntegrationEventHandler _handler;

    public EnableKeycloakUserIntegrationEventHandlerTests()
    {
        _handler = new(
            _keycloakMock.Object,
            Mock.Of<ILogger<EnableKeycloakUserIntegrationEventHandler>>(),
            _logBufferMock.Object
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenConsuming_ThenShouldCallEnableUserAsync()
    {
        var accountId = Guid.CreateVersion7();
        var @event = new EnableKeycloakUserIntegrationEvent(accountId);
        var context = CreateConsumeContext(@event);

        await _handler.Consume(context.Object);

        _keycloakMock.Verify(
            k => k.EnableUserAsync(accountId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenKeycloakThrows_WhenConsuming_ThenShouldFlushLogBufferAndRethrow()
    {
        var @event = new EnableKeycloakUserIntegrationEvent(Guid.CreateVersion7());
        var context = CreateConsumeContext(@event);

        _keycloakMock
            .Setup(k => k.EnableUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Keycloak unavailable"));

        await Should.ThrowAsync<HttpRequestException>(() => _handler.Consume(context.Object));

        _logBufferMock.Verify(b => b.Flush(), Times.Once);
    }

    private static Mock<ConsumeContext<EnableKeycloakUserIntegrationEvent>> CreateConsumeContext(
        EnableKeycloakUserIntegrationEvent @event
    )
    {
        var mock = new Mock<ConsumeContext<EnableKeycloakUserIntegrationEvent>>();
        mock.Setup(c => c.Message).Returns(@event);
        mock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
        return mock;
    }
}
