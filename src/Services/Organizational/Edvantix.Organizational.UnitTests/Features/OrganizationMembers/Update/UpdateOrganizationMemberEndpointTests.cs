namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Update;

public sealed class UpdateOrganizationMemberEndpointTests
{
    private readonly UpdateOrganizationMemberEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    private static UpdateOrganizationMemberCommand BuildValidCommand() =>
        new(
            OrganizationId: Guid.CreateVersion7(),
            Id: Guid.CreateVersion7(),
            OrganizationMemberRoleId: Guid.CreateVersion7()
        );

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
    {
        var command = BuildValidCommand();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnNoContent()
    {
        var command = BuildValidCommand();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }

    [Test]
    public async Task GivenExceptionFromSender_WhenHandling_ThenShouldPropagateException()
    {
        var command = BuildValidCommand();
        var expected = new InvalidOperationException("sender error");
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expected);

        var act = async () => await _endpoint.HandleAsync(command, _senderMock.Object);

        var ex = await act.ShouldThrowAsync<InvalidOperationException>();
        ex.Message.ShouldBe(expected.Message);
    }
}
