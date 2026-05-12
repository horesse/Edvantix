namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.UpdateSettings;

public sealed class UpdateGroupScheduleSettingsEndpointTests
{
    private readonly UpdateGroupScheduleSettingsEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldSendCommand()
    {
        var request = CreateRequest();
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<UpdateGroupScheduleSettingsCommand>(c =>
                        c.GroupId == request.GroupId && c.Slots == request.Slots
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(request, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.IsAny<UpdateGroupScheduleSettingsCommand>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldReturnNoContent()
    {
        _senderMock
            .Setup(s =>
                s.Send(
                    It.IsAny<UpdateGroupScheduleSettingsCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(CreateRequest(), _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }

    private static UpdateGroupScheduleSettingsRequest CreateRequest() =>
        new(
            Guid.CreateVersion7(),
            RecurrenceType.Weekly,
            60,
            EndMode.Date,
            new DateOnly(2026, 2, 5),
            null,
            null,
            SkipHolidays: false,
            NotifyStudents: true,
            [new SlotRequest(1, 600)]
        );
}
