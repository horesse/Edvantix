using Wolverine.Attributes;

namespace Edvantix.Contracts;

[MessageIdentity("Edvantix.Contracts.SendEmailInvitationIntegrationEvent")]
public sealed record SendEmailInvitationIntegrationEvent : IntegrationEvent
{
    /// <summary>Идентификатор приглашения — для идемпотентности.</summary>
    public required Guid InvitationId { get; init; }

    /// <summary>Email адресата.</summary>
    public required string Email { get; init; }

    /// <summary>Plaintext-токен для включения в ссылку-приглашение.</summary>
    public required string Token { get; init; }

    /// <summary>Идентификатор организации.</summary>
    public required Guid OrganizationId { get; init; }

    /// <summary>Дата истечения срока действия приглашения.</summary>
    public required DateTime ExpiresAt { get; init; }
}
