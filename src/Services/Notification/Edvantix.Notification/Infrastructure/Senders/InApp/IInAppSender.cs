namespace Edvantix.Notification.Infrastructure.Senders.InApp;

/// <summary>
/// Интерфейс отправителя in-app уведомлений.
/// Следует тому же паттерну, что и <see cref="ISender"/> для email.
/// </summary>
public interface IInAppSender
{
    /// <summary>Создаёт и сохраняет in-app уведомление для пользователя.</summary>
    Task SendAsync(InAppNotificationMessage message, CancellationToken cancellationToken = default);
}
