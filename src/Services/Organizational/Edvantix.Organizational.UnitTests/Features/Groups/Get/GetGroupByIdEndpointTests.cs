namespace Edvantix.Organizational.UnitTests.Features.Groups.Get;

public sealed class GetGroupByIdEndpointTests
{
    private readonly GetGroupByIdEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldSendQueryWithCorrectId()
    {
        var id = Guid.CreateVersion7();
        var dto = CreateDto(id);
        _senderMock
            .Setup(s =>
                s.Send(It.Is<GetGroupByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(dto);

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.Is<GetGroupByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var id = Guid.CreateVersion7();
        var dto = CreateDto(id);
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetGroupByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.ShouldBeOfType<Ok<GroupDetailDto>>();
        result.Value.ShouldBe(dto);
    }

    private static GroupDetailDto CreateDto(Guid id) =>
        new(
            id,
            "B1-01",
            "Английский B1",
            "Описание",
            GroupLevel.B1,
            GroupFormat.Online,
            GroupStatus.Recruiting,
            10,
            0,
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 6, 30),
            Guid.CreateVersion7(),
            Teacher: new TeacherDto(Guid.CreateVersion7(), string.Empty, string.Empty, null),
            null,
            null,
            OnlinePlatform.Zoom
        );
}
