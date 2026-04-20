namespace Edvantix.Organizational.UnitTests.Features.Roles.List;

public sealed class GetRolesEndpointTests
{
    private readonly GetRolesEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenRequest_WhenHandling_ThenShouldReturnOk()
    {
        var query = new GetRolesQuery();
        var pagedResult = new PagedResult<RoleDto>([], 1, 10, 0);
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<PagedResult<RoleDto>>>();
    }

    [Test]
    public async Task GivenRequest_WhenHandling_ThenShouldCallSenderOnce()
    {
        var query = new GetRolesQuery(PageIndex: 2, PageSize: 5);
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<RoleDto>([], 2, 5, 0));

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }
}
