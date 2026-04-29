using Edvantix.Constants.Other;
using Edvantix.Notification.Domain.Models;

namespace Edvantix.Notification.UnitTests.Domain;

public sealed class InAppNotificationSpecTests
{
    private static InAppNotification CreateNotification(Guid profileId, bool isRead = false)
    {
        var notification = new InAppNotification(
            profileId,
            NotificationType.Info,
            "Title",
            "Message"
        );

        if (isRead)
        {
            notification.MarkAsRead();
        }

        return notification;
    }

    // InAppNotificationsByAccountSpec

    [Test]
    public void GivenProfileId_WhenCreatingByAccountSpec_ThenShouldFilterByProfileId()
    {
        var profileId = Guid.NewGuid();
        var spec = new InAppNotificationsByAccountSpec(profileId, 1, 10);
        var filter = spec.WhereExpressions.First().Filter.Compile();

        filter(CreateNotification(profileId)).ShouldBeTrue();
        filter(CreateNotification(Guid.NewGuid())).ShouldBeFalse();
    }

    [Test]
    public void GivenNullIsRead_WhenCreatingByAccountSpec_ThenShouldHaveOnlyProfileFilter()
    {
        var spec = new InAppNotificationsByAccountSpec(Guid.NewGuid(), 1, 10, isRead: null);

        spec.WhereExpressions.Count().ShouldBe(1);
    }

    [Test]
    public void GivenIsReadFilter_WhenCreatingByAccountSpec_ThenShouldHaveTwoWhereExpressions()
    {
        var spec = new InAppNotificationsByAccountSpec(Guid.NewGuid(), 1, 10, isRead: true);

        spec.WhereExpressions.Count().ShouldBe(2);
    }

    [Test]
    public void GivenIsReadTrue_WhenEvaluatingByAccountSpec_ThenShouldMatchOnlyReadNotifications()
    {
        var profileId = Guid.NewGuid();
        var spec = new InAppNotificationsByAccountSpec(profileId, 1, 10, isRead: true);
        var isReadFilter = spec.WhereExpressions.Last().Filter.Compile();

        isReadFilter(CreateNotification(profileId, isRead: true)).ShouldBeTrue();
        isReadFilter(CreateNotification(profileId, isRead: false)).ShouldBeFalse();
    }

    [Test]
    public void GivenIsReadFalse_WhenEvaluatingByAccountSpec_ThenShouldMatchOnlyUnreadNotifications()
    {
        var profileId = Guid.NewGuid();
        var spec = new InAppNotificationsByAccountSpec(profileId, 1, 10, isRead: false);
        var isReadFilter = spec.WhereExpressions.Last().Filter.Compile();

        isReadFilter(CreateNotification(profileId, isRead: false)).ShouldBeTrue();
        isReadFilter(CreateNotification(profileId, isRead: true)).ShouldBeFalse();
    }

    // InAppNotificationsCountSpec

    [Test]
    public void GivenProfileId_WhenCreatingCountSpec_ThenShouldFilterByProfileId()
    {
        var profileId = Guid.NewGuid();
        var spec = new InAppNotificationsCountSpec(profileId);
        var filter = spec.WhereExpressions.First().Filter.Compile();

        filter(CreateNotification(profileId)).ShouldBeTrue();
        filter(CreateNotification(Guid.NewGuid())).ShouldBeFalse();
    }

    [Test]
    public void GivenNullIsRead_WhenCreatingCountSpec_ThenShouldHaveOnlyProfileFilter()
    {
        var spec = new InAppNotificationsCountSpec(Guid.NewGuid(), isRead: null);

        spec.WhereExpressions.Count().ShouldBe(1);
    }

    [Test]
    public void GivenIsReadFilter_WhenCreatingCountSpec_ThenShouldHaveTwoWhereExpressions()
    {
        var spec = new InAppNotificationsCountSpec(Guid.NewGuid(), isRead: false);

        spec.WhereExpressions.Count().ShouldBe(2);
    }

    // InAppNotificationByIdAndAccountSpec

    [Test]
    public void GivenMatchingIdAndProfileId_WhenEvaluatingByIdSpec_ThenShouldMatch()
    {
        var profileId = Guid.NewGuid();
        var notification = CreateNotification(profileId);
        // Id is Guid.Empty before EF persistence — using it as the known value in the spec
        var spec = new InAppNotificationByIdAndAccountSpec(Guid.Empty, profileId);
        var filter = spec.WhereExpressions.First().Filter.Compile();

        filter(notification).ShouldBeTrue();
    }

    [Test]
    public void GivenMismatchingProfileId_WhenEvaluatingByIdSpec_ThenShouldNotMatch()
    {
        var notification = CreateNotification(Guid.NewGuid());
        var spec = new InAppNotificationByIdAndAccountSpec(Guid.Empty, Guid.NewGuid());
        var filter = spec.WhereExpressions.First().Filter.Compile();

        filter(notification).ShouldBeFalse();
    }

    [Test]
    public void GivenMismatchingId_WhenEvaluatingByIdSpec_ThenShouldNotMatch()
    {
        var profileId = Guid.NewGuid();
        var notification = CreateNotification(profileId);
        var spec = new InAppNotificationByIdAndAccountSpec(Guid.NewGuid(), profileId);
        var filter = spec.WhereExpressions.First().Filter.Compile();

        filter(notification).ShouldBeFalse();
    }
}
