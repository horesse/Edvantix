using Edvantix.Chassis.Security.Tenant;

namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Get;

public sealed class GetOrganizationMemberQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRepository> _repoMock = new();
    private readonly Mock<IMapper<OrganizationMember, OrganizationMemberDto>> _mapperMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetOrganizationMemberQueryHandler _handler;

    public GetOrganizationMemberQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenExistingMember_WhenQuerying_ThenShouldReturnDto()
    {
        var member = CreateMember(_organizationId);
        var dto = CreateDto(member.Id, _organizationId);

        _repoMock
            .Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _mapperMock.Setup(m => m.Map(member)).Returns(dto);

        var result = await _handler.Handle(
            new GetOrganizationMemberQuery(member.Id),
            CancellationToken.None
        );

        result.ShouldBe(dto);
        _mapperMock.Verify(m => m.Map(member), Times.Once);
    }

    [Test]
    public async Task GivenMemberNotFound_WhenQuerying_ThenShouldThrowNotFoundException()
    {
        var memberId = Guid.CreateVersion7();

        _repoMock
            .Setup(r => r.GetByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationMember?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetOrganizationMemberQuery(memberId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenMemberFromDifferentOrganization_WhenQuerying_ThenShouldThrowNotFoundException()
    {
        var member = CreateMember(Guid.CreateVersion7());

        _repoMock
            .Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new GetOrganizationMemberQuery(member.Id), CancellationToken.None)
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
