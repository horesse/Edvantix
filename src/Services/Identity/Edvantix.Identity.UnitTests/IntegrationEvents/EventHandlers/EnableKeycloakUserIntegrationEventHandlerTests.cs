using Edvantix.Contracts;
using Edvantix.Identity.IntegrationEvents.EventHandlers;

namespace Edvantix.Identity.UnitTests.IntegrationEvents.EventHandlers;

public sealed class EnableKeycloakUserIntegrationEventHandlerTests
{
    private readonly Mock<IKeycloakAdminService> _keycloakMock = new();

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldCallEnableUserAsync()
    {
        var accountId = Guid.CreateVersion7();
        var @event = new EnableKeycloakUserIntegrationEvent(accountId);

        await EnableKeycloakUserIntegrationEventHandler.Handle(
            @event,
            _keycloakMock.Object,
            CancellationToken.None
        );

        _keycloakMock.Verify(
            k => k.EnableUserAsync(accountId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenKeycloakThrows_WhenHandling_ThenShouldFlushLogBufferAndRethrow()
    {
        var @event = new EnableKeycloakUserIntegrationEvent(Guid.CreateVersion7());

        _keycloakMock
            .Setup(k => k.EnableUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Keycloak unavailable"));

        await Should.ThrowAsync<HttpRequestException>(() =>
            EnableKeycloakUserIntegrationEventHandler.Handle(
                @event,
                _keycloakMock.Object,
                CancellationToken.None
            )
        );
    }
}
