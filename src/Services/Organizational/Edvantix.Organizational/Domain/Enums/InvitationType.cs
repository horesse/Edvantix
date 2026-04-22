namespace Edvantix.Organizational.Domain.Enums;

/// <summary>Способ доставки приглашения в организацию.</summary>
public enum InvitationType
{
    /// <summary>Приглашение отправляется на email — получатель переходит по ссылке с токеном.</summary>
    Email,

    /// <summary>Приглашение доставляется внутри приложения — адресат ищется по логину.</summary>
    InApp,
}
