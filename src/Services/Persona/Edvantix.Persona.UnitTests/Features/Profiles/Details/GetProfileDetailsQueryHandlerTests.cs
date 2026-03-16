using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Profiles.Details;
using Edvantix.Persona.UnitTests.Helpers;

namespace Edvantix.Persona.UnitTests.Features.Profiles.Details;

public sealed class GetProfileDetailsQueryHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IMapper<Profile, ProfileDetailsModel>> _mapperMock = new();

    [Test]
    public async Task GivenProfileId_WhenHandling_ThenShouldFindProfileAndReturnDetailsModel()
    {
        var profileId = Guid.CreateVersion7();
        var profile = CreateProfile(profileId);
        var expectedModel = BuildDetailsModel(profileId, profile.AccountId);
        var handler = CreateHandler(Guid.CreateVersion7());

        _profileRepoMock
            .Setup(r => r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _mapperMock.Setup(m => m.Map(profile)).Returns(expectedModel);

        var result = await handler.Handle(
            new GetProfileDetailsQuery(ProfileId: profileId),
            CancellationToken.None
        );

        result.ShouldBe(expectedModel);
    }

    [Test]
    public async Task GivenAccountId_WhenHandling_ThenShouldFindProfileByAccountIdAndReturnDetailsModel()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(Guid.CreateVersion7(), accountId);
        var expectedModel = BuildDetailsModel(profile.Id, accountId);
        var handler = CreateHandler(Guid.CreateVersion7());

        _profileRepoMock
            .Setup(r => r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _mapperMock.Setup(m => m.Map(profile)).Returns(expectedModel);

        var result = await handler.Handle(
            new GetProfileDetailsQuery(AccountId: accountId),
            CancellationToken.None
        );

        result.ShouldBe(expectedModel);
    }

    [Test]
    public async Task GivenNoIds_WhenHandling_ThenShouldUseCurrentUserAccountId()
    {
        var currentUserAccountId = Guid.CreateVersion7();
        var profile = CreateProfile(Guid.CreateVersion7(), currentUserAccountId);
        var expectedModel = BuildDetailsModel(profile.Id, currentUserAccountId);
        var handler = CreateHandler(currentUserAccountId);

        _profileRepoMock
            .Setup(r => r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _mapperMock.Setup(m => m.Map(profile)).Returns(expectedModel);

        var result = await handler.Handle(new GetProfileDetailsQuery(), CancellationToken.None);

        result.ShouldBe(expectedModel);
    }

    [Test]
    public async Task GivenProfileNotFound_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var handler = CreateHandler(Guid.CreateVersion7());

        _profileRepoMock
            .Setup(r => r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            handler.Handle(
                new GetProfileDetailsQuery(ProfileId: Guid.CreateVersion7()),
                CancellationToken.None
            ).AsTask()
        );
    }

    private GetProfileDetailsQueryHandler CreateHandler(Guid currentUserAccountId)
    {
        var providerMock = new Mock<IServiceProvider>();
        providerMock.SetupUser(currentUserAccountId);
        providerMock.SetupService<IProfileRepository>(_profileRepoMock.Object);
        providerMock.SetupService<IMapper<Profile, ProfileDetailsModel>>(_mapperMock.Object);

        return new GetProfileDetailsQueryHandler(providerMock.Object);
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

    private static ProfileDetailsModel BuildDetailsModel(Guid id, Guid accountId) =>
        new(
            id,
            accountId,
            "testuser",
            Gender.Male,
            new DateOnly(1990, 1, 1),
            "Иван",
            "Иванов",
            null,
            null,
            null,
            [],
            [],
            [],
            []
        );
}
