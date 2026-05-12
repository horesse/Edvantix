namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.Get;

public sealed class GetGroupScheduleEndpointTests
{
    private readonly GetGroupScheduleEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidGroupId_WhenHandling_ThenShouldSendQuery()
    {
        var groupId = Guid.CreateVersion7();
        var dto = CreateDto(groupId);
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<GetGroupScheduleQuery>(q => q.GroupId == groupId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dto);

        await _endpoint.HandleAsync(groupId, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<GetGroupScheduleQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidGroupId_WhenHandling_ThenShouldReturnOk()
    {
        var groupId = Guid.CreateVersion7();
        var dto = CreateDto(groupId);
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetGroupScheduleQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(groupId, _senderMock.Object);

        result.ShouldBeOfType<Ok<GroupScheduleDto>>();
        result.Value.ShouldBe(dto);
    }

    private static GroupScheduleDto CreateDto(Guid groupId) =>
        new(
            Guid.CreateVersion7(),
            groupId,
            Guid.CreateVersion7(),
            RecurrenceType.Weekly,
            null,
            60,
            new DateOnly(2026, 1, 5),
            EndMode.Date,
            new DateOnly(2026, 2, 5),
            null,
            false,
            true,
            [],
            []
        );
}
