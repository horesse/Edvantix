namespace Edvantix.Persona.UnitTests.Domain.EventHandlers;

public sealed class AvatarDomainEventHandlerTests
{
    private readonly Mock<IBlobService> _blobServiceMock = new();
    private readonly AvatarDomainEventHandler _handler;

    public AvatarDomainEventHandlerTests()
    {
        _handler = new(_blobServiceMock.Object, Mock.Of<ILogger<AvatarDomainEventHandler>>());
    }

    [Test]
    public async Task GivenAvatarDeletedEvent_WhenHandling_ThenShouldDeleteFileFromBlobStorage()
    {
        const string avatarUrn = "urn:blob:avatars/photo.jpg";
        var @event = new AvatarDeletedDomainEvent(avatarUrn);

        await _handler.Handle(@event, CancellationToken.None);

        _blobServiceMock.Verify(
            x => x.DeleteFileAsync(avatarUrn, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenDifferentAvatarUrns_WhenHandling_ThenShouldDeleteCorrectFile()
    {
        const string avatarUrn = "urn:blob:avatars/profile/user-12345.png";
        var @event = new AvatarDeletedDomainEvent(avatarUrn);

        await _handler.Handle(@event, CancellationToken.None);

        _blobServiceMock.Verify(
            x =>
                x.DeleteFileAsync(
                    "urn:blob:avatars/profile/user-12345.png",
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenCancellationToken_WhenHandling_ThenShouldPassCancellationTokenToStorage()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var @event = new AvatarDeletedDomainEvent("urn:blob:avatars/photo.jpg");

        await _handler.Handle(@event, token);

        _blobServiceMock.Verify(x => x.DeleteFileAsync(It.IsAny<string>(), token), Times.Once);
    }
}
