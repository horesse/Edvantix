namespace Edvantix.Curriculum.UnitTests.Features.Modules.Add;

public sealed class AddModuleEndpointTests
{
    private readonly AddModuleEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldSendCommand()
    {
        var command = new AddModuleCommand(Guid.CreateVersion7(), "Module", null, 2);
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldReturnCreated()
    {
        var command = new AddModuleCommand(Guid.CreateVersion7(), "Module", null, 2);
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.Value.ShouldBe(expectedId);
        var location = result.Location;
        location.ShouldNotBeNull();
        location.ShouldContain(command.CourseId.ToString());
        location.ShouldContain(expectedId.ToString());
    }
}
