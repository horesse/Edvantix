using System.ComponentModel.DataAnnotations;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>Формат проведения занятий группы.</summary>
public enum GroupFormat
{
    /// <summary>Занятия проходят в физическом кабинете (необходим <c>RoomId</c>).</summary>
    [Display(Name = "Очно")]
    Offline = 0,

    /// <summary>Занятия проходят онлайн (необходима <c>OnlinePlatform</c>).</summary>
    [Display(Name = "Онлайн")]
    Online = 1,

    /// <summary>Смешанный формат — часть занятий очно, часть онлайн.</summary>
    [Display(Name = "Смешанный")]
    Mixed = 2,
}
