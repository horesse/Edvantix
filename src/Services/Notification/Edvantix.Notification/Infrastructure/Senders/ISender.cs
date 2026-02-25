namespace Edvantix.Notification.Infrastructure.Senders;

public interface ISender
{
    Task SendAsync(MimeMessage mailMessage, CancellationToken cancellationToken = default);
}
