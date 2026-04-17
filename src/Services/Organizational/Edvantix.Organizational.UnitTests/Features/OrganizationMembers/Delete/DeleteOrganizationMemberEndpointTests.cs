namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Delete;

public sealed class DeleteOrganizationMemberEndpointTests
{
    private readonly DeleteOrganizationMemberEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidIds_WhenHandling_ThenShouldSendDeleteCommandWithCorrectIds()
    {
        var organizationId = Guid.CreateVersion7();
        var memberId = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<DeleteOrganizationMemberCommand>(c =>
                        c.OrganizationId == organizationId && c.Id == memberId
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(organizationId, memberId, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<DeleteOrganizationMemberCommand>(c =>
                        c.OrganizationId == organizationId && c.Id == memberId
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidIds_WhenHandling_ThenShouldReturnNoContent()
    {
        var organizationId = Guid.CreateVersion7();
        var memberId = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<DeleteOrganizationMemberCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(organizationId, memberId, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }

    [Test]
    public async Task GivenExceptionFromSender_WhenHandling_ThenShouldPropagateException()
    {
        var organizationId = Guid.CreateVersion7();
        var memberId = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<DeleteOrganizationMemberCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(NotFoundException.For<OrganizationMember>(memberId));

        var act = async () =>
            await _endpoint.HandleAsync(organizationId, memberId, _senderMock.Object);

        await act.ShouldThrowAsync<NotFoundException>();
    }
}
