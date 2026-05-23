namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.Update;

public sealed class UpdateLeadSourceEndpointTests
{
    private readonly UpdateLeadSourceEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var command = new UpdateLeadSourceCommand(
            Guid.CreateVersion7(),
            "ВКонтакте",
            LeadChannel.Online,
            null
        );
        var dto = CreateDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<Ok<LeadSourceDto>>();
        result.Value.ShouldBe(dto);
    }

    private static LeadSourceDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "ВКонтакте",
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
