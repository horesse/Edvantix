namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.List;

public sealed class GetOrganizationMembersEndpointTests
{
    private readonly GetOrganizationMembersEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    private static OrganizationMemberDto CreateDto(Guid organizationId) =>
        new(
            Guid.CreateVersion7(),
            organizationId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            OrganizationStatus.Active,
            new DateOnly(2025, 1, 1),
            null
        );

    [Test]
    public async Task GivenDefaultQuery_WhenHandling_ThenShouldSendQueryToSender()
    {
        var organizationId = Guid.CreateVersion7();
        var query = new GetOrganizationMembersQuery(organizationId);
        var pagedResult = new PagedResult<OrganizationMemberDto>([], 1, 20, 0);

        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenMembersExist_WhenHandling_ThenShouldReturnOkWithPagedResult()
    {
        var organizationId = Guid.CreateVersion7();
        var dto = CreateDto(organizationId);
        var query = new GetOrganizationMembersQuery(organizationId);
        var pagedResult = new PagedResult<OrganizationMemberDto>([dto], 1, 20, 1);

        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<PagedResult<OrganizationMemberDto>>>();
        result.Value!.Count.ShouldBe(1);
        result.Value.TotalItems.ShouldBe(1);
    }

    [Test]
    public async Task GivenNoMembers_WhenHandling_ThenShouldReturnOkWithEmptyResult()
    {
        var organizationId = Guid.CreateVersion7();
        var query = new GetOrganizationMembersQuery(organizationId);
        var pagedResult = new PagedResult<OrganizationMemberDto>([], 1, 20, 0);

        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<PagedResult<OrganizationMemberDto>>>();
        result.Value!.ShouldBeEmpty();
        result.Value.TotalItems.ShouldBe(0);
    }

    [Test]
    public async Task GivenStatusFilter_WhenHandling_ThenShouldSendQueryWithFilter()
    {
        var organizationId = Guid.CreateVersion7();
        var query = new GetOrganizationMembersQuery(
            organizationId,
            Status: OrganizationStatus.Active
        );
        var pagedResult = new PagedResult<OrganizationMemberDto>([], 1, 20, 0);

        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<GetOrganizationMembersQuery>(q => q.Status == OrganizationStatus.Active),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}
