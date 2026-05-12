namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.Regenerate;

public sealed class RegenerateOccurrencesEndpointTests
{
    private readonly RegenerateOccurrencesEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldSendCommand()
    {
        var request = new RegenerateOccurrencesRequest(Guid.CreateVersion7(), "BLR");
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<RegenerateOccurrencesCommand>(c =>
                        c.GroupId == request.GroupId
                        && c.HolidayCountryCode == request.HolidayCountryCode
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(request, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<RegenerateOccurrencesCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldReturnNoContent()
    {
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<RegenerateOccurrencesCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(
            new RegenerateOccurrencesRequest(Guid.CreateVersion7(), null),
            _senderMock.Object
        );

        result.ShouldBeOfType<NoContent>();
    }
}
