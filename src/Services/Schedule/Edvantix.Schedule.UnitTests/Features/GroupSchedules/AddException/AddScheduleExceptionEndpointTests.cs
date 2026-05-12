namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.AddException;

public sealed class AddScheduleExceptionEndpointTests
{
    private readonly AddScheduleExceptionEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldSendCommand()
    {
        var request = new AddScheduleExceptionRequest(
            Guid.CreateVersion7(),
            new DateOnly(2026, 1, 5),
            "reason"
        );
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<AddScheduleExceptionCommand>(c =>
                        c.GroupId == request.GroupId && c.ExceptionDate == request.ExceptionDate
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Guid.CreateVersion7());

        await _endpoint.HandleAsync(request, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<AddScheduleExceptionCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldReturnCreatedWithId()
    {
        var request = new AddScheduleExceptionRequest(
            Guid.CreateVersion7(),
            new DateOnly(2026, 1, 5),
            "reason"
        );
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<AddScheduleExceptionCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(request, _senderMock.Object);

        result.ShouldBeOfType<Created<Guid>>();
        result.Value.ShouldBe(expectedId);
        result.Location!.ShouldContain(expectedId.ToString());
    }
}
