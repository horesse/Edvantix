using Edvantix.Organizational.Features.Directories.PaymentMethods;
using Edvantix.Organizational.Features.Directories.PaymentMethods.List;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.List;

public sealed class ListPaymentMethodsEndpointTests
{
    private readonly ListPaymentMethodsEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldReturnOkWithPagedResult()
    {
        var query = new ListPaymentMethodsQuery();
        var pagedResult = new PagedResult<PaymentMethodListItemDto>([], 1, 50, 0);
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<PagedResult<PaymentMethodListItemDto>>>();
        result.Value.ShouldBe(pagedResult);
    }
}
