using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Organizations.Configurations;

/// <summary>
/// Strongly-typed application settings for the Organizations service.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class OrganizationsAppSettings : AppSettings
{
    /// <inheritdoc/>
    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Organizations Service API",
            Summary = "Сервис организаций, ролей и разрешений",
            Description =
                "Управление школьными организациями, ролями пользователей и разрешениями.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
