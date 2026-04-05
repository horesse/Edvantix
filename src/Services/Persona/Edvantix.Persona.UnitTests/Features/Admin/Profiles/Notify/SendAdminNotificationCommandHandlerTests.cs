using Edvantix.Chassis.Specification;
using Edvantix.Contracts;
using Edvantix.Persona.Features.Admin.Profiles.Notify;

namespace Edvantix.Persona.UnitTests.Features.Admin.Profiles.Notify;

public sealed class SendAdminNotificationCommandHandlerTests
{
    private readonly Mock<IProfileRepository> _profileRepoMock = new();
    private readonly Mock<IBus> _busMock = new();
    private readonly SendAdminNotificationCommandHandler _handler;

    public SendAdminNotificationCommandHandlerTests()
    {
        _handler = new(
            _profileRepoMock.Object,
            _busMock.Object,
            Mock.Of<ILogger<SendAdminNotificationCommandHandler>>()
        );
    }

    [Test]
    public async Task GivenExistingProfile_WhenSendingNotification_ThenShouldPublishIntegrationEvent()
    {
        var profileId = Guid.CreateVersion7();
        var profile = CreateProfile(profileId);
        var command = new SendAdminNotificationCommand(
            profileId,
            "Заголовок",
            "Текст сообщения",
            Constants.Other.NotificationType.Info
        );

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(profile);

        await _handler.Handle(command, CancellationToken.None);

        _busMock.Verify(
            b =>
                b.Publish(
                    It.Is<SendInAppNotificationIntegrationEvent>(e =>
                        e.AccountId == profile.AccountId
                        && e.Title == "Заголовок"
                        && e.MessageText == "Текст сообщения"
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenProfileNotFound_WhenSendingNotification_ThenShouldThrowNotFoundException()
    {
        var command = new SendAdminNotificationCommand(
            Guid.CreateVersion7(),
            "Заголовок",
            "Текст",
            Constants.Other.NotificationType.Info
        );

        _profileRepoMock
            .Setup(r =>
                r.FindAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((Profile?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );

        _busMock.Verify(
            b =>
                b.Publish(
                    It.IsAny<SendInAppNotificationIntegrationEvent>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    private static Profile CreateProfile(Guid profileId) =>
        new(
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
}
