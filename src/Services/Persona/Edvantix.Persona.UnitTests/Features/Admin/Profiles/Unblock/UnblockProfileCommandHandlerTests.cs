using Edvantix.Chassis.Specification;
using Edvantix.Contracts;
using Edvantix.Persona.Features.Admin.Profiles.Unblock;

namespace Edvantix.Persona.UnitTests.Features.Admin.Profiles.Unblock;

public sealed class UnblockProfileCommandHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IPublishEndpoint> _publishEndpointMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly UnblockProfileCommandHandler _handler;

    public UnblockProfileCommandHandlerTests()
    {
        _profileRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

        _handler = new(
            _profileRepoMock.Object,
            _publishEndpointMock.Object,
            Mock.Of<ILogger<UnblockProfileCommandHandler>>()
        );
    }

    [Test]
    public async Task GivenBlockedProfile_WhenUnblocking_ThenShouldUnblockAndPublishEnableEvent()
    {
        var profileId = Guid.CreateVersion7();
        var profile = CreateBlockedProfile(profileId);
        var command = new UnblockProfileCommand(profileId);

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        profile.IsBlocked.ShouldBeFalse();
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _publishEndpointMock.Verify(
            p =>
                p.Publish(
                    It.Is<EnableKeycloakUserIntegrationEvent>(e =>
                        e.AccountId == profile.AccountId
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenProfileNotFound_WhenUnblocking_ThenShouldThrowNotFoundException()
    {
        var command = new UnblockProfileCommand(Guid.CreateVersion7());

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
                    It.IsAny<EnableKeycloakUserIntegrationEvent>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    private static Profile CreateBlockedProfile(Guid profileId)
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
        profile.Block();

        return profile;
    }
}
