namespace Edvantix.Organizational.UnitTests.Features.Roles.Delete;

public sealed class DeleteRoleEndpointTests
{
    private readonly DeleteRoleEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldReturnNoContent()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(new DeleteRoleCommand(id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldCallSenderOnce()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(new DeleteRoleCommand(id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(new DeleteRoleCommand(id), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
