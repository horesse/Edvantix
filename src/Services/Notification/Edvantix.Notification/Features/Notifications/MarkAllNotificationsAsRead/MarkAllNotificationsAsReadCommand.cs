namespace Edvantix.Notification.Features.Notifications.MarkAllNotificationsAsRead;

public sealed record MarkAllNotificationsAsReadCommand(Guid AccountId) : ICommand;

public sealed class MarkAllNotificationsAsReadCommandHandler(
    IInAppNotificationRepository repository
) : ICommandHandler<MarkAllNotificationsAsReadCommand>
{
    public async ValueTask<Unit> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken
    )
    {
        await repository.MarkAllAsReadAsync(request.AccountId, cancellationToken);

        return Unit.Value;
    }
}
