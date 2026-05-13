namespace Edvantix.Organizational.UnitTests.Features.Rooms.Create;

public sealed class CreateRoomEndpointTests
{
    private readonly CreateRoomEndpoint _endpoint = new();
    private readonly LinkGenerator _linkGenerator = new Mock<LinkGenerator>().Object;
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
    {
        var command = new CreateRoomCommand("Каб. 204", Floor: 2, Seats: 20);
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.CreateVersion7());

        await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnCreated()
    {
        var command = new CreateRoomCommand("Зал А", Floor: 1, Seats: 50);
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        result.ShouldBeOfType<Created<Guid>>();
        result.Value.ShouldBe(expectedId);
    }

    [Test]
    public async Task GivenLinkGeneratorReturnsNull_WhenHandling_ThenLocationShouldFallback()
    {
        var command = new CreateRoomCommand("Каб. 101", Floor: 1, Seats: 15);
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.CreateVersion7());

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        result.Location.ShouldBe("/api/rooms");
    }
}
