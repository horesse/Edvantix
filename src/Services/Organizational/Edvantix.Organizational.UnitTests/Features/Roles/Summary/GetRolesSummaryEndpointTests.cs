using Edvantix.Organizational.Features.Roles.Summary;

namespace Edvantix.Organizational.UnitTests.Features.Roles.Summary;

public sealed class GetRolesSummaryEndpointTests
{
    private readonly GetRolesSummaryEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldCallSenderOnce()
    {
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetRolesSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSummaryDto());

        await _endpoint.HandleAsync(_senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<GetRolesSummaryQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var dto = CreateSummaryDto();
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetRolesSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(_senderMock.Object);

        result.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenResultShouldBeHttpOk()
    {
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetRolesSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSummaryDto());

        var result = await _endpoint.HandleAsync(_senderMock.Object);

        result.ShouldBeOfType<Ok<RolesSummaryDto>>();
    }

    private static RolesSummaryDto CreateSummaryDto() =>
        new(TotalRoles: 7, AssignedMembersCount: 3, RoleNamesPreview: ["Владелец", "Директор"]);
}
