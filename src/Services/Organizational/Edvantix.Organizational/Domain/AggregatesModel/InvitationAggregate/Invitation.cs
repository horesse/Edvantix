using Edvantix.Organizational.Domain.Enums;
using Edvantix.Organizational.Domain.Events;
using Edvantix.SharedKernel.Helpers;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

/// <summary>
/// Агрегат приглашения в организацию.
/// Поддерживает два канала доставки: <see cref="InvitationType.Email"/> и <see cref="InvitationType.InApp"/>.
/// Токен хранится только в виде SHA-256 хэша — защита на случай компрометации БД.
/// </summary>
public sealed class Invitation() : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
    /// <summary>
    /// Создаёт новое приглашение.
    /// Для email-типа обязателен <paramref name="email"/>;
    /// для in-app — <paramref name="inviteeLogin"/>, <paramref name="inviteeProfileId"/> и <paramref name="inviteeAccountId"/>.
    /// </summary>
    public Invitation(
        Guid organizationId,
        Guid inviterProfileId,
        Guid roleId,
        InvitationType type,
        DateTime expiresAt,
        string? email = null,
        string? inviteeLogin = null,
        Guid? inviteeProfileId = null,
        Guid? inviteeAccountId = null
    )
        : this()
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );
        if (inviterProfileId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор отправителя не может быть пустым.",
                nameof(inviterProfileId)
            );
        if (roleId == Guid.Empty)
            throw new ArgumentException("Идентификатор роли не может быть пустым.", nameof(roleId));
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Дата истечения должна быть в будущем.", nameof(expiresAt));

        if (type == InvitationType.Email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Email обязателен для email-приглашения.",
                    nameof(email)
                );
        }
        else
        {
            if (string.IsNullOrWhiteSpace(inviteeLogin))
                throw new ArgumentException(
                    "Логин обязателен для in-app-приглашения.",
                    nameof(inviteeLogin)
                );
            if (inviteeProfileId is null || inviteeProfileId == Guid.Empty)
                throw new ArgumentException(
                    "ProfileId приглашённого обязателен для in-app-приглашения.",
                    nameof(inviteeProfileId)
                );
            if (inviteeAccountId is null || inviteeAccountId == Guid.Empty)
                throw new ArgumentException(
                    "AccountId приглашённого обязателен для in-app-приглашения.",
                    nameof(inviteeAccountId)
                );
        }

        var token = InvitationToken.Generate();

        Id = Guid.CreateVersion7();
        OrganizationId = organizationId;
        InviterProfileId = inviterProfileId;
        RoleId = roleId;
        Type = type;
        ExpiresAt = expiresAt;
        TokenHash = token.Hash;
        Email = email?.ToLowerInvariant().Trim();
        InviteeLogin = inviteeLogin;
        InviteeProfileId = inviteeProfileId;
        InviteeAccountId = inviteeAccountId;
        Status = InvitationStatus.Pending;
        CreatedAt = DateTimeHelper.UtcNow();
        IsDeleted = false;

        RegisterDomainEvent(
            new InvitationCreatedDomainEvent(
                Id,
                organizationId,
                inviterProfileId,
                roleId,
                type,
                token.Value,
                email,
                inviteeLogin,
                inviteeProfileId,
                inviteeAccountId,
                expiresAt
            )
        );
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>ProfileId пользователя, отправившего приглашение.</summary>
    public Guid InviterProfileId { get; private set; }

    /// <summary>Роль, которая будет назначена при принятии приглашения.</summary>
    public Guid RoleId { get; private set; }

    /// <summary>Способ доставки приглашения.</summary>
    public InvitationType Type { get; private set; }

    /// <summary>Текущий статус приглашения.</summary>
    public InvitationStatus Status { get; private set; }

    /// <summary>SHA-256 хэш одноразового токена. Используется для проверки ссылки из письма.</summary>
    public string TokenHash { get; private set; } = null!;

    /// <summary>Email адресата (только для <see cref="InvitationType.Email"/>).</summary>
    public string? Email { get; private set; }

    /// <summary>Логин адресата в системе (только для <see cref="InvitationType.InApp"/>).</summary>
    public string? InviteeLogin { get; private set; }

    /// <summary>ProfileId адресата (только для <see cref="InvitationType.InApp"/>).</summary>
    public Guid? InviteeProfileId { get; private set; }

    /// <summary>AccountId (Keycloak) адресата — нужен для доставки in-app уведомления.</summary>
    public Guid? InviteeAccountId { get; private set; }

    /// <summary>Дата и время истечения срока действия приглашения (UTC).</summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>Дата и время создания приглашения (UTC).</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Дата и время принятия приглашения (UTC). Null до принятия.</summary>
    public DateTime? AcceptedAt { get; private set; }

    /// <summary>ProfileId пользователя, принявшего приглашение.</summary>
    public Guid? AcceptedByProfileId { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>Возвращает true, если срок действия приглашения истёк.</summary>
    public bool IsExpired => DateTimeHelper.UtcNow() > ExpiresAt;

    /// <summary>
    /// Принимает приглашение. Автоматически создаёт участника организации через доменное событие.
    /// </summary>
    /// <param name="profileId">ProfileId пользователя, принимающего приглашение.</param>
    /// <exception cref="InvalidOperationException">Если приглашение уже обработано или истёкло.</exception>
    public void Accept(Guid profileId)
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException(
                "Приглашение уже обработано и не может быть принято повторно."
            );
        if (IsExpired)
            throw new InvalidOperationException("Срок действия приглашения истёк.");

        Status = InvitationStatus.Accepted;
        AcceptedAt = DateTimeHelper.UtcNow();
        AcceptedByProfileId = profileId;

        RegisterDomainEvent(new InvitationAcceptedDomainEvent(OrganizationId, profileId, RoleId));
    }

    /// <summary>Отклоняет приглашение.</summary>
    /// <exception cref="InvalidOperationException">Если приглашение уже обработано.</exception>
    public void Decline()
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException(
                "Приглашение уже обработано и не может быть отклонено."
            );

        Status = InvitationStatus.Declined;
        RegisterDomainEvent(new InvitationDeclinedDomainEvent(OrganizationId, Id));
    }

    /// <summary>Отзывает приглашение отправителем.</summary>
    /// <exception cref="InvalidOperationException">Если приглашение уже обработано.</exception>
    public void Revoke()
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException("Можно отозвать только ожидающее приглашение.");

        Status = InvitationStatus.Revoked;
        RegisterDomainEvent(new InvitationRevokedDomainEvent(OrganizationId, Id));
    }

    /// <inheritdoc />
    public void Delete() => IsDeleted = true;
}
