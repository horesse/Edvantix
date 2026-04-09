using Edvantix.Notification.Infrastructure.Attributes;

namespace Edvantix.Notification.Domain.Models;

/// <summary>
///     Represents the data payload for a welcome notification email
///     sent to a user upon successful registration on the platform.
/// </summary>
public sealed record WelcomeNotification
{
    /// <summary>Gets or inits the recipient's full name.</summary>
    public required string FullName { get; init; }

    /// <summary>Gets or inits the recipient's email address.</summary>
    public required string Email { get; init; }

    /// <summary>
    ///     Gets or inits the date and time when the user joined the platform.
    ///     Formatted as <c>dd MMM yyyy HH:mm</c> (e.g. "25 Feb 2026 14:30").
    /// </summary>
    [Format("{0:dd MMM yyyy HH:mm}", "en-US")]
    public required DateTime JoinedAt { get; init; }
}
