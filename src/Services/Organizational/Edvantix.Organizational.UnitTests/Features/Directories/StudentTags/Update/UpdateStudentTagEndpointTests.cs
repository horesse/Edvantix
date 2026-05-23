using Edvantix.Organizational.Features.Directories.StudentTags;
using Edvantix.Organizational.Features.Directories.StudentTags.Update;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.Update;

public sealed class UpdateStudentTagEndpointTests
{
    private readonly UpdateStudentTagEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
    {
        var command = new UpdateStudentTagCommand(Guid.CreateVersion7(), "VIP", "#FF5733");
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDto());

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var command = new UpdateStudentTagCommand(Guid.CreateVersion7(), "Premium", "#0000FF");
        var dto = CreateDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<Ok<StudentTagDto>>();
        result.Value.ShouldBe(dto);
    }

    private static StudentTagDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "VIP",
            "#FF5733",
            false,
            0,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
