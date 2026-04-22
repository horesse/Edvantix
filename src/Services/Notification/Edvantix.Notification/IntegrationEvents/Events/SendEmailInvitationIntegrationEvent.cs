namespace Edvantix.Contracts;

/// <summary>
/// Интеграционное событие для отправки email-приглашения в организацию.
/// Публикуется Organizational-сервисом; потребляется Notification-сервисом через MassTransit.
/// </summary>
public sealed record SendEmailInvitationIntegrationEvent : IntegrationEvent
{
    /// <summary>Идентификатор приглашения — для идемпотентности.</summary>
    public required Guid InvitationId { get; init; }

    /// <summary>Email адресата.</summary>
    public required string Email { get; init; }

    /// <summary>Plaintext-токен для формирования ссылки-приглашения.</summary>
    public required string Token { get; init; }

    /// <summary>Идентификатор организации.</summary>
    public required Guid OrganizationId { get; init; }

    /// <summary>Дата истечения срока действия приглашения (UTC).</summary>
    public required DateTime ExpiresAt { get; init; }
}
