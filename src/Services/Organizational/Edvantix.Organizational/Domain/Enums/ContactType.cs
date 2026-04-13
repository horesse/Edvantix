using System.ComponentModel.DataAnnotations;

namespace Edvantix.Organizational.Domain.Enums;

/// <summary>Тип контактных данных участника или организации.</summary>
public enum ContactType
{
    /// <summary>Адрес электронной почты.</summary>
    [Display(Name = "Электронная почта")]
    Email = 0,

    /// <summary>Номер мобильного телефона.</summary>
    [Display(Name = "Мобильный телефон")]
    MobilePhone = 1,

    /// <summary>Имя пользователя или номер в Telegram.</summary>
    [Display(Name = "Telegram")]
    Telegram = 2,

    /// <summary>Номер в WhatsApp.</summary>
    [Display(Name = "WhatsApp")]
    WhatsApp = 3,

    /// <summary>Номер в Viber.</summary>
    [Display(Name = "Viber")]
    Viber = 4,
}
