using Edvantix.Organizational.Features.Directories.StudentTags;
using Edvantix.Organizational.Features.Directories.StudentTags.Create;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.Create;

public sealed class CreateStudentTagEndpointTests
{
    private readonly CreateStudentTagEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
    {
        var command = new CreateStudentTagCommand("VIP", "#FF5733");
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDto());

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnCreated()
    {
        var command = new CreateStudentTagCommand("Premium", "#0000FF");
        var dto = CreateDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<Created<StudentTagDto>>();
        result.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenLocationShouldContainId()
    {
        var command = new CreateStudentTagCommand("VIP", "#FF5733");
        var dto = CreateDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.Location.ShouldNotBeNull();
        result.Location!.ShouldContain(dto.Id.ToString());
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
