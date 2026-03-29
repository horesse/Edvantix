using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Organizational.Configurations;

/// <summary>
/// Настройки приложения для сервиса организаций.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class OrganizationalAppSettings : AppSettings
{
    /// <inheritdoc />
    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Organizational Service API",
            Summary = "Сервис управления организациями",
            Description = "Управление организациями, их участниками и структурой.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
