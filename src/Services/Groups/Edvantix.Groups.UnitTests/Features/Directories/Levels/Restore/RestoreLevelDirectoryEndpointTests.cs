namespace Edvantix.Groups.UnitTests.Features.Directories.Levels.Restore;

public sealed class RestoreLevelDirectoryEndpointTests
{
    private readonly RestoreLevelDirectoryEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenExistingLevel_WhenHandling_ThenDelegatesToSender()
    {
        var id = Guid.CreateVersion7();
        SetupSender(id);

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<RestoreLevelDirectoryCommand>(c => c.Id == id),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenExistingLevel_WhenHandling_ThenReturnsNoContent()
    {
        var id = Guid.CreateVersion7();
        SetupSender(id);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.Result.ShouldBeOfType<NoContent>();
    }

    private void SetupSender(Guid id) =>
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<RestoreLevelDirectoryCommand>(c => c.Id == id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Unit.Value);
}
