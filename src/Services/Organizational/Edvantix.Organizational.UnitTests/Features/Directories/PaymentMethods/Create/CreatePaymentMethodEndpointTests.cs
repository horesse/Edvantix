using Edvantix.Organizational.Features.Directories.PaymentMethods;
using Edvantix.Organizational.Features.Directories.PaymentMethods.Create;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.Create;

public sealed class CreatePaymentMethodEndpointTests
{
    private readonly CreatePaymentMethodEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnCreated()
    {
        var command = new CreatePaymentMethodCommand("Карта", "card", true, false);
        var dto = CreateDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<Created<PaymentMethodDto>>();
        result.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenLocationShouldContainId()
    {
        var command = new CreatePaymentMethodCommand("Рассрочка", "installment", false, true);
        var dto = CreateDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.Location.ShouldNotBeNull();
        result.Location!.ShouldContain(dto.Id.ToString());
    }

    private static PaymentMethodDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
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
