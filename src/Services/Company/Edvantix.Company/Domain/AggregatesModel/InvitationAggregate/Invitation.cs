using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Events;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.SharedKernel.Helpers;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Company.Domain.AggregatesModel.InvitationAggregate;

/// <summary>
/// Приглашение в организацию. Статусный lifecycle: Pending → Accepted/Declined/Cancelled/Expired.
/// </summary>
public sealed class Invitation() : Entity<Guid>, IAggregateRoot
{
    /// <summary>
    /// Срок действия приглашения по умолчанию (7 дней).
    /// </summary>
    public const int DefaultTtlDays = 7;

    /// <summary>
    /// Создаёт приглашение в организацию.
    /// </summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="invitedByProfileId">Профиль пригласившего.</param>
    /// <param name="role">Роль, которая будет назначена при принятии.</param>
    /// <param name="inviteeEmail">Email приглашённого (опционально).</param>
    /// <param name="inviteeProfileId">Профиль приглашённого (опционально).</param>
    /// <param name="ttlDays">Срок действия в днях.</param>
    public Invitation(
        long organizationId,
        long invitedByProfileId,
        OrganizationRole role,
        string? inviteeEmail = null,
        long? inviteeProfileId = null,
        int ttlDays = DefaultTtlDays
    )
        : this()
    {
        if (organizationId <= 0)
            throw new ArgumentException(
                "Некорректный идентификатор организации.",
                nameof(organizationId)
            );

        if (invitedByProfileId <= 0)
            throw new ArgumentException(
                "Некорректный идентификатор профиля пригласившего.",
                nameof(invitedByProfileId)
            );

        if (inviteeEmail is null && inviteeProfileId is null)
            throw new ArgumentException(
                "Необходимо указать email или идентификатор профиля приглашённого."
            );

        if (ttlDays is < 1 or > 30)
            throw new ArgumentOutOfRangeException(
                nameof(ttlDays),
                "Срок действия должен быть от 1 до 30 дней."
            );

        var now = DateTimeHelper.UtcNow();

        OrganizationId = organizationId;
        InvitedByProfileId = invitedByProfileId;
        InviteeProfileId = inviteeProfileId;
        InviteeEmail = inviteeEmail?.Trim().ToLowerInvariant();
        Role = role;
        Status = InvitationStatus.Pending;
        Token = Guid.NewGuid();
        CreatedAt = now;
        ExpiresAt = now.AddDays(ttlDays);

        RegisterDomainEvent(
            new InvitationCreatedEvent(Id, OrganizationId, InviteeEmail, InviteeProfileId)
        );
    }

    public long OrganizationId { get; private set; }
    public long InvitedByProfileId { get; private set; }
    public long? InviteeProfileId { get; private set; }
    public string? InviteeEmail { get; private set; }
    public OrganizationRole Role { get; private set; }
    public InvitationStatus Status { get; private set; }

    /// <summary>
    /// Отдельный токен для ссылки accept/decline (≠ Id, для безопасности).
    /// </summary>
    public Guid Token { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RespondedAt { get; private set; }

    /// <summary>
    /// Проверяет, истёк ли срок действия приглашения.
    /// </summary>
    public bool IsExpired => Status == InvitationStatus.Pending && DateTimeHelper.UtcNow() > ExpiresAt;

    /// <summary>
    /// Принимает приглашение. Вызывающий код должен создать OrganizationMember.
    /// </summary>
    /// <param name="profileId">Профиль принимающего пользователя.</param>
    public void Accept(long profileId)
    {
        EnsurePendingAndNotExpired();

        if (InviteeProfileId.HasValue && InviteeProfileId.Value != profileId)
            throw new InvalidOperationException("Приглашение адресовано другому пользователю.");

        InviteeProfileId ??= profileId;
        Status = InvitationStatus.Accepted;
        RespondedAt = DateTimeHelper.UtcNow();

        RegisterDomainEvent(new InvitationAcceptedEvent(Id, OrganizationId, profileId));
    }

    /// <summary>
    /// Отклоняет приглашение.
    /// </summary>
    /// <param name="profileId">Профиль отклоняющего пользователя.</param>
    public void Decline(long profileId)
    {
        EnsurePendingAndNotExpired();

        if (InviteeProfileId.HasValue && InviteeProfileId.Value != profileId)
            throw new InvalidOperationException("Приглашение адресовано другому пользователю.");

        InviteeProfileId ??= profileId;
        Status = InvitationStatus.Declined;
        RespondedAt = DateTimeHelper.UtcNow();

        RegisterDomainEvent(new InvitationDeclinedEvent(Id, OrganizationId));
    }

    /// <summary>
    /// Отменяет приглашение (Owner/Manager).
    /// </summary>
    public void Cancel()
    {
        if (Status is not InvitationStatus.Pending)
            throw new InvalidOperationException(
                "Можно отменить только ожидающее приглашение."
            );

        Status = InvitationStatus.Cancelled;
        RespondedAt = DateTimeHelper.UtcNow();
    }

    /// <summary>
    /// Помечает приглашение как истёкшее (пассивная проверка).
    /// </summary>
    public void MarkExpired()
    {
        if (Status is not InvitationStatus.Pending)
            throw new InvalidOperationException(
                "Можно пометить истёкшим только ожидающее приглашение."
            );

        Status = InvitationStatus.Expired;
    }

    private void EnsurePendingAndNotExpired()
    {
        if (Status is not InvitationStatus.Pending)
            throw new InvalidOperationException("Приглашение уже обработано.");

        if (IsExpired)
        {
            MarkExpired();
            throw new InvalidOperationException("Срок действия приглашения истёк.");
        }
    }
}
