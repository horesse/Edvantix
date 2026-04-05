using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Admin.Profiles.TrackLogin;
using Edvantix.Persona.UnitTests.Helpers;

namespace Edvantix.Persona.UnitTests.Features.Admin.Profiles.TrackLogin;

public sealed class RecordLastLoginCommandHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public RecordLastLoginCommandHandlerTests()
    {
        _profileRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task GivenProfileExists_WhenHandling_ThenShouldRecordLastLoginAndSave()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(accountId);
        var handler = CreateHandler(accountId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var before = DateTime.UtcNow;

        await handler.Handle(new RecordLastLoginCommand(), CancellationToken.None);

        profile.LastLoginAt.ShouldNotBeNull();
        profile.LastLoginAt.Value.ShouldBeGreaterThanOrEqualTo(before);
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenProfileNotFound_WhenHandling_ThenShouldReturnWithoutSaving()
    {
        var accountId = Guid.CreateVersion7();
        var handler = CreateHandler(accountId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Profile?)null);

        await handler.Handle(new RecordLastLoginCommand(), CancellationToken.None);

        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenClaimsWithoutProfileId_WhenHandling_ThenShouldReturnWithoutSaving()
    {
        var claims = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity()
        );
        var handler = new RecordLastLoginCommandHandler(_profileRepoMock.Object, claims);

        await handler.Handle(new RecordLastLoginCommand(), CancellationToken.None);

        _profileRepoMock.Verify(
            r => r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    private RecordLastLoginCommandHandler CreateHandler(Guid accountId)
    {
        var claims = ServiceProviderHelper.CreateClaimsPrincipal(accountId);
        return new RecordLastLoginCommandHandler(_profileRepoMock.Object, claims);
    }

    private static Profile CreateProfile(Guid accountId) =>
        new(accountId, "testuser", Gender.Male, new DateOnly(1990, 1, 1), "Иван", "Иванов")
        {
            Id = Guid.CreateVersion7(),
        };
}
