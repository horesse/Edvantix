namespace Edvantix.Organizational.UnitTests.Features.Groups.Members.List;

public sealed class GetGroupMembersEndpointTests
{
    private readonly GetGroupMembersEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidQuery_WhenHandling_ThenShouldReturnOkWithPagedResult()
    {
        var query = new GetGroupMembersQuery(Guid.CreateVersion7());
        var pagedResult = new PagedResult<GroupMemberDto>([], 1, 50, 0);

        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<PagedResult<GroupMemberDto>>>();
        result.Value.ShouldBe(pagedResult);
    }

    [Test]
    public async Task GivenValidQuery_WhenHandling_ThenShouldSendQueryToSender()
    {
        var query = new GetGroupMembersQuery(
            Guid.CreateVersion7(),
            IncludeExited: true,
            PageIndex: 2,
            PageSize: 25
        );

        _senderMock
            .Setup(s => s.Send(It.IsAny<GetGroupMembersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<GroupMemberDto>([], 2, 25, 0));

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(It.Is<GetGroupMembersQuery>(q => q == query), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
