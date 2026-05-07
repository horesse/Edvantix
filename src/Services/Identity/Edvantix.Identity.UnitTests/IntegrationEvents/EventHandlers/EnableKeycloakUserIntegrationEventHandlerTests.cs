using Edvantix.Contracts;
using Edvantix.Identity.IntegrationEvents.EventHandlers;

namespace Edvantix.Identity.UnitTests.IntegrationEvents.EventHandlers;

public sealed class EnableKeycloakUserIntegrationEventHandlerTests
{
    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldCallEnableUserAsync()
    {
        var keycloakMock = new Mock<IKeycloakAdminService>();
        var accountId = Guid.CreateVersion7();
        var @event = new EnableKeycloakUserIntegrationEvent(accountId);

        await new EnableKeycloakUserIntegrationEventHandler(keycloakMock.Object).Handle(
            @event,
            CancellationToken.None
        );

        keycloakMock.Verify(
            x => x.EnableUserAsync(accountId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenKeycloakThrows_WhenHandling_ThenShouldFlushLogBufferAndRethrow()
    {
        var keycloakMock = new Mock<IKeycloakAdminService>();
        var @event = new EnableKeycloakUserIntegrationEvent(Guid.CreateVersion7());

        keycloakMock
            .Setup(x => x.EnableUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Keycloak unavailable"));

        await Should.ThrowAsync<HttpRequestException>(() =>
            new EnableKeycloakUserIntegrationEventHandler(keycloakMock.Object).Handle(
                @event,
                CancellationToken.None
            )
        );
    }
}
