namespace Edvantix.Organizational.UnitTests.Features.Levels.Create;

public sealed class CreateLevelEndpointTests
{
    private readonly CreateLevelEndpoint _endpoint = new();
    private readonly LinkGenerator _linkGenerator = new Mock<LinkGenerator>().Object;
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenCallsSenderOnce()
    {
        var command = BuildValidCommand();
        var expectedId = Guid.CreateVersion7();

        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenReturnsCreatedWithId()
    {
        var command = BuildValidCommand();
        var expectedId = Guid.CreateVersion7();

        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        result.ShouldBeOfType<Created<Guid>>();
        result.Value.ShouldBe(expectedId);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenLocationContainsCreatedId()
    {
        var command = BuildValidCommand();
        var expectedId = Guid.CreateVersion7();

        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        result.Location!.ShouldContain(expectedId.ToString());
    }

    private static CreateLevelCommand BuildValidCommand() =>
        new("A1", "Beginner", null, LevelTone.Blue, SortOrder: 1);
}
