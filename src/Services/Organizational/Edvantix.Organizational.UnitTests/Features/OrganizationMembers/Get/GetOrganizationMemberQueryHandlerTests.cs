namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Get;

public sealed class GetOrganizationMemberQueryHandlerTests
{
    private readonly Mock<IOrganizationMemberRepository> _repoMock = new();
    private readonly Mock<IMapper<OrganizationMember, OrganizationMemberDto>> _mapperMock = new();
    private readonly GetOrganizationMemberQueryHandler _handler;

    public GetOrganizationMemberQueryHandlerTests()
    {
        _handler = new(_repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenExistingMember_WhenQuerying_ThenShouldReturnDto()
    {
        var orgId = Guid.CreateVersion7();
        var member = CreateMember(orgId);
        var dto = CreateDto(member.Id, orgId);

        _repoMock
            .Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _mapperMock.Setup(m => m.Map(member)).Returns(dto);

        var result = await _handler.Handle(
            new GetOrganizationMemberQuery(orgId, member.Id),
            CancellationToken.None
        );

        result.ShouldBe(dto);
        _mapperMock.Verify(m => m.Map(member), Times.Once);
    }

    [Test]
    public async Task GivenMemberNotFound_WhenQuerying_ThenShouldThrowNotFoundException()
    {
        var orgId = Guid.CreateVersion7();
        var memberId = Guid.CreateVersion7();

        _repoMock
            .Setup(r => r.GetByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationMember?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new GetOrganizationMemberQuery(orgId, memberId), CancellationToken.None)
                .AsTask()
        );
    }

    [Test]
    public async Task GivenMemberFromDifferentOrganization_WhenQuerying_ThenShouldThrowNotFoundException()
    {
        var requestOrgId = Guid.CreateVersion7();
        var actualOrgId = Guid.CreateVersion7();
        var member = CreateMember(actualOrgId);

        _repoMock
            .Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new GetOrganizationMemberQuery(requestOrgId, member.Id),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    private static OrganizationMember CreateMember(Guid orgId) =>
        new(orgId, Guid.CreateVersion7(), Guid.CreateVersion7(), new DateOnly(2025, 1, 1));

    private static OrganizationMemberDto CreateDto(Guid id, Guid orgId) =>
        new(
            id,
            orgId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            OrganizationStatus.Active,
            new DateOnly(2025, 1, 1),
            null
        );
}
