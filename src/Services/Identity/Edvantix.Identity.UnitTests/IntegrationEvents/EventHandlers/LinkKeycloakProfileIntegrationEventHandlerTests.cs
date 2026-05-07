using Edvantix.Contracts;
using Edvantix.Identity.IntegrationEvents.EventHandlers;

namespace Edvantix.Identity.UnitTests.IntegrationEvents.EventHandlers;

public sealed class LinkKeycloakProfileIntegrationEventHandlerTests
{
    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldCallSetProfileIdAsync()
    {
        var keycloakMock = new Mock<IKeycloakAdminService>();
        var accountId = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        var @event = new LinkKeycloakProfileIntegrationEvent(accountId, profileId);

        await new LinkKeycloakProfileIntegrationEventHandler(keycloakMock.Object).Handle(
            @event,
            CancellationToken.None
        );

        keycloakMock.Verify(
            x => x.SetProfileIdAsync(accountId, profileId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenKeycloakThrows_WhenHandling_ThenShouldFlushLogBufferAndRethrow()
    {
        var keycloakMock = new Mock<IKeycloakAdminService>();
        var @event = new LinkKeycloakProfileIntegrationEvent(
            Guid.CreateVersion7(),
            Guid.CreateVersion7()
        );

        keycloakMock
            .Setup(x =>
                x.SetProfileIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new HttpRequestException("Keycloak unavailable"));

        await Should.ThrowAsync<HttpRequestException>(() =>
            new LinkKeycloakProfileIntegrationEventHandler(keycloakMock.Object).Handle(
                @event,
                CancellationToken.None
            )
        );
    }
}
