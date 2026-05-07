using Wolverine.Attributes;

namespace Edvantix.Contracts;

[MessageIdentity("Edvantix.Contracts.SendInAppNotificationIntegrationEvent")]
public sealed record SendInAppNotificationIntegrationEvent : IntegrationEvent
{
    /// <summary>Идентификатор профиля получателя (Profile.Id).</summary>
    public required Guid ProfileId { get; init; }

    /// <summary>Тип уведомления (соответствует <c>NotificationType</c> enum).</summary>
    public required int Type { get; init; }

    /// <summary>Заголовок уведомления.</summary>
    public required string Title { get; init; }

    /// <summary>
    /// Текст уведомления.
    /// Назван <c>MessageText</c>, чтобы не конфликтовать с System.Messaging.
    /// </summary>
    public required string MessageText { get; init; }

    /// <summary>Опциональные метаданные в JSON-формате.</summary>
    public string? Metadata { get; init; }
}
