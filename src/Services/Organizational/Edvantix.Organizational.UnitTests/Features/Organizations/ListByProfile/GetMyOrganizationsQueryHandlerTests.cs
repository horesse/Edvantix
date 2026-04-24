using Edvantix.Chassis.Security.Keycloak;

namespace Edvantix.Organizational.UnitTests.Features.Organizations.ListByProfile;

public sealed class GetMyOrganizationsQueryHandlerTests
{
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock = new();
    private readonly Mock<IOrganizationRepository> _orgRepoMock = new();
    private readonly Guid _profileId = Guid.CreateVersion7();

    private static readonly Guid ValidCountryId = Guid.CreateVersion7();
    private static readonly Guid ValidCurrencyId = Guid.CreateVersion7();

    [Test]
    public async Task GivenActiveMember_WhenHandling_ThenShouldReturnOrganizationWithRole()
    {
        var handler = CreateHandler(_profileId);
        var orgId = Guid.CreateVersion7();
        var role = CreateRole(orgId, "teacher", "Преподаватель");
        var member = CreateMember(orgId, _profileId, role);
        var org = CreateOrganization(orgId);

        SetupMemberRepo([member]);
        SetupOrgRepo([org]);

        var result = await handler.Handle(new GetMyOrganizationsQuery(), CancellationToken.None);

        result.ShouldHaveSingleItem();
        result[0].Id.ShouldBe(orgId);
        result[0].RoleCode.ShouldBe("teacher");
        result[0].RoleDescription.ShouldBe("Преподаватель");
    }

    [Test]
    public async Task GivenActiveMember_WhenHandling_ThenShouldReturnCorrectOrganizationProperties()
    {
        var handler = CreateHandler(_profileId);
        var orgId = Guid.CreateVersion7();
        var role = CreateRole(orgId, "owner");
        var member = CreateMember(orgId, _profileId, role);
        var org = CreateOrganization(orgId);

        SetupMemberRepo([member]);
        SetupOrgRepo([org]);

        var result = await handler.Handle(new GetMyOrganizationsQuery(), CancellationToken.None);

        result[0].FullLegalName.ShouldBe("ООО Тестовая Компания");
        result[0].OrganizationType.ShouldBe(OrganizationType.PrivateEducationalCenter);
        result[0].Status.ShouldBe(OrganizationStatus.Active);
        result[0].IsLegalEntity.ShouldBeTrue();
    }

    [Test]
    public async Task GivenNoMemberships_WhenHandling_ThenShouldReturnEmptyList()
    {
        var handler = CreateHandler(_profileId);

        SetupMemberRepo([]);

        var result = await handler.Handle(new GetMyOrganizationsQuery(), CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenNoMemberships_WhenHandling_ThenShouldNotQueryOrganizationRepository()
    {
        var handler = CreateHandler(_profileId);

        SetupMemberRepo([]);

        await handler.Handle(new GetMyOrganizationsQuery(), CancellationToken.None);

        _orgRepoMock.Verify(
            r =>
                r.ListAsync(
                    It.IsAny<ISpecification<Organization>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Test]
    public async Task GivenMemberInMultipleOrganizations_WhenHandling_ThenShouldReturnAllOrganizations()
    {
        var handler = CreateHandler(_profileId);
        var orgId1 = Guid.CreateVersion7();
        var orgId2 = Guid.CreateVersion7();
        var member1 = CreateMember(orgId1, _profileId, CreateRole(orgId1, "owner"));
        var member2 = CreateMember(orgId2, _profileId, CreateRole(orgId2, "teacher"));

        SetupMemberRepo([member1, member2]);
        SetupOrgRepo([CreateOrganization(orgId1), CreateOrganization(orgId2)]);

        var result = await handler.Handle(new GetMyOrganizationsQuery(), CancellationToken.None);

        result.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenMemberWithNullRole_WhenHandling_ThenShouldExcludeMember()
    {
        var handler = CreateHandler(_profileId);
        var orgId = Guid.CreateVersion7();
        var memberWithoutRole = new OrganizationMember(
            orgId,
            _profileId,
            Guid.CreateVersion7(),
            DateOnly.FromDateTime(DateTime.Today)
        );

        SetupMemberRepo([memberWithoutRole]);
        SetupOrgRepo([CreateOrganization(orgId)]);

        var result = await handler.Handle(new GetMyOrganizationsQuery(), CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenOrganizationNotFoundForMember_WhenHandling_ThenShouldExcludeOrphanedMember()
    {
        var handler = CreateHandler(_profileId);
        var orgId = Guid.CreateVersion7();
        var member = CreateMember(orgId, _profileId, CreateRole(orgId, "teacher"));

        SetupMemberRepo([member]);
        SetupOrgRepo([]);

        var result = await handler.Handle(new GetMyOrganizationsQuery(), CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenUserWithoutProfileId_WhenHandling_ThenShouldThrowException()
    {
        var claims = new ClaimsPrincipal(new ClaimsIdentity());
        var handler = new GetMyOrganizationsQueryHandler(
            claims,
            _memberRepoMock.Object,
            _orgRepoMock.Object
        );

        await Should.ThrowAsync<Exception>(() =>
            handler.Handle(new GetMyOrganizationsQuery(), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenActiveMember_WhenHandling_ThenShouldPassProfileIdToMemberRepository()
    {
        var handler = CreateHandler(_profileId);
        ISpecification<OrganizationMember>? capturedSpec = null;

        _memberRepoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<ISpecification<OrganizationMember>, CancellationToken>(
                (spec, _) => capturedSpec = spec
            )
            .ReturnsAsync([]);

        await handler.Handle(new GetMyOrganizationsQuery(), CancellationToken.None);

        capturedSpec.ShouldBeOfType<OrganizationsByProfileSpecification>();
    }

    private GetMyOrganizationsQueryHandler CreateHandler(Guid profileId)
    {
        var claims = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(KeycloakClaimTypes.Profile, profileId.ToString())])
        );
        return new(claims, _memberRepoMock.Object, _orgRepoMock.Object);
    }

    private void SetupMemberRepo(IReadOnlyCollection<OrganizationMember> members) =>
        _memberRepoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(members);

    private void SetupOrgRepo(IReadOnlyCollection<Organization> organizations) =>
        _orgRepoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Organization>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(organizations);

    private static OrganizationMember CreateMember(
        Guid orgId,
        Guid profileId,
        OrganizationMemberRole role
    )
    {
        var member = new OrganizationMember(
            orgId,
            profileId,
            role.Id,
            DateOnly.FromDateTime(DateTime.Today)
        );

        typeof(OrganizationMember)
            .GetProperty(nameof(OrganizationMember.Role))!
            .SetValue(member, role);

        return member;
    }

    private static OrganizationMemberRole CreateRole(
        Guid orgId,
        string code,
        string? description = null
    ) => new(orgId, code, description);

    private static Organization CreateOrganization(Guid id) =>
        new Organization(
            "ООО Тестовая Компания",
            isLegalEntity: true,
            new DateOnly(2020, 1, 15),
            LegalForm.Llc,
            ValidCountryId,
            ValidCurrencyId,
            OrganizationType.PrivateEducationalCenter
        )
        {
            Id = id,
        };
}
