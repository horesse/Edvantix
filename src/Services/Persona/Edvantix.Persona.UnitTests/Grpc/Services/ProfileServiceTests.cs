using Edvantix.Chassis.Specification;
using Edvantix.Persona.UnitTests.Grpc.Context;
using Grpc.Core;

namespace Edvantix.Persona.UnitTests.Grpc.Services;

public sealed class ProfileServiceTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly ProfileService _service;

    public ProfileServiceTests()
    {
        _service = new(_profileRepoMock.Object);
    }

    [Test]
    public async Task GivenExistingProfile_WhenGetProfileCalled_ThenShouldReturnCorrectReply()
    {
        var profileId = Guid.CreateVersion7();
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(
            profileId,
            accountId,
            "john.doe",
            Gender.Male,
            new DateOnly(1990, 5, 20),
            "Иван",
            "Иванов",
            "Петрович"
        );
        var context = new TestServerCallContext();

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);

        var result = await _service.GetProfile(
            new GetProfileRequest { ProfileId = profileId.ToString() },
            context
        );

        result.ShouldNotBeNull();
        result.Id.ShouldBe(profileId.ToString());
        result.FirstName.ShouldBe("Иван");
        result.LastName.ShouldBe("Иванов");
        result.MiddleName.ShouldBe("Петрович");
        result.FullName.ShouldBe("Иванов Иван Петрович");
    }

    [Test]
    public async Task GivenProfileWithNoMiddleName_WhenGetProfileCalled_ThenMiddleNameShouldBeEmpty()
    {
        var profileId = Guid.CreateVersion7();
        var profile = CreateProfile(
            profileId,
            Guid.CreateVersion7(),
            "jane.doe",
            Gender.Female,
            new DateOnly(1995, 8, 12),
            "Анна",
            "Смирнова"
        );
        var context = new TestServerCallContext();

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);

        var result = await _service.GetProfile(
            new GetProfileRequest { ProfileId = profileId.ToString() },
            context
        );

        result.MiddleName.ShouldBe(string.Empty);
        result.FullName.ShouldBe("Смирнова Анна");
    }

    [Test]
    public async Task GivenNonExistentProfileId_WhenGetProfileCalled_ThenShouldThrowRpcExceptionWithNotFoundStatus()
    {
        var nonExistentId = Guid.CreateVersion7();
        var context = new TestServerCallContext();

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Profile?)null);

        var exception = await Should.ThrowAsync<RpcException>(async () =>
            await _service.GetProfile(
                new GetProfileRequest { ProfileId = nonExistentId.ToString() },
                context
            )
        );

        exception.StatusCode.ShouldBe(StatusCode.NotFound);
    }

    /// <summary>Creates a Profile and sets its Id via the public setter on Entity.</summary>
    private static Profile CreateProfile(
        Guid profileId,
        Guid accountId,
        string login,
        Gender gender,
        DateOnly birthDate,
        string firstName,
        string lastName,
        string? middleName = null
    )
    {
        var profile = new Profile(
            accountId,
            login,
            gender,
            birthDate,
            firstName,
            lastName,
            middleName
        );
        profile.Id = profileId;

        return profile;
    }
}
