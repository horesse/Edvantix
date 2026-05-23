namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.GetById;

public sealed class GetLeadSourceByIdEndpointTests
{
    private readonly GetLeadSourceByIdEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenExistingSource_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var id = Guid.CreateVersion7();
        var dto = CreateDto(id);
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetLeadSourceByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        var okResult = result.Result.ShouldBeOfType<Ok<LeadSourceDto>>();
        okResult.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenSourceNotFound_WhenHandling_ThenShouldReturnNotFound()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetLeadSourceByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(NotFoundException.For<LeadSource>(id));

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.Result.ShouldBeOfType<NotFound>();
    }

    private static LeadSourceDto CreateDto(Guid id) =>
        new(
            id,
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
