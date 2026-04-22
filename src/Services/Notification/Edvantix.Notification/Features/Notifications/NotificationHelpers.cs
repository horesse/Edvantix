using Edvantix.Chassis.Utilities;

namespace Edvantix.Notification.Features.Notifications;

/// <summary>
/// Вспомогательные методы для эндпоинтов уведомлений.
/// </summary>
internal static class NotificationHelpers
{
    /// <summary>
    /// Извлекает ProfileId из клейма "profile" JWT-токена.
    /// </summary>
    internal static Guid GetProfileId(ClaimsPrincipal user) => user.GetProfileIdOrError();
}
