namespace Edvantix.Notification.Features.Notifications.MarkAllNotificationsAsRead;

public sealed record MarkAllNotificationsAsReadCommand(Guid ProfileId) : ICommand;

public sealed class MarkAllNotificationsAsReadCommandHandler(
    IInAppNotificationRepository repository
) : ICommandHandler<MarkAllNotificationsAsReadCommand>
{
    public async ValueTask<Unit> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken
    )
    {
        await repository.MarkAllAsReadAsync(request.ProfileId, cancellationToken);

        return Unit.Value;
    }
}
