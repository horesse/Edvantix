using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Scheduling.Configurations;

/// <summary>
/// Strongly-typed application settings for the Scheduling service.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class SchedulingAppSettings : AppSettings
{
    /// <inheritdoc/>
    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Scheduling Service API",
            Summary = "Сервис расписания уроков",
            Description =
                "Управление слотами расписания, группами учеников и учителей.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
