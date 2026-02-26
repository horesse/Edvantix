namespace Edvantix.Notification.Infrastructure.Senders.InApp;

/// <summary>
/// Сообщение для отправки in-app уведомления.
/// Аналог <see cref="MimeKit.MimeMessage"/> для email-уведомлений.
/// </summary>
public sealed record InAppNotificationMessage
{
    /// <summary>Идентификатор Keycloak-аккаунта получателя.</summary>
    public required Guid AccountId { get; init; }

    /// <summary>Тип уведомления (определяет иконку и цвет на фронте).</summary>
    public required NotificationType Type { get; init; }

    /// <summary>Заголовок уведомления (макс. 100 символов).</summary>
    public required string Title { get; init; }

    /// <summary>Текст уведомления (макс. 10 000 символов).</summary>
    public required string Message { get; init; }

    /// <summary>Опциональные метаданные в формате JSON (ссылки, идентификаторы и т.д.).</summary>
    public string? Metadata { get; init; }
}
