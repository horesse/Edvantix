namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Get;

public sealed class GetOrganizationMemberEndpointTests
{
    private readonly GetOrganizationMemberEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    private static OrganizationMemberDto CreateDto(Guid organizationId, Guid memberId) =>
        new(
            memberId,
            organizationId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            OrganizationStatus.Active,
            new DateOnly(2025, 1, 1),
            null
        );

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldSendQueryWithCorrectId()
    {
        var memberId = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<GetOrganizationMemberQuery>(q => q.Id == memberId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(CreateDto(Guid.CreateVersion7(), memberId));

        await _endpoint.HandleAsync(memberId, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<GetOrganizationMemberQuery>(q => q.Id == memberId),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var organizationId = Guid.CreateVersion7();
        var memberId = Guid.CreateVersion7();
        var dto = CreateDto(organizationId, memberId);
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<GetOrganizationMemberQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(memberId, _senderMock.Object);

        result.ShouldBeOfType<Ok<OrganizationMemberDto>>();
        result.Value.ShouldBe(dto);
        result.Value!.Id.ShouldBe(memberId);
    }

    [Test]
    public async Task GivenExceptionFromSender_WhenHandling_ThenShouldPropagateException()
    {
        var memberId = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<GetOrganizationMemberQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(NotFoundException.For<OrganizationMember>(memberId));

        var act = async () => await _endpoint.HandleAsync(memberId, _senderMock.Object);

        await act.ShouldThrowAsync<NotFoundException>();
    }
}
