using Edvantix.Constants.Other;
using Wolverine.Attributes;

namespace Edvantix.Contracts;

[MessageIdentity("Edvantix.Contracts.SendInAppNotificationIntegrationEvent")]
public sealed record SendInAppNotificationIntegrationEvent(
    Guid ProfileId,
    NotificationType Type,
    string Title,
    string MessageText,
    string? Metadata = null
) : IntegrationEvent;
