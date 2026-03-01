using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Subscriptions.Configurations;

/// <summary>
/// Настройки приложения микросервиса Subscriptions.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class SubscriptionsAppSettings : AppSettings
{
    /// <summary>
    /// Продолжительность пробного периода в днях.
    /// </summary>
    public int TrialPeriodDays { get; set; } = 14;

    /// <summary>
    /// Продолжительность льготного периода в днях для просроченных подписок.
    /// </summary>
    public int GracePeriodDays { get; set; } = 7;

    /// <summary>
    /// Пороговый процент использования лимита для отправки предупреждений.
    /// </summary>
    public int LimitWarningThresholdPercent { get; set; } = 80;

    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Subscriptions Service API",
            Summary = "Сервис подписок",
            Description = "Управление подписками, тарифными планами и периодами.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
