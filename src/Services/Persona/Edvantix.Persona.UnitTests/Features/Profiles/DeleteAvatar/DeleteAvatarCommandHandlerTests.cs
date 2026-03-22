using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Profiles.Avatar.DeleteAvatar;
using Edvantix.Persona.UnitTests.Helpers;

namespace Edvantix.Persona.UnitTests.Features.Profiles.DeleteAvatar;

public sealed class DeleteAvatarCommandHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public DeleteAvatarCommandHandlerTests()
    {
        _profileRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task GivenProfileWithAvatar_WhenHandling_ThenShouldDeleteAvatarAndReturnProfileId()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(accountId, avatarUrn: "urn:blob:avatars/photo.jpg");
        var handler = CreateHandler(accountId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await handler.Handle(new DeleteAvatarCommand(), CancellationToken.None);

        result.ShouldBe(accountId);
        profile.AvatarUrl.ShouldBeNull();
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenProfileWithoutAvatar_WhenHandling_ThenShouldSaveAndReturnProfileId()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(accountId, avatarUrn: null);
        var handler = CreateHandler(accountId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await handler.Handle(new DeleteAvatarCommand(), CancellationToken.None);

        result.ShouldBe(accountId);
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
            handler.Handle(new DeleteAvatarCommand(), CancellationToken.None).AsTask()
        );

        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    private DeleteAvatarCommandHandler CreateHandler(Guid accountId)
    {
        var claims = ServiceProviderHelper.CreateClaimsPrincipal(accountId);

        return new DeleteAvatarCommandHandler(_profileRepoMock.Object, claims);
    }

    private static Profile CreateProfile(Guid accountId, string? avatarUrn)
    {
        var profile = new Profile(
            accountId,
            "testuser",
            Gender.Male,
            new DateOnly(1990, 1, 1),
            "Иван",
            "Иванов"
        );
        profile.Id = Guid.CreateVersion7();

        if (avatarUrn is not null)
            profile.UploadAvatar(avatarUrn);

        return profile;
    }
}
