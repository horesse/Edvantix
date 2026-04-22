using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Features.Invitations;

/// <summary>DTO приглашения в организацию.</summary>
/// <param name="Id">Уникальный идентификатор приглашения.</param>
/// <param name="OrganizationId">Идентификатор организации.</param>
/// <param name="InviterProfileId">ProfileId отправителя.</param>
/// <param name="RoleId">Роль, которая будет назначена при принятии.</param>
/// <param name="Type">Способ доставки (Email / InApp).</param>
/// <param name="Status">Текущий статус приглашения.</param>
/// <param name="Email">Email адресата (только для Email-приглашений).</param>
/// <param name="InviteeLogin">Логин адресата (только для InApp-приглашений).</param>
/// <param name="ExpiresAt">Дата истечения срока действия (UTC).</param>
/// <param name="CreatedAt">Дата создания (UTC).</param>
/// <param name="AcceptedAt">Дата принятия (UTC). Null до принятия.</param>
public sealed record InvitationDto(
    Guid Id,
    Guid OrganizationId,
    Guid InviterProfileId,
    Guid RoleId,
    InvitationType Type,
    InvitationStatus Status,
    string? Email,
    string? InviteeLogin,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    DateTime? AcceptedAt
);
