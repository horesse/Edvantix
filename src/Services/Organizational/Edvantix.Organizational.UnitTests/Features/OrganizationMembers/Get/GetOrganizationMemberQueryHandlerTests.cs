using Edvantix.Chassis.Security.Tenant;
using Edvantix.Organizational.Grpc.Services.Profiles;
using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Get;

public sealed class GetOrganizationMemberQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRepository> _repoMock = new();
    private readonly Mock<IMapper<OrganizationMember, OrganizationMemberDto>> _mapperMock = new();
    private readonly Mock<IProfileService> _profileServiceMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetOrganizationMemberQueryHandler _handler;

    public GetOrganizationMemberQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(
            _tenantMock.Object,
            _repoMock.Object,
            _mapperMock.Object,
            _profileServiceMock.Object
        );
    }

    [Test]
    public async Task GivenExistingMember_WhenQuerying_ThenShouldReturnDtoWithFullName()
    {
        var member = CreateMember(_organizationId);
        var dto = CreateDto(member.Id);

        _repoMock
            .Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _mapperMock.Setup(m => m.Map(member)).Returns(dto);
        _profileServiceMock
            .Setup(p =>
                p.GetProfileByIdAsync(member.ProfileId.ToString(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new GetProfileResponse
                {
                    Id = dto.ProfileId.ToString(),
                    FullName = "Иванов Иван Иванович",
                }
            );

        var result = await _handler.Handle(
            new GetOrganizationMemberQuery(member.Id),
            CancellationToken.None
        );

        result.Id.ShouldBe(dto.Id);
        result.ProfileId.ShouldBe(dto.ProfileId);
        result.FullName.ShouldBe("Иванов Иван Иванович");
        result.LastActivity.ShouldNotBeNull();
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
            _handler
                .Handle(new GetOrganizationMemberQuery(memberId), CancellationToken.None)
                .AsTask()
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

    [Test]
    public async Task GivenProfileNotFound_WhenQuerying_ThenShouldThrowNotFoundException()
    {
        var member = CreateMember(_organizationId);
        var dto = CreateDto(member.Id);

        _repoMock
            .Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _mapperMock.Setup(m => m.Map(member)).Returns(dto);
        _profileServiceMock
            .Setup(p =>
                p.GetProfileByIdAsync(member.ProfileId.ToString(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((GetProfileResponse?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new GetOrganizationMemberQuery(member.Id), CancellationToken.None)
                .AsTask()
        );
    }

    private static OrganizationMember CreateMember(Guid orgId) =>
        new(orgId, Guid.CreateVersion7(), Guid.CreateVersion7(), new DateOnly(2025, 1, 1));

    private static OrganizationMemberDto CreateDto(Guid id) =>
        new(id, Guid.CreateVersion7(), "admin", OrganizationStatus.Active);
}
