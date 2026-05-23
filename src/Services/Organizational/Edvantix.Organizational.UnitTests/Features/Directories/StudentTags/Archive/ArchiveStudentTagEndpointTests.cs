using Edvantix.Organizational.Features.Directories.StudentTags.Archive;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.Archive;

public sealed class ArchiveStudentTagEndpointTests
{
    private readonly ArchiveStudentTagEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldCallSenderOnce()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(It.IsAny<ArchiveStudentTagCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(
                It.Is<ArchiveStudentTagCommand>(c => c.Id == id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldReturnNoContent()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(It.IsAny<ArchiveStudentTagCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }
}
