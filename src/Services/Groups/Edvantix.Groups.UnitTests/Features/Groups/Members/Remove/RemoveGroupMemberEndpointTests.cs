namespace Edvantix.Groups.UnitTests.Features.Groups.Members.Remove;

public sealed class RemoveGroupMemberEndpointTests
{
    private readonly RemoveGroupMemberEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnNoContent()
    {
        var command = new RemoveGroupMemberCommand(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            new DateOnly(2025, 11, 1),
            null
        );

        _senderMock
            .Setup(s => s.Send(It.IsAny<RemoveGroupMemberCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldSendCorrectCommand()
    {
        var command = new RemoveGroupMemberCommand(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            new DateOnly(2025, 11, 1),
            "Отчисление"
        );

        _senderMock
            .Setup(s => s.Send(It.IsAny<RemoveGroupMemberCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<RemoveGroupMemberCommand>(c => c == command),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}
