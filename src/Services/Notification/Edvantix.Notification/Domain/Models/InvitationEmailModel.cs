using Edvantix.Notification.Infrastructure.Attributes;

namespace Edvantix.Notification.Domain.Models;

/// <summary>Модель данных для MJML-шаблона email-приглашения в организацию.</summary>
public sealed record InvitationEmailModel
{
    /// <summary>Email адресата.</summary>
    public required string Email { get; init; }

    /// <summary>Ссылка для принятия приглашения. Содержит plaintext-токен.</summary>
    public required string AcceptUrl { get; init; }

    /// <summary>Ссылка для отклонения приглашения. Содержит plaintext-токен.</summary>
    public required string DeclineUrl { get; init; }

    /// <summary>Дата истечения срока действия приглашения.</summary>
    [Format("{0:dd MMM yyyy}", "ru-RU")]
    public required DateTime ExpiresAt { get; init; }
}
