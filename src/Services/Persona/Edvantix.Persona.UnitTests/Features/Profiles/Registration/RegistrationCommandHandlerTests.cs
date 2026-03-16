using Edvantix.Chassis.Specification;
using Edvantix.Persona.Features.Profiles.Registration;
using Edvantix.Persona.UnitTests.Helpers;

namespace Edvantix.Persona.UnitTests.Features.Profiles.Registration;

public sealed class RegistrationCommandHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IBlobService> _blobServiceMock = new();
    private readonly Mock<IKeycloakAdminService> _keycloakMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public RegistrationCommandHandlerTests()
    {
        _profileRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCreateProfileAndReturnId()
    {
        var accountId = Guid.CreateVersion7();
        var handler = CreateHandler(accountId);
        var command = BuildCommand();

        _profileRepoMock
            .Setup(r => r.ExistsByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _profileRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (Profile p, CancellationToken _) =>
                {
                    p.Id = Guid.CreateVersion7(); // имитируем присваивание Id базой данных
                    return p;
                }
            );
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldNotBe(Guid.Empty);
        _profileRepoMock.Verify(
            r =>
                r.AddAsync(
                    It.Is<Profile>(p =>
                        p.FullName.FirstName == command.FirstName
                        && p.FullName.LastName == command.LastName
                        && p.Gender == command.Gender
                        && p.AccountId == accountId
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _keycloakMock.Verify(
            k => k.SetProfileIdAsync(accountId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenExistingProfileForAccount_WhenHandling_ThenShouldThrowInvalidOperationException()
    {
        var accountId = Guid.CreateVersion7();
        var handler = CreateHandler(accountId);

        _profileRepoMock
            .Setup(r => r.ExistsByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            handler.Handle(BuildCommand(), CancellationToken.None).AsTask()
        );

        _profileRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenCommandWithAvatar_WhenHandling_ThenShouldUploadAvatarBeforeSaving()
    {
        var accountId = Guid.CreateVersion7();
        const string avatarUrn = "urn:blob:avatars/photo.jpg";
        var avatarMock = new Mock<IFormFile>();
        var handler = CreateHandler(accountId);

        _profileRepoMock
            .Setup(r => r.ExistsByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _profileRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile p, CancellationToken _) => p);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _blobServiceMock
            .Setup(b => b.UploadFileAsync(avatarMock.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(avatarUrn);

        var command = BuildCommand(avatar: avatarMock.Object);

        await handler.Handle(command, CancellationToken.None);

        _blobServiceMock.Verify(
            b => b.UploadFileAsync(avatarMock.Object, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _profileRepoMock.Verify(
            r =>
                r.AddAsync(
                    It.Is<Profile>(p => p.AvatarUrl == avatarUrn),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenCommandWithAvatarAndSaveFails_WhenHandling_ThenShouldDeleteUploadedAvatar()
    {
        var accountId = Guid.CreateVersion7();
        const string avatarUrn = "urn:blob:avatars/photo.jpg";
        var avatarMock = new Mock<IFormFile>();
        var handler = CreateHandler(accountId);

        _profileRepoMock
            .Setup(r => r.ExistsByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _profileRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile p, CancellationToken _) => p);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));
        _blobServiceMock
            .Setup(b => b.UploadFileAsync(avatarMock.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(avatarUrn);

        var command = BuildCommand(avatar: avatarMock.Object);

        await Should.ThrowAsync<Exception>(() =>
            handler.Handle(command, CancellationToken.None).AsTask()
        );

        _blobServiceMock.Verify(
            b => b.DeleteFileAsync(avatarUrn, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenCommandWithoutAvatar_WhenSaveFails_ThenShouldNotCallDeleteFile()
    {
        var accountId = Guid.CreateVersion7();
        var handler = CreateHandler(accountId);

        _profileRepoMock
            .Setup(r => r.ExistsByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _profileRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile p, CancellationToken _) => p);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        await Should.ThrowAsync<Exception>(() =>
            handler.Handle(BuildCommand(), CancellationToken.None).AsTask()
        );

        _blobServiceMock.Verify(
            b => b.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    private RegistrationCommandHandler CreateHandler(Guid accountId, string login = "testuser")
    {
        var providerMock = new Mock<IServiceProvider>();
        providerMock.SetupUser(accountId, login);
        providerMock.SetupService<IProfileRepository>(_profileRepoMock.Object);
        providerMock.SetupService<IBlobService>(_blobServiceMock.Object);
        providerMock.SetupService<IKeycloakAdminService>(_keycloakMock.Object);

        return new RegistrationCommandHandler(providerMock.Object);
    }

    private static RegistrationCommand BuildCommand(IFormFile? avatar = null) =>
        new()
        {
            FirstName = "Иван",
            LastName = "Иванов",
            MiddleName = "Петрович",
            BirthDate = new DateOnly(1990, 6, 15),
            Gender = Gender.Male,
            Avatar = avatar,
        };
}
