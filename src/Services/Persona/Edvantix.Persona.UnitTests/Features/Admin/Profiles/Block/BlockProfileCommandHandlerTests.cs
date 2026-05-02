using Edvantix.Chassis.Specification;
using Edvantix.Contracts;
using Edvantix.Persona.Features.Admin.Profiles.Block;
using Wolverine;

namespace Edvantix.Persona.UnitTests.Features.Admin.Profiles.Block;

public sealed class BlockProfileCommandHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IMessageBus> _busMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly BlockProfileCommandHandler _handler;

    public BlockProfileCommandHandlerTests()
    {
        _profileRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

        _handler = new(
            _profileRepoMock.Object,
            _busMock.Object,
            Mock.Of<ILogger<BlockProfileCommandHandler>>()
        );
    }

    [Test]
    public async Task GivenExistingProfile_WhenBlocking_ThenShouldBlockAndPublishDisableEvent()
    {
        var profileId = Guid.CreateVersion7();
        var profile = CreateProfile(profileId);
        var command = new BlockProfileCommand(profileId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        profile.IsBlocked.ShouldBeTrue();
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _busMock.Verify(
            p =>
                p.PublishAsync(
                    It.Is<DisableKeycloakUserIntegrationEvent>(e =>
                        e.AccountId == profile.AccountId
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenProfileNotFound_WhenBlocking_ThenShouldThrowNotFoundException()
    {
        var command = new BlockProfileCommand(Guid.CreateVersion7());

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Profile?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );

        _busMock.Verify(
            p => p.PublishAsync(It.IsAny<DisableKeycloakUserIntegrationEvent>()),
            Times.Never
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
}
