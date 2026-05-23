using Edvantix.Organizational.Features.Directories.PaymentMethods;
using Edvantix.Organizational.Features.Directories.PaymentMethods.Update;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.Update;

public sealed class UpdatePaymentMethodEndpointTests
{
    private readonly UpdatePaymentMethodEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnOk()
    {
        var command = new UpdatePaymentMethodCommand(
            Guid.CreateVersion7(),
            "Перевод",
            "transfer",
            true,
            false
        );
        var dto = CreateDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<Ok<PaymentMethodDto>>();
        result.Value.ShouldBe(dto);
    }

    private static PaymentMethodDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "Перевод",
            "transfer",
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
