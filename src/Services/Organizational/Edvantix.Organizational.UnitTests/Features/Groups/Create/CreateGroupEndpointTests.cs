namespace Edvantix.Organizational.UnitTests.Features.Groups.Create;

public sealed class CreateGroupEndpointTests
{
    private readonly CreateGroupEndpoint _endpoint = new();
    private readonly LinkGenerator _linkGenerator = new Mock<LinkGenerator>().Object;
    private readonly Mock<ISender> _senderMock = new();

    private static CreateGroupCommand BuildValidCommand() =>
        new(
            Code: "B1-01",
            Name: "Английский B1",
            Description: "Группа уровня B1",
            Level: GroupLevel.B1,
            CourseId: Guid.CreateVersion7(),
            TeacherMemberId: Guid.CreateVersion7(),
            Format: GroupFormat.Online,
            RoomId: null,
            Platform: OnlinePlatform.Zoom,
            Capacity: 10,
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30)
        );

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
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
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnCreated()
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
    public async Task GivenValidCommand_WhenHandling_ThenLocationShouldContainGroupId()
    {
        var command = BuildValidCommand();
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        result.Location!.ShouldContain(expectedId.ToString());
    }
}
