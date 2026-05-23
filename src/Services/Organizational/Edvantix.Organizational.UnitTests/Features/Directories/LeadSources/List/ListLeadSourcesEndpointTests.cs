namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.List;

public sealed class ListLeadSourcesEndpointTests
{
    private readonly ListLeadSourcesEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidQuery_WhenHandling_ThenShouldReturnOkWithPagedResult()
    {
        var query = new ListLeadSourcesQuery();
        var pagedResult = new PagedResult<LeadSourceListItemDto>(
            Array.Empty<LeadSourceListItemDto>(),
            1,
            50,
            0
        );
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<PagedResult<LeadSourceListItemDto>>>();
        result.Value.ShouldBe(pagedResult);
    }
}
