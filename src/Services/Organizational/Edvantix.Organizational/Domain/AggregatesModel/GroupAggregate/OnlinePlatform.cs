using System.ComponentModel.DataAnnotations;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>Платформа для проведения онлайн-занятий.</summary>
public enum OnlinePlatform
{
    /// <summary>Zoom.</summary>
    [Display(Name = "Zoom")]
    Zoom = 0,

    /// <summary>Google Meet.</summary>
    [Display(Name = "Google Meet")]
    GoogleMeet = 1,

    /// <summary>Telegram.</summary>
    [Display(Name = "Telegram")]
    Telegram = 2,

    /// <summary>Microsoft Teams.</summary>
    [Display(Name = "Microsoft Teams")]
    Teams = 3,
}
