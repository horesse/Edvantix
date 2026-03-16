using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Profiles.DeleteAvatar;
using Edvantix.Persona.UnitTests.Helpers;

namespace Edvantix.Persona.UnitTests.Features.Profiles.DeleteAvatar;

public sealed class DeleteAvatarCommandHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IMapper<Profile, ProfileDetailsModel>> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public DeleteAvatarCommandHandlerTests()
    {
        _profileRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task GivenProfileWithAvatar_WhenHandling_ThenShouldDeleteAvatarAndReturnDetailsModel()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(accountId, avatarUrn: "urn:blob:avatars/photo.jpg");
        var expectedModel = BuildDetailsModel(profile.Id, accountId);
        var handler = CreateHandler(accountId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map(profile)).Returns(expectedModel);

        var result = await handler.Handle(new DeleteAvatarCommand(), CancellationToken.None);

        result.ShouldBe(expectedModel);
        profile.AvatarUrl.ShouldBeNull();
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenProfileWithoutAvatar_WhenHandling_ThenShouldSaveAndReturnDetailsModel()
    {
        var accountId = Guid.CreateVersion7();
        var profile = CreateProfile(accountId, avatarUrn: null);
        var expectedModel = BuildDetailsModel(profile.Id, accountId);
        var handler = CreateHandler(accountId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map(profile)).Returns(expectedModel);

        var result = await handler.Handle(new DeleteAvatarCommand(), CancellationToken.None);

        result.ShouldBe(expectedModel);
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
        var providerMock = new Mock<IServiceProvider>();
        providerMock.SetupUser(accountId);
        providerMock.SetupService<IProfileRepository>(_profileRepoMock.Object);
        providerMock.SetupService<IMapper<Profile, ProfileDetailsModel>>(_mapperMock.Object);

        return new DeleteAvatarCommandHandler(providerMock.Object);
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
