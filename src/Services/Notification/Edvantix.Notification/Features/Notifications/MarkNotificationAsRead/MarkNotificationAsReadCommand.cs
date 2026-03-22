namespace Edvantix.Notification.Features.Notifications.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(Guid NotificationId, Guid AccountId)
    : ICommand<bool>;

public sealed class MarkNotificationAsReadCommandHandler(IInAppNotificationRepository repository)
    : ICommandHandler<MarkNotificationAsReadCommand, bool>
{
    public async ValueTask<bool> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken
    )
    {
        var spec = new InAppNotificationByIdAndAccountSpec(
            request.NotificationId,
            request.AccountId
        );
        var notification = await repository.FindAsync(spec, cancellationToken);

        if (notification is null)
        {
            return false;
        }

        notification.MarkAsRead();
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
