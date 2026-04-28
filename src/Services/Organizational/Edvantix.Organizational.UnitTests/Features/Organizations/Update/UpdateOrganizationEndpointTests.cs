namespace Edvantix.Organizational.UnitTests.Features.Organizations.Update;

public sealed class UpdateOrganizationEndpointTests
{
    private readonly UpdateOrganizationEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    private static UpdateOrganizationCommand BuildValidCommand(Guid? id = null) =>
        new(
            id ?? Guid.CreateVersion7(),
            "ООО Обновлённая Организация",
            "ОО",
            OrganizationType.PrivateEducationalCenter,
            LegalForm.Llc,
            new DateOnly(2020, 5, 1),
            ContactType.Email,
            "org@example.com",
            "Основной контакт"
        );

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
    {
        var command = BuildValidCommand();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnNoContent()
    {
        var command = BuildValidCommand();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }

    [Test]
    public async Task GivenExceptionFromSender_WhenHandling_ThenShouldPropagateException()
    {
        var command = BuildValidCommand();
        var expected = new InvalidOperationException("sender error");
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expected);

        var act = async () => await _endpoint.HandleAsync(command, _senderMock.Object);

        var ex = await act.ShouldThrowAsync<InvalidOperationException>();
        ex.Message.ShouldBe(expected.Message);
    }
}
