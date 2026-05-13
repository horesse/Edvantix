namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Teachers;

public sealed class GetTeachersQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRepository> _repoMock = new();
    private readonly Mock<IProfileService> _profileServiceMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetTeachersQueryHandler _handler;

    public GetTeachersQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _profileServiceMock.Object);
    }

    [Test]
    public async Task GivenActiveMembersExist_WhenHandling_ThenShouldReturnEnrichedTeacherDtos()
    {
        var member = CreateMember();

        SetupRepo([member]);
        SetupProfileService(member.ProfileId, "Иванов Иван Иванович");

        var result = await _handler.Handle(new GetTeachersQuery(), CancellationToken.None);

        result.Count.ShouldBe(1);
        result.First().MemberId.ShouldBe(member.Id);
        result.First().FullName.ShouldBe("Иванов Иван Иванович");
    }

    [Test]
    public async Task GivenNoActiveMembers_WhenHandling_ThenShouldReturnEmptyCollectionWithoutCallingPersona()
    {
        SetupRepo([]);

        var result = await _handler.Handle(new GetTeachersQuery(), CancellationToken.None);

        result.ShouldBeEmpty();
        _profileServiceMock.Verify(
            p => p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenMemberWithAvatar_WhenHandling_ThenShouldIncludeAvatarUrl()
    {
        const string avatarUrl = "https://cdn.example.com/avatars/ivan.jpg";
        var member = CreateMember();

        SetupRepo([member]);
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
                            Id = member.ProfileId.ToString(),
                            FullName = "Иванов Иван",
                            AvatarUrl = avatarUrl,
                        },
                    },
                }
            );

        var result = await _handler.Handle(new GetTeachersQuery(), CancellationToken.None);

        result.First().AvatarUrl.ShouldBe(avatarUrl);
    }

    [Test]
    public async Task GivenMemberWithoutAvatar_WhenHandling_ThenShouldReturnNullAvatarUrl()
    {
        var member = CreateMember();

        SetupRepo([member]);
        SetupProfileService(member.ProfileId, "Иванов Иван");

        var result = await _handler.Handle(new GetTeachersQuery(), CancellationToken.None);

        result.First().AvatarUrl.ShouldBeNull();
    }

    [Test]
    public async Task GivenPersonaResponseIsNull_WhenHandling_ThenShouldThrowArgumentNullException()
    {
        var member = CreateMember();

        SetupRepo([member]);
        _profileServiceMock
            .Setup(p =>
                p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((GetProfilesResponse?)null);

        await Should.ThrowAsync<ArgumentNullException>(() =>
            _handler.Handle(new GetTeachersQuery(), CancellationToken.None).AsTask()
        );
    }

    private OrganizationMember CreateMember() =>
        new(
            _organizationId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            new DateOnly(2025, 1, 1)
        );

    private void SetupRepo(IReadOnlyCollection<OrganizationMember> members) =>
        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<OrganizationMember>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(members);

    private void SetupProfileService(Guid profileId, string fullName) =>
        _profileServiceMock
            .Setup(p =>
                p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new GetProfilesResponse
                {
                    Profiles =
                    {
                        new GetProfileResponse { Id = profileId.ToString(), FullName = fullName },
                    },
                }
            );
}
