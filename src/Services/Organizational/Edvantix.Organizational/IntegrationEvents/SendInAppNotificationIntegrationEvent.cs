using Edvantix.Constants.Other;

namespace Edvantix.Contracts;

/// <summary>
/// Интеграционное событие для создания in-app уведомления.
/// Публикуется Organizational-сервисом; потребляется Notification-сервисом через MassTransit.
/// </summary>
public sealed record SendInAppNotificationIntegrationEvent(
    Guid ProfileId,
    NotificationType Type,
    string Title,
    string MessageText,
    string? Metadata = null
) : IntegrationEvent;
