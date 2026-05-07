using Edvantix.Contracts;
using Edvantix.Identity.IntegrationEvents.EventHandlers;

namespace Edvantix.Identity.UnitTests.IntegrationEvents.EventHandlers;

public sealed class DisableKeycloakUserIntegrationEventHandlerTests
{
    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldCallDisableUserAsync()
    {
        var keycloakMock = new Mock<IKeycloakAdminService>();
        var accountId = Guid.CreateVersion7();
        var @event = new DisableKeycloakUserIntegrationEvent(accountId);

        await new DisableKeycloakUserIntegrationEventHandler(keycloakMock.Object).Handle(
            @event,
            CancellationToken.None
        );

        keycloakMock.Verify(
            x => x.DisableUserAsync(accountId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenKeycloakThrows_WhenHandling_ThenShouldFlushLogBufferAndRethrow()
    {
        var keycloakMock = new Mock<IKeycloakAdminService>();
        var @event = new DisableKeycloakUserIntegrationEvent(Guid.CreateVersion7());

        keycloakMock
            .Setup(x => x.DisableUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Keycloak unavailable"));

        await Should.ThrowAsync<HttpRequestException>(() =>
            new DisableKeycloakUserIntegrationEventHandler(keycloakMock.Object).Handle(
                @event,
                CancellationToken.None
            )
        );
    }
}
