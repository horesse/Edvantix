using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Profiles.GetProfile;
using Edvantix.Persona.UnitTests.Helpers;

namespace Edvantix.Persona.UnitTests.Features.Profiles.GetProfile;

public sealed class GetProfileQueryHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IMapper<Profile, ProfileViewModel>> _mapperMock = new();

    [Test]
    public async Task GivenProfileId_WhenHandling_ThenShouldFindProfileAndReturnViewModel()
    {
        var profileId = Guid.CreateVersion7();
        var profile = CreateProfile(profileId);
        var expectedViewModel = new ProfileViewModel(profileId, "Иванов Иван", "testuser", null);
        var handler = CreateHandler(Guid.CreateVersion7());

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _mapperMock.Setup(m => m.Map(profile)).Returns(expectedViewModel);

        var result = await handler.Handle(
            new GetProfileQuery(ProfileId: profileId),
            CancellationToken.None
        );

        result.ShouldBe(expectedViewModel);
        _profileRepoMock.Verify(
            r => r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenAccountId_WhenHandling_ThenShouldFindProfileByAccountIdAndReturnViewModel()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(Guid.CreateVersion7());
        var expectedViewModel = new ProfileViewModel(profile.Id, "Петров Пётр", "petrov", null);
        var handler = CreateHandler(Guid.CreateVersion7());

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _mapperMock.Setup(m => m.Map(profile)).Returns(expectedViewModel);

        var result = await handler.Handle(
            new GetProfileQuery(AccountId: accountId),
            CancellationToken.None
        );

        result.ShouldBe(expectedViewModel);
    }

    [Test]
    public async Task GivenNoIds_WhenHandling_ThenShouldUseCurrentUserAccountId()
    {
        var currentUserAccountId = Guid.CreateVersion7();
        var profile = CreateProfile(Guid.CreateVersion7(), currentUserAccountId);
        var expectedViewModel = new ProfileViewModel(
            profile.Id,
            "Смирнов Алексей",
            "smirnov",
            null
        );
        var handler = CreateHandler(currentUserAccountId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _mapperMock.Setup(m => m.Map(profile)).Returns(expectedViewModel);

        var result = await handler.Handle(new GetProfileQuery(), CancellationToken.None);

        result.ShouldBe(expectedViewModel);
        _profileRepoMock.Verify(
            r => r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenProfileNotFound_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var handler = CreateHandler(Guid.CreateVersion7());

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Profile?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(
                    new GetProfileQuery(ProfileId: Guid.CreateVersion7()),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    private GetProfileQueryHandler CreateHandler(Guid currentUserAccountId)
    {
        var providerMock = new Mock<IServiceProvider>();
        providerMock.SetupUser(currentUserAccountId);
        providerMock.SetupService<IProfileRepository>(_profileRepoMock.Object);
        providerMock.SetupService<IMapper<Profile, ProfileViewModel>>(_mapperMock.Object);

        return new GetProfileQueryHandler(providerMock.Object);
    }

    private static Profile CreateProfile(Guid profileId, Guid? accountId = null)
    {
        var profile = new Profile(
            accountId ?? Guid.CreateVersion7(),
            "testuser",
            Gender.Male,
            new DateOnly(1990, 1, 1),
            "Иван",
            "Иванов"
        );
        profile.Id = profileId;

        return profile;
    }
}
