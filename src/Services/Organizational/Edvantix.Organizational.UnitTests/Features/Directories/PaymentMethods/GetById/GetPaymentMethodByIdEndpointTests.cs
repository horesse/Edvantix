using Edvantix.Organizational.Features.Directories.PaymentMethods;
using Edvantix.Organizational.Features.Directories.PaymentMethods.GetById;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.GetById;

public sealed class GetPaymentMethodByIdEndpointTests
{
    private readonly GetPaymentMethodByIdEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenExistingId_WhenHandling_ThenShouldReturnOk()
    {
        var id = Guid.CreateVersion7();
        var dto = CreateDto(id);
        _senderMock
            .Setup(s => s.Send(new GetPaymentMethodByIdQuery(id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        var okResult = result.Result.ShouldBeOfType<Ok<PaymentMethodDto>>();
        okResult.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenNotFoundId_WhenHandling_ThenShouldReturnNotFound()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(new GetPaymentMethodByIdQuery(id), It.IsAny<CancellationToken>()))
            .ThrowsAsync(NotFoundException.For<PaymentMethod>(id));

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.Result.ShouldBeOfType<NotFound>();
    }

    private static PaymentMethodDto CreateDto(Guid id) =>
        new(
            id,
            "Карта",
            "card",
            true,
            false,
            false,
            0,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
