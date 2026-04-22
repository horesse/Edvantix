using Edvantix.Constants.Other;
using Edvantix.SharedKernel.Helpers;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Notification.Domain.Models;

/// <summary>
/// Агрегат внутриприложенческого уведомления.
/// Хранит уведомление, адресованное конкретному пользователю (по Profile.Id).
/// </summary>
public sealed class InAppNotification() : IAggregateRoot
{
    /// <summary>
    /// Создаёт новое уведомление для пользователя.
    /// </summary>
    /// <param name="profileId">Идентификатор профиля получателя.</param>
    /// <param name="type">Тип уведомления (Info, Success, Warning и т.д.).</param>
    /// <param name="title">Краткий заголовок (до 100 символов).</param>
    /// <param name="message">Текст уведомления (до 10 000 символов).</param>
    /// <param name="metadata">Опциональные метаданные в формате JSON.</param>
    public InAppNotification(
        Guid profileId,
        NotificationType type,
        string title,
        string message,
        string? metadata = null
    )
        : this()
    {
        ProfileId = profileId;
        Type = type;
        Title = title;
        Message = message;
        Metadata = metadata;
        IsRead = false;
        CreatedAt = DateTimeHelper.UtcNow();
    }

    public Guid Id { get; private set; }

    /// <summary>Идентификатор профиля пользователя-получателя (Profile.Id).</summary>
    public Guid ProfileId { get; private set; }

    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;

    /// <summary>Произвольные метаданные в формате JSON (например, ссылки на сущности).</summary>
    public string? Metadata { get; private set; }

    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    /// <summary>
    /// Помечает уведомление как прочитанное, записывая текущее UTC-время.
    /// </summary>
    public InAppNotification MarkAsRead()
    {
        if (IsRead)
        {
            return this;
        }

        IsRead = true;
        ReadAt = DateTimeHelper.UtcNow();

        return this;
    }
}
