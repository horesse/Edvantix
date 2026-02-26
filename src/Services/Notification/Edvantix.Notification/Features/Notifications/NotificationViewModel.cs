using Edvantix.Notification.Domain.Models;

namespace Edvantix.Notification.Features.Notifications;

/// <summary>
/// DTO уведомления для передачи на фронтенд.
/// </summary>
public sealed record NotificationViewModel
{
    public required Guid Id { get; init; }
    public required NotificationType Type { get; init; }
    public required string Title { get; init; }
    public required string Message { get; init; }
    public string? Metadata { get; init; }
    public required bool IsRead { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? ReadAt { get; init; }

    /// <summary>
    /// Маппит доменный агрегат в DTO для фронтенда.
    /// </summary>
    public static NotificationViewModel FromDomain(InAppNotification n) =>
        new()
        {
            Id = n.Id,
            Type = n.Type,
            Title = n.Title,
            Message = n.Message,
            Metadata = n.Metadata,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            ReadAt = n.ReadAt,
        };
}

/// <summary>
/// Страница уведомлений с метаданными пагинации.
/// </summary>
public sealed record NotificationPage
{
    public required IReadOnlyList<NotificationViewModel> Items { get; init; }
    public required int TotalCount { get; init; }
}
