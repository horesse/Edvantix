namespace Edvantix.Notification.Features.Notifications.GetUnreadCount;

public sealed record UnreadCountViewModel(int Count);

public sealed record GetUnreadCountQuery(Guid ProfileId) : IQuery<UnreadCountViewModel>;

public sealed class GetUnreadCountQueryHandler(IInAppNotificationRepository repository)
    : IQueryHandler<GetUnreadCountQuery, UnreadCountViewModel>
{
    public async ValueTask<UnreadCountViewModel> Handle(
        GetUnreadCountQuery request,
        CancellationToken cancellationToken
    )
    {
        var spec = new InAppNotificationsCountSpec(request.ProfileId, isRead: false);
        var count = await repository.CountAsync(spec, cancellationToken);

        return new UnreadCountViewModel(count);
    }
}
