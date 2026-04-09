using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Admin.Profiles;
using Edvantix.Persona.Features.Admin.Profiles.Get;

namespace Edvantix.Persona.UnitTests.Features.Admin.Profiles.Get;

public sealed class GetAdminProfileQueryHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IMapper<Profile, AdminProfileDetailDto>> _mapperMock = new();
    private readonly GetAdminProfileQueryHandler _handler;

    public GetAdminProfileQueryHandlerTests()
    {
        _handler = new(_profileRepoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenExistingProfile_WhenHandling_ThenShouldReturnMappedDto()
    {
        var profileId = Guid.CreateVersion7();
        var profile = CreateProfile(profileId);
        var expectedDto = BuildDetailDto(profileId, profile.AccountId);
        var query = new GetAdminProfileQuery(profileId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _mapperMock.Setup(m => m.Map(profile)).Returns(expectedDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.ShouldBe(expectedDto);
    }

    [Test]
    public async Task GivenProfileNotFound_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var query = new GetAdminProfileQuery(Guid.CreateVersion7());

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Profile?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None).AsTask()
        );
    }

    private static Profile CreateProfile(Guid profileId)
    {
        var profile = new Profile(
            Guid.CreateVersion7(),
            "testuser",
            Gender.Male,
            new DateOnly(1990, 1, 1),
            "Иван",
            "Иванов"
        )
        {
            Id = profileId,
        };

        return profile;
    }

    private static AdminProfileDetailDto BuildDetailDto(Guid id, Guid accountId) =>
        new(
            id,
            accountId,
            "testuser",
            "Иван",
            "Иванов",
            null,
            Gender.Male,
            new DateOnly(1990, 1, 1),
            null,
            null,
            false,
            null,
            [],
            [],
            [],
            []
        );
}
