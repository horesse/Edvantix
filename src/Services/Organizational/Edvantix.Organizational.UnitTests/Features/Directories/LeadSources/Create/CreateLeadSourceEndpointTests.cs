namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.Create;

public sealed class CreateLeadSourceEndpointTests
{
    private readonly CreateLeadSourceEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnCreated()
    {
        var command = new CreateLeadSourceCommand("Инстаграм", LeadChannel.Online, null);
        var dto = CreateDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<Created<LeadSourceDto>>();
        result.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenLocationShouldContainId()
    {
        var command = new CreateLeadSourceCommand("Флаер", LeadChannel.Offline, null);
        var dto = CreateDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.Location.ShouldNotBeNull();
        result.Location!.ShouldContain(dto.Id.ToString());
    }

    private static LeadSourceDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "Инстаграм",
            LeadChannel.Online,
            null,
            false,
            0,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
