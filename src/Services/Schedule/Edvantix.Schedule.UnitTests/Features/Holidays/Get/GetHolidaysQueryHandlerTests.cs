namespace Edvantix.Schedule.UnitTests.Features.Holidays.Get;

public sealed class GetHolidaysQueryHandlerTests
{
    private readonly Mock<IHolidayRepository> _repositoryMock = new();
    private readonly GetHolidaysQueryHandler _handler;

    public GetHolidaysQueryHandlerTests()
    {
        _handler = new(_repositoryMock.Object);
    }

    [Test]
    public async Task GivenHolidays_WhenHandling_ThenShouldReturnDtos()
    {
        IReadOnlyList<Holiday> holidays =
        [
            new("blr", new DateOnly(2026, 1, 1), "New year", true),
            new("BLR", new DateOnly(2026, 5, 9), "Victory day", false),
        ];
        _repositoryMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Holiday>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(holidays);

        var result = await _handler.Handle(
            new GetHolidaysQuery("BLR", 2026),
            CancellationToken.None
        );

        result.Count.ShouldBe(2);
        result[0].CountryCode.ShouldBe("BLR");
        result[0].Name.ShouldBe("New year");
    }
}
