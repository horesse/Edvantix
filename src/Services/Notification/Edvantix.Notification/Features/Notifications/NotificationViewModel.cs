namespace Edvantix.Notification.Features.Notifications;

/// <summary>
/// ViewModel in-app уведомления для REST API.
/// Возвращается как элемент <see cref="PagedResult{T}"/>.
/// </summary>
public sealed record NotificationViewModel
{
    public required Guid Id { get; init; }
    public required int Type { get; init; }
    public required string Title { get; init; }
    public required string Message { get; init; }
    public string? Metadata { get; init; }
    public required bool IsRead { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? ReadAt { get; init; }

    /// <summary>Маппинг из доменной модели в ViewModel.</summary>
    public static NotificationViewModel FromDomain(InAppNotification n) =>
        new()
        {
            Id = n.Id,
            Type = (int)n.Type,
            Title = n.Title,
            Message = n.Message,
            Metadata = n.Metadata,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            ReadAt = n.ReadAt,
        };
}
