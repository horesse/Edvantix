namespace Edvantix.Contracts;

/// <summary>
/// Интеграционное событие для создания in-app уведомления.
/// Публикуется другими сервисами (Organizational, Blog и т.д.);
/// потребляется Notification-сервисом через MassTransit.
/// </summary>
public sealed record SendInAppNotificationIntegrationEvent : IntegrationEvent
{
    /// <summary>Keycloak account_id получателя.</summary>
    public required Guid AccountId { get; init; }

    /// <summary>Тип уведомления (соответствует <c>NotificationType</c> enum).</summary>
    public required int Type { get; init; }

    /// <summary>Заголовок уведомления.</summary>
    public required string Title { get; init; }

    /// <summary>
    /// Текст уведомления.
    /// Назван <c>MessageText</c>, чтобы не конфликтовать с System.Messaging.
    /// </summary>
    public required string MessageText { get; init; }

    /// <summary>Опциональные метаданные в JSON-формате.</summary>
    public string? Metadata { get; init; }
}
