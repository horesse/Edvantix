namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.Archive;

public sealed class ArchiveLeadSourceEndpointTests
{
    private readonly ArchiveLeadSourceEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldReturnNoContent()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(It.IsAny<ArchiveLeadSourceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldSendCommandWithCorrectId()
    {
        var id = Guid.CreateVersion7();
        ArchiveLeadSourceCommand? capturedCommand = null;
        _senderMock
            .Setup(s => s.Send(It.IsAny<ArchiveLeadSourceCommand>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>(
                (cmd, _) => capturedCommand = (ArchiveLeadSourceCommand)cmd
            )
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(id, _senderMock.Object);

        capturedCommand.ShouldNotBeNull();
        capturedCommand!.Id.ShouldBe(id);
    }
}
