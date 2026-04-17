namespace Edvantix.Organizational.UnitTests.Features.Organizations.List;

public sealed class GetOrganizationsEndpointTests
{
    private readonly GetOrganizationsEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenDefaultQuery_WhenHandling_ThenShouldSendQueryToSender()
    {
        var query = new GetOrganizationsQuery();
        var pagedResult = new PagedResult<OrganizationDto>([], 1, 20, 0);

        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenOrganizationsExist_WhenHandling_ThenShouldReturnOkWithPagedResult()
    {
        var dto = new OrganizationDto(
            Guid.CreateVersion7(),
            "ООО Тест",
            null,
            OrganizationType.ItSchool,
            OrganizationStatus.Active,
            true
        );
        var pagedResult = new PagedResult<OrganizationDto>([dto], 1, 20, 1);
        var query = new GetOrganizationsQuery();

        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<PagedResult<OrganizationDto>>>();
        result.Value!.Count.ShouldBe(1);
        result.Value.TotalItems.ShouldBe(1);
    }

    [Test]
    public async Task GivenNoOrganizations_WhenHandling_ThenShouldReturnOkWithEmptyResult()
    {
        var pagedResult = new PagedResult<OrganizationDto>([], 1, 20, 0);
        var query = new GetOrganizationsQuery();

        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<PagedResult<OrganizationDto>>>();
        result.Value!.ShouldBeEmpty();
        result.Value.TotalItems.ShouldBe(0);
    }
}
