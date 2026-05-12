namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.RemoveException;

public sealed class RemoveScheduleExceptionEndpointTests
{
    private readonly RemoveScheduleExceptionEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldSendCommand()
    {
        var groupId = Guid.CreateVersion7();
        var exceptionId = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<RemoveScheduleExceptionCommand>(c =>
                        c.GroupId == groupId && c.ExceptionId == exceptionId
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync((groupId, exceptionId), _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<RemoveScheduleExceptionCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldReturnNoContent()
    {
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<RemoveScheduleExceptionCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(
            (Guid.CreateVersion7(), Guid.CreateVersion7()),
            _senderMock.Object
        );

        result.ShouldBeOfType<NoContent>();
    }
}
