namespace Edvantix.Organizational.UnitTests.Features.Groups.Members.Add;

public sealed class AddGroupMemberEndpointTests
{
    private readonly AddGroupMemberEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();
    private readonly Mock<LinkGenerator> _linkerMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnCreated()
    {
        var command = new AddGroupMemberCommand(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupMemberRole.Student,
            new DateOnly(2025, 9, 1)
        );
        var memberId = Guid.CreateVersion7();

        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memberId);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkerMock.Object);

        result.ShouldBeOfType<Created<Guid>>();
        result.Value.ShouldBe(memberId);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldSendCorrectCommand()
    {
        var command = new AddGroupMemberCommand(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupMemberRole.Teacher,
            new DateOnly(2025, 9, 1)
        );

        _senderMock
            .Setup(s => s.Send(It.IsAny<AddGroupMemberCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.CreateVersion7());

        await _endpoint.HandleAsync(command, _senderMock.Object, _linkerMock.Object);

        _senderMock.Verify(
            s => s.Send(It.Is<AddGroupMemberCommand>(c => c == command), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
