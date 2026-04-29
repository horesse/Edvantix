using Edvantix.Constants.Other;
using Edvantix.Notification.Domain.Models;
using Edvantix.SharedKernel.Helpers;

namespace Edvantix.Notification.UnitTests.Domain;

public sealed class InAppNotificationTests
{
    [Test]
    public void GivenValidArguments_WhenCreated_ThenPropertiesShouldBeSet()
    {
        var profileId = Guid.NewGuid();
        const NotificationType type = NotificationType.Info;
        const string title = "Test Title";
        const string message = "Test Message";
        const string metadata = "{\"key\":\"value\"}";

        var notification = new InAppNotification(profileId, type, title, message, metadata);

        notification.ProfileId.ShouldBe(profileId);
        notification.Type.ShouldBe(type);
        notification.Title.ShouldBe(title);
        notification.Message.ShouldBe(message);
        notification.Metadata.ShouldBe(metadata);
        notification.IsRead.ShouldBeFalse();
        notification.ReadAt.ShouldBeNull();
        notification.CreatedAt.ShouldBeLessThanOrEqualTo(DateTimeHelper.UtcNow());
    }

    [Test]
    public void GivenNoMetadata_WhenCreated_ThenMetadataShouldBeNull()
    {
        var notification = new InAppNotification(
            Guid.NewGuid(),
            NotificationType.Info,
            "Title",
            "Message"
        );

        notification.Metadata.ShouldBeNull();
    }

    [Test]
    public void GivenUnreadNotification_WhenMarkAsRead_ThenIsReadShouldBeTrue()
    {
        var notification = new InAppNotification(
            Guid.NewGuid(),
            NotificationType.Success,
            "Title",
            "Message"
        );

        notification.MarkAsRead();

        notification.IsRead.ShouldBeTrue();
    }

    [Test]
    public void GivenUnreadNotification_WhenMarkAsRead_ThenReadAtShouldBeSet()
    {
        var notification = new InAppNotification(
            Guid.NewGuid(),
            NotificationType.Success,
            "Title",
            "Message"
        );

        notification.MarkAsRead();

        notification.ReadAt.ShouldNotBeNull();
        notification.ReadAt?.ShouldBeLessThanOrEqualTo(DateTimeHelper.UtcNow());
    }

    [Test]
    public void GivenUnreadNotification_WhenMarkAsRead_ThenShouldReturnSameInstance()
    {
        var notification = new InAppNotification(
            Guid.NewGuid(),
            NotificationType.Invitation,
            "Title",
            "Message"
        );

        var result = notification.MarkAsRead();

        result.ShouldBeSameAs(notification);
    }

    [Test]
    public void GivenAlreadyReadNotification_WhenMarkAsRead_ThenReadAtShouldNotChange()
    {
        var notification = new InAppNotification(
            Guid.NewGuid(),
            NotificationType.Warning,
            "Title",
            "Message"
        );
        notification.MarkAsRead();
        var firstReadAt = notification.ReadAt;

        notification.MarkAsRead();

        notification.ReadAt.ShouldBe(firstReadAt);
    }

    [Test]
    public void GivenAlreadyReadNotification_WhenMarkAsRead_ThenShouldReturnSameInstance()
    {
        var notification = new InAppNotification(
            Guid.NewGuid(),
            NotificationType.System,
            "Title",
            "Message"
        );
        notification.MarkAsRead();

        var result = notification.MarkAsRead();

        result.ShouldBeSameAs(notification);
    }
}
