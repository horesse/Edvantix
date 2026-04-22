using Edvantix.Organizational.Domain.Enums;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

/// <summary>
/// Вызывается при создании нового приглашения.
/// Содержит plaintext-токен — используется единожды для отправки уведомления; после этого хранится только хэш.
/// </summary>
public sealed class InvitationCreatedDomainEvent(
    Guid invitationId,
    Guid organizationId,
    Guid inviterProfileId,
    Guid roleId,
    InvitationType type,
    string token,
    string? email,
    string? inviteeLogin,
    Guid? inviteeProfileId,
    Guid? inviteeAccountId,
    DateTime expiresAt
) : DomainEvent
{
    public Guid InvitationId { get; } = invitationId;
    public Guid OrganizationId { get; } = organizationId;
    public Guid InviterProfileId { get; } = inviterProfileId;
    public Guid RoleId { get; } = roleId;
    public InvitationType Type { get; } = type;

    /// <summary>Plaintext-токен для включения в email-ссылку или in-app метаданные.</summary>
    public string Token { get; } = token;

    public string? Email { get; } = email;
    public string? InviteeLogin { get; } = inviteeLogin;
    public Guid? InviteeProfileId { get; } = inviteeProfileId;
    public Guid? InviteeAccountId { get; } = inviteeAccountId;
    public DateTime ExpiresAt { get; } = expiresAt;
}
