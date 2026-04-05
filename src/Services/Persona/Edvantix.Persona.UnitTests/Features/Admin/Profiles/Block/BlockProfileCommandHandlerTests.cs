using Edvantix.Chassis.Specification;
using Edvantix.Contracts;
using Edvantix.Persona.Features.Admin.Profiles.Block;

namespace Edvantix.Persona.UnitTests.Features.Admin.Profiles.Block;

public sealed class BlockProfileCommandHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IPublishEndpoint> _publishEndpointMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly BlockProfileCommandHandler _handler;

    public BlockProfileCommandHandlerTests()
    {
        _profileRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

        _handler = new(
            _profileRepoMock.Object,
            _publishEndpointMock.Object,
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
        _publishEndpointMock.Verify(
            p =>
                p.Publish(
                    It.Is<DisableKeycloakUserIntegrationEvent>(e =>
                        e.AccountId == profile.AccountId
                    ),
                    It.IsAny<CancellationToken>()
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

        _publishEndpointMock.Verify(
            p =>
                p.Publish(
                    It.IsAny<DisableKeycloakUserIntegrationEvent>(),
                    It.IsAny<CancellationToken>()
                ),
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
