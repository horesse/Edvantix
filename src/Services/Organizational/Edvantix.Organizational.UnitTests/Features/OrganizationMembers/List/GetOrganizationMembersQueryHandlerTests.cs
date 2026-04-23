using Edvantix.Chassis.Security.Tenant;
using Edvantix.Organizational.Grpc.Services.Profiles;
using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.List;

public sealed class GetOrganizationMembersQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRepository> _repoMock = new();
    private readonly Mock<IMapper<OrganizationMember, OrganizationMemberDto>> _mapperMock = new();
    private readonly Mock<IProfileService> _profileServiceMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetOrganizationMembersQueryHandler _handler;

    public GetOrganizationMembersQueryHandlerTests()
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
    public async Task GivenMembersExist_WhenHandling_ThenShouldReturnPagedResult()
    {
        var member = CreateMember(_organizationId);
        var dto = CreateDto(member.Id);
        var query = new GetOrganizationMembersQuery(PageIndex: 1, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([member]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map(member)).Returns(dto);
        _profileServiceMock
            .Setup(p =>
                p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new GetProfilesResponse
                {
                    Profiles =
                    {
                        new GetProfileResponse
                        {
                            Id = dto.ProfileId.ToString(),
                            FullName = "Иванов Иван Иванович",
                        },
                    },
                }
            );

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.TotalItems.ShouldBe(1);
        result.PageIndex.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        result[0].FullName.ShouldBe("Иванов Иван Иванович");
        result[0].AvatarUrl.ShouldBeNull();
    }

    [Test]
    public async Task GivenMembersWithAvatar_WhenHandling_ThenShouldReturnDtoWithAvatarUrl()
    {
        const string avatarUrl = "https://cdn.example.com/avatars/ivan.jpg";
        var member = CreateMember(_organizationId);
        var dto = CreateDto(member.Id);
        var query = new GetOrganizationMembersQuery(PageIndex: 1, PageSize: 10);
        var profileResponse = new GetProfileResponse
        {
            Id = dto.ProfileId.ToString(),
            FullName = "Иванов Иван Иванович",
            AvatarUrl = avatarUrl,
        };

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([member]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map(member)).Returns(dto);
        _profileServiceMock
            .Setup(p =>
                p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new GetProfilesResponse { Profiles = { profileResponse } });

        var result = await _handler.Handle(query, CancellationToken.None);

        result[0].AvatarUrl.ShouldBe(avatarUrl);
    }

    [Test]
    public async Task GivenNoMembers_WhenHandling_ThenShouldReturnEmptyPagedResult()
    {
        var query = new GetOrganizationMembersQuery();

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.ShouldBeEmpty();
        result.TotalItems.ShouldBe(0);
        _profileServiceMock.Verify(
            p => p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenPageIndexBelowOne_WhenHandling_ThenShouldClampToOne()
    {
        var query = new GetOrganizationMembersQuery(PageIndex: -3, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageIndex.ShouldBe(1);
    }

    [Test]
    public async Task GivenPageSizeAbove100_WhenHandling_ThenShouldClampTo100()
    {
        var query = new GetOrganizationMembersQuery(PageIndex: 1, PageSize: 999);

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageSize.ShouldBe(100);
    }

    [Test]
    public async Task GivenProfileServiceReturnsNull_WhenHandling_ThenShouldThrowArgumentNullException()
    {
        var member = CreateMember(_organizationId);
        var dto = CreateDto(member.Id);
        var query = new GetOrganizationMembersQuery(PageIndex: 1, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([member]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map(member)).Returns(dto);
        _profileServiceMock
            .Setup(p =>
                p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((GetProfilesResponse?)null);

        await Should.ThrowAsync<ArgumentNullException>(() =>
            _handler.Handle(query, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenMemberProfileMissingInResponse_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var member = CreateMember(_organizationId);
        var dto = CreateDto(member.Id);
        var query = new GetOrganizationMembersQuery(PageIndex: 1, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([member]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map(member)).Returns(dto);
        _profileServiceMock
            .Setup(p =>
                p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new GetProfilesResponse()); // пустой ответ — профиль участника не найден

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None).AsTask()
        );
    }

    private static OrganizationMember CreateMember(Guid orgId) =>
        new(orgId, Guid.CreateVersion7(), Guid.CreateVersion7(), new DateOnly(2025, 1, 1));

    private static OrganizationMemberDto CreateDto(Guid id) =>
        new(id, Guid.CreateVersion7(), "admin", OrganizationStatus.Active);
}
