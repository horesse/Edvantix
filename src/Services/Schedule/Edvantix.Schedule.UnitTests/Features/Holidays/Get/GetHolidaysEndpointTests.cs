namespace Edvantix.Schedule.UnitTests.Features.Holidays.Get;

public sealed class GetHolidaysEndpointTests
{
    private readonly GetHolidaysEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldSendQuery()
    {
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<GetHolidaysQuery>(q => q.CountryCode == "BLR" && q.Year == 2026),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);

        await _endpoint.HandleAsync(("BLR", 2026), _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<GetHolidaysQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldReturnOk()
    {
        IReadOnlyList<HolidayDto> expected =
        [
            new(Guid.CreateVersion7(), "BLR", new DateOnly(2026, 1, 1), "New year", true),
        ];
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetHolidaysQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _endpoint.HandleAsync(("BLR", 2026), _senderMock.Object);

        result.ShouldBeOfType<Ok<IReadOnlyList<HolidayDto>>>();
        result.Value.ShouldBe(expected);
    }
}
