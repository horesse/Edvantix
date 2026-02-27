using Edvantix.Constants.Other;

namespace Edvantix.Contracts;

/// <summary>
/// Интеграционное событие для создания in-app уведомления.
/// Публикуется другими сервисами (Organizational, Blog и т.д.);
/// потребляется Notification-сервисом через MassTransit.
/// </summary>
public sealed record SendInAppNotificationIntegrationEvent(
    Guid AccountId,
    NotificationType Type,
    string Title,
    string MessageText
) : IntegrationEvent;
