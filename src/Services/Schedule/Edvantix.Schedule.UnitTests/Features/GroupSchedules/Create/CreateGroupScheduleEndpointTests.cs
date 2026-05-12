namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.Create;

public sealed class CreateGroupScheduleEndpointTests
{
    private readonly CreateGroupScheduleEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldSendCommand()
    {
        var request = CreateRequest();
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<CreateGroupScheduleCommand>(c =>
                        c.GroupId == request.GroupId
                        && c.OrganizationId == request.OrganizationId
                        && c.Slots == request.Slots
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(expectedId);

        await _endpoint.HandleAsync(request, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<CreateGroupScheduleCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldReturnCreatedWithId()
    {
        var request = CreateRequest();
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<CreateGroupScheduleCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(request, _senderMock.Object);

        result.ShouldBeOfType<Created<Guid>>();
        result.Value.ShouldBe(expectedId);
        result.Location!.ShouldContain(request.GroupId.ToString());
    }

    private static CreateGroupScheduleRequest CreateRequest() =>
        new(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            RecurrenceType.Weekly,
            60,
            new DateOnly(2026, 1, 5),
            EndMode.Date,
            new DateOnly(2026, 2, 5),
            null,
            null,
            SkipHolidays: false,
            NotifyStudents: true,
            [new SlotRequest(1, 600)],
            HolidayCountryCode: null
        );
}
