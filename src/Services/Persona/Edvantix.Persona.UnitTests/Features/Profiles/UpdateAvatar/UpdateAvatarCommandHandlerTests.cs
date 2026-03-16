using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Profiles.UpdateAvatar;
using Edvantix.Persona.UnitTests.Helpers;

namespace Edvantix.Persona.UnitTests.Features.Profiles.UpdateAvatar;

public sealed class UpdateAvatarCommandHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IBlobService> _blobServiceMock = new();
    private readonly Mock<IMapper<Profile, ProfileDetailsModel>> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public UpdateAvatarCommandHandlerTests()
    {
        _profileRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldUploadAvatarAndReturnDetailsModel()
    {
        var accountId = Guid.CreateVersion7();
        const string avatarUrn = "urn:blob:avatars/new.jpg";
        var profile = CreateProfile(accountId);
        var expectedModel = BuildDetailsModel(profile.Id, accountId);
        var handler = CreateHandler(accountId);
        var avatarMock = new Mock<IFormFile>();

        _profileRepoMock
            .Setup(r => r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _blobServiceMock
            .Setup(b => b.UploadFileAsync(avatarMock.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(avatarUrn);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map(profile)).Returns(expectedModel);

        var command = new UpdateAvatarCommand { Avatar = avatarMock.Object };

        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldBe(expectedModel);
        profile.AvatarUrl.ShouldBe(avatarUrn);
        _blobServiceMock.Verify(b => b.UploadFileAsync(avatarMock.Object, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
                new UpdateAvatarCommand { Avatar = new Mock<IFormFile>().Object },
                CancellationToken.None
            ).AsTask()
        );

        _blobServiceMock.Verify(
            b => b.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenSaveFails_WhenHandling_ThenShouldDeleteUploadedAvatarAndRethrow()
    {
        var accountId = Guid.CreateVersion7();
        const string newAvatarUrn = "urn:blob:avatars/new.jpg";
        var profile = CreateProfile(accountId);
        var handler = CreateHandler(accountId);
        var avatarMock = new Mock<IFormFile>();

        _profileRepoMock
            .Setup(r => r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _blobServiceMock
            .Setup(b => b.UploadFileAsync(avatarMock.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newAvatarUrn);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        await Should.ThrowAsync<Exception>(() =>
            handler.Handle(
                new UpdateAvatarCommand { Avatar = avatarMock.Object },
                CancellationToken.None
            ).AsTask()
        );

        _blobServiceMock.Verify(
            b => b.DeleteFileAsync(newAvatarUrn, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private UpdateAvatarCommandHandler CreateHandler(Guid accountId)
    {
        var providerMock = new Mock<IServiceProvider>();
        providerMock.SetupUser(accountId);
        providerMock.SetupService<IProfileRepository>(_profileRepoMock.Object);
        providerMock.SetupService<IBlobService>(_blobServiceMock.Object);
        providerMock.SetupService<IMapper<Profile, ProfileDetailsModel>>(_mapperMock.Object);

        return new UpdateAvatarCommandHandler(providerMock.Object);
    }

    private static Profile CreateProfile(Guid accountId)
    {
        var profile = new Profile(
            accountId, "testuser", Gender.Male, new DateOnly(1990, 1, 1), "Иван", "Иванов"
        );
        profile.Id = Guid.CreateVersion7();

        return profile;
    }

    private static ProfileDetailsModel BuildDetailsModel(Guid id, Guid accountId) =>
        new(id, accountId, "testuser", Gender.Male, new DateOnly(1990, 1, 1),
            "Иван", "Иванов", null, null, null, [], [], [], []);
}
