using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Schedule.Configurations;

/// <summary>
/// Настройки приложения для сервиса расписаний.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ScheduleAppSettings : AppSettings
{
    /// <inheritdoc />
    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Schedule Service API",
            Summary = "Сервис управления расписаниями учебных групп",
            Description =
                "Управление расписаниями (слоты, исключения, рекуррентность), материализованными занятиями и праздничными днями.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
