using Edvantix.Contracts;
using Edvantix.Identity.IntegrationEvents.EventHandlers;

namespace Edvantix.Identity.UnitTests.IntegrationEvents.EventHandlers;

public sealed class LinkKeycloakProfileIntegrationEventHandlerTests
{
    private readonly Mock<IKeycloakAdminService> _keycloakMock = new();
    private readonly Mock<GlobalLogBuffer> _logBufferMock = new();
    private readonly ILogger _logger = Mock.Of<ILogger>();

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldCallSetProfileIdAsync()
    {
        var accountId = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        var @event = new LinkKeycloakProfileIntegrationEvent(accountId, profileId);

        await LinkKeycloakProfileIntegrationEventHandler.Handle(
            @event,
            _keycloakMock.Object,
            _logger,
            _logBufferMock.Object,
            CancellationToken.None
        );

        _keycloakMock.Verify(
            k => k.SetProfileIdAsync(accountId, profileId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenKeycloakThrows_WhenHandling_ThenShouldFlushLogBufferAndRethrow()
    {
        var @event = new LinkKeycloakProfileIntegrationEvent(
            Guid.CreateVersion7(),
            Guid.CreateVersion7()
        );

        _keycloakMock
            .Setup(k =>
                k.SetProfileIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new HttpRequestException("Keycloak unavailable"));

        await Should.ThrowAsync<HttpRequestException>(() =>
            LinkKeycloakProfileIntegrationEventHandler.Handle(
                @event,
                _keycloakMock.Object,
                _logger,
                _logBufferMock.Object,
                CancellationToken.None
            )
        );

        _logBufferMock.Verify(b => b.Flush(), Times.Once);
    }
}
