namespace Edvantix.Organizational.UnitTests.Features.Groups.Update;

public sealed class UpdateGroupEndpointTests
{
    private readonly UpdateGroupEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
    {
        var command = BuildCommand();
        _senderMock
            .Setup(s => s.Send(It.IsAny<UpdateGroupCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<UpdateGroupCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnNoContent()
    {
        var command = BuildCommand();
        _senderMock
            .Setup(s => s.Send(It.IsAny<UpdateGroupCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }

    private static UpdateGroupCommand BuildCommand() =>
        new(
            Id: Guid.CreateVersion7(),
            Name: "Английский B1",
            Description: "Описание",
            Level: GroupLevel.B1,
            CourseId: Guid.CreateVersion7(),
            TeacherMemberId: Guid.CreateVersion7(),
            Format: GroupFormat.Online,
            RoomId: null,
            Platform: OnlinePlatform.Zoom,
            Capacity: 12,
            EndDate: new DateOnly(2026, 6, 30)
        );
}
