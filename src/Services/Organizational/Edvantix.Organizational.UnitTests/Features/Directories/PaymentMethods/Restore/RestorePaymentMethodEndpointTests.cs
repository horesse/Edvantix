using Edvantix.Organizational.Features.Directories.PaymentMethods.Restore;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.Restore;

public sealed class RestorePaymentMethodEndpointTests
{
    private readonly RestorePaymentMethodEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldReturnNoContent()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(new RestorePaymentMethodCommand(id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mediator.Unit.Value);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldSendCommand()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<RestorePaymentMethodCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(Mediator.Unit.Value);

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(new RestorePaymentMethodCommand(id), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
