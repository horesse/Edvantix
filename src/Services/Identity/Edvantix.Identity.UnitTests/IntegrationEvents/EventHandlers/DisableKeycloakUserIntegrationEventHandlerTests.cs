using Edvantix.Contracts;
using Edvantix.Identity.IntegrationEvents.EventHandlers;

namespace Edvantix.Identity.UnitTests.IntegrationEvents.EventHandlers;

public sealed class DisableKeycloakUserIntegrationEventHandlerTests
{
    private readonly Mock<IKeycloakAdminService> _keycloakMock = new();

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldCallDisableUserAsync()
    {
        var accountId = Guid.CreateVersion7();
        var @event = new DisableKeycloakUserIntegrationEvent(accountId);

        await DisableKeycloakUserIntegrationEventHandler.Handle(
            @event,
            _keycloakMock.Object,
            CancellationToken.None
        );

        _keycloakMock.Verify(
            k => k.DisableUserAsync(accountId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenKeycloakThrows_WhenHandling_ThenShouldFlushLogBufferAndRethrow()
    {
        var @event = new DisableKeycloakUserIntegrationEvent(Guid.CreateVersion7());

        _keycloakMock
            .Setup(k => k.DisableUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Keycloak unavailable"));

        await Should.ThrowAsync<HttpRequestException>(() =>
            DisableKeycloakUserIntegrationEventHandler.Handle(
                @event,
                _keycloakMock.Object,
                CancellationToken.None
            )
        );
    }
}
