using Edvantix.Organizational.Features.Settings.OrganizationSummary;

namespace Edvantix.Organizational.UnitTests.Features.Settings.OrganizationSummary;

public sealed class GetOrganizationSummaryEndpointTests
{
    private readonly GetOrganizationSummaryEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldCallSenderOnce()
    {
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<GetOrganizationSummaryQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(CreateSummaryDto());

        await _endpoint.HandleAsync(_senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<GetOrganizationSummaryQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var dto = CreateSummaryDto();
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<GetOrganizationSummaryQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(_senderMock.Object);

        result.Value.ShouldBe(dto);
    }

    private static OrganizationSummaryDto CreateSummaryDto() =>
        new(
            Guid.CreateVersion7(),
            "ООО Тест",
            null,
            OrganizationType.PrivateEducationalCenter,
            OrganizationStatus.Active,
            true,
            new DateOnly(2020, 1, 1),
            10,
            null,
            new OrganizationSummaryDto.LastModifiedInfo(null, null)
        );
}
