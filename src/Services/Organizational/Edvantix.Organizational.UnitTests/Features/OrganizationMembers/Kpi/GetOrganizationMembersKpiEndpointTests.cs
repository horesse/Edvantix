namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Kpi;

public sealed class GetOrganizationMembersKpiEndpointTests
{
    private readonly GetOrganizationMembersKpiEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldSendQueryToSender()
    {
        var kpi = new OrganizationMembersKpiDto(10, 6, 3, 1);
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<GetOrganizationMembersKpiQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(kpi);

        await _endpoint.HandleAsync(new GetOrganizationMembersKpiQuery(), _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<GetOrganizationMembersKpiQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenMembersExist_WhenHandling_ThenShouldReturnOkWithKpiData()
    {
        var kpi = new OrganizationMembersKpiDto(10, 6, 3, 1);
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<GetOrganizationMembersKpiQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(kpi);

        var result = await _endpoint.HandleAsync(
            new GetOrganizationMembersKpiQuery(),
            _senderMock.Object
        );

        result.ShouldBeOfType<Ok<OrganizationMembersKpiDto>>();
        result.Value!.Total.ShouldBe(10);
        result.Value.Active.ShouldBe(6);
        result.Value.Archived.ShouldBe(3);
        result.Value.Deleted.ShouldBe(1);
    }

    [Test]
    public async Task GivenNoMembers_WhenHandling_ThenShouldReturnOkWithZeroedKpi()
    {
        var kpi = new OrganizationMembersKpiDto(0, 0, 0, 0);
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<GetOrganizationMembersKpiQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(kpi);

        var result = await _endpoint.HandleAsync(
            new GetOrganizationMembersKpiQuery(),
            _senderMock.Object
        );

        result.ShouldBeOfType<Ok<OrganizationMembersKpiDto>>();
        result.Value!.Total.ShouldBe(0);
        result.Value.Active.ShouldBe(0);
        result.Value.Archived.ShouldBe(0);
        result.Value.Deleted.ShouldBe(0);
    }
}
