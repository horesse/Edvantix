using Edvantix.Constants.Other;

namespace Edvantix.Contracts;

public sealed record SendInAppNotificationIntegrationEvent(
    Guid AccountId,
    NotificationType Type,
    string Title,
    string MessageText
) : IntegrationEvent;
