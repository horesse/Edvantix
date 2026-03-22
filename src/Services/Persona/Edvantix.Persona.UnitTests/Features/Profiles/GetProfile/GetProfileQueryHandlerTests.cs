using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Profiles.Get;
using Edvantix.Persona.UnitTests.Helpers;

namespace Edvantix.Persona.UnitTests.Features.Profiles.GetProfile;

public sealed class GetProfileQueryHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IMapper<Profile, ProfileDto>> _mapperMock = new();

    [Test]
    public async Task GivenClaimsWithProfileId_WhenHandling_ThenShouldFindProfileAndReturnViewModel()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(Guid.CreateVersion7(), accountId);
        var expectedViewModel = new ProfileDto(profile.Id, "Иванов Иван", "testuser", null);
        var handler = CreateHandler(accountId);

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
            handler.Handle(new GetProfileQuery(), CancellationToken.None).AsTask()
        );
    }

    private GetProfileQueryHandler CreateHandler(Guid currentUserAccountId)
    {
        var claims = ServiceProviderHelper.CreateClaimsPrincipal(currentUserAccountId);

        return new GetProfileQueryHandler(_profileRepoMock.Object, claims, _mapperMock.Object);
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
