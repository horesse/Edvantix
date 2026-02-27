namespace Edvantix.Constants.Other;

/// <summary>
/// Тип внутриприложенческого уведомления. Определяет иконку и цветовое оформление на фронтенде.
/// </summary>
public enum NotificationType
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,

    /// <summary>Приглашение в организацию или группу.</summary>
    Invitation = 4,

    /// <summary>Достижение или награда пользователя.</summary>
    Achievement = 5,

    /// <summary>Системное уведомление (техническое обслуживание, обновления).</summary>
    System = 6,
}
