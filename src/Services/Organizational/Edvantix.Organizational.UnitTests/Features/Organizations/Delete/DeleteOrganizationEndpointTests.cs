namespace Edvantix.Organizational.UnitTests.Features.Organizations.Delete;

public sealed class DeleteOrganizationEndpointTests
{
    private readonly DeleteOrganizationEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldSendDeleteCommandWithCorrectId()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<DeleteOrganizationCommand>(c => c.Id == id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<DeleteOrganizationCommand>(c => c.Id == id),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldReturnNoContent()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<DeleteOrganizationCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }

    [Test]
    public async Task GivenExceptionFromSender_WhenHandling_ThenShouldPropagateException()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<DeleteOrganizationCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(NotFoundException.For<Organization>(id));

        var act = async () => await _endpoint.HandleAsync(id, _senderMock.Object);

        await act.ShouldThrowAsync<NotFoundException>();
    }
}
