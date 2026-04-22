namespace Edvantix.Organizational.Domain.Enums;

/// <summary>Статус приглашения в организацию.</summary>
public enum InvitationStatus
{
    /// <summary>Ожидает ответа от приглашённого.</summary>
    Pending,

    /// <summary>Приглашение принято — участник добавлен в организацию.</summary>
    Accepted,

    /// <summary>Приглашение отклонено приглашённым.</summary>
    Declined,

    /// <summary>Приглашение отозвано отправителем до истечения срока.</summary>
    Revoked,

    /// <summary>Срок действия приглашения истёк без ответа.</summary>
    Expired,
}
