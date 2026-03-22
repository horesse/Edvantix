using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Profiles.Details;
using Edvantix.Persona.UnitTests.Helpers;

namespace Edvantix.Persona.UnitTests.Features.Profiles.Details;

public sealed class GetProfileDetailsQueryHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IMapper<Profile, ProfileDetailsDto>> _mapperMock = new();

    [Test]
    public async Task GivenClaimsWithProfileId_WhenHandling_ThenShouldFindProfileAndReturnDetailsModel()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(Guid.CreateVersion7(), accountId);
        var expectedModel = BuildDetailsModel(profile.Id, accountId);
        var handler = CreateHandler(accountId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
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
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Profile?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            handler.Handle(new GetProfileDetailsQuery(), CancellationToken.None).AsTask()
        );
    }

    private GetProfileDetailsQueryHandler CreateHandler(Guid currentUserAccountId)
    {
        var claims = ServiceProviderHelper.CreateClaimsPrincipal(currentUserAccountId);

        return new GetProfileDetailsQueryHandler(
            _profileRepoMock.Object,
            claims,
            _mapperMock.Object
        );
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

    private static ProfileDetailsDto BuildDetailsModel(Guid id, Guid accountId) =>
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
