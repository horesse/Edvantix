namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Create;

public sealed class CreateOrganizationMemberEndpointTests
{
    private readonly CreateOrganizationMemberEndpoint _endpoint = new();
    private readonly LinkGenerator _linkGenerator = new Mock<LinkGenerator>().Object;
    private readonly Mock<ISender> _senderMock = new();

    private static CreateOrganizationMemberCommand BuildValidCommand() =>
        new(
            OrganizationId: Guid.CreateVersion7(),
            ProfileId: Guid.CreateVersion7(),
            OrganizationMemberRoleId: Guid.CreateVersion7(),
            StartDate: new DateOnly(2025, 1, 1)
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
    public async Task GivenValidCommand_WhenHandling_ThenLocationShouldContainMemberId()
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
    public async Task GivenValidCommand_WhenHandling_ThenLocationShouldContainOrganizationId()
    {
        var command = BuildValidCommand();
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        result.Location!.ShouldContain(command.OrganizationId.ToString());
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
