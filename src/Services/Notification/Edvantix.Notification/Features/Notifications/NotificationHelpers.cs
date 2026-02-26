namespace Edvantix.Notification.Features.Notifications;

/// <summary>
/// Вспомогательные методы для эндпоинтов уведомлений.
/// </summary>
internal static class NotificationHelpers
{
    /// <summary>
    /// Извлекает Keycloak account_id из claim "sub" JWT-токена.
    /// </summary>
    internal static Guid GetAccountId(ClaimsPrincipal user)
    {
        var sub =
            user.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("Пользователь не аутентифицирован.");

        return Guid.TryParse(sub, out var id)
            ? id
            : throw new UnauthorizedAccessException(
                "Некорректный формат идентификатора пользователя."
            );
    }
}
