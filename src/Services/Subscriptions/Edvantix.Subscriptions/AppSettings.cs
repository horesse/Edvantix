namespace Edvantix.Subscriptions;

/// <summary>
/// Application settings for the Subscriptions service.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AppSettings
{
    /// <summary>
    /// Gets or sets the default trial period duration in days.
    /// </summary>
    public int TrialPeriodDays { get; set; } = 14;

    /// <summary>
    /// Gets or sets the grace period duration in days for past due subscriptions.
    /// </summary>
    public int GracePeriodDays { get; set; } = 7;

    /// <summary>
    /// Gets or sets the threshold percentage for limit warning notifications.
    /// </summary>
    public int LimitWarningThresholdPercent { get; set; } = 80;
}
