namespace Edvantix.Organizational.UnitTests.Features.Organizations.Create;

public sealed class CreateOrganizationEndpointTests
{
    private readonly CreateOrganizationEndpoint _endpoint = new();
    private readonly LinkGenerator _linkGenerator = new Mock<LinkGenerator>().Object;
    private readonly Mock<ISender> _senderMock = new();

    private static CreateOrganizationCommand BuildValidCommand() =>
        new(
            "ООО Тест",
            "ТО",
            IsLegalEntity: true,
            new DateOnly(2020, 1, 1),
            LegalForm.Llc,
            OrganizationType.PrivateEducationalCenter,
            "info@test.com",
            ContactType.Email,
            "Основной контакт"
        );

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
    {
        var command = BuildValidCommand();
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnCreated()
    {
        var command = BuildValidCommand();
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        result.ShouldBeOfType<Created<Guid>>();
        result.Value.ShouldBe(expectedId);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenLocationShouldContainId()
    {
        var command = BuildValidCommand();
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        result.Location!.ShouldContain(expectedId.ToString());
    }

    [Test]
    public async Task GivenExceptionFromSender_WhenHandling_ThenShouldPropagateException()
    {
        var command = BuildValidCommand();
        var expected = new InvalidOperationException("sender error");
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expected);

        var act = async () =>
            await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        var ex = await act.ShouldThrowAsync<InvalidOperationException>();
        ex.Message.ShouldBe(expected.Message);
    }
}
