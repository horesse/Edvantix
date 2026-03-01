using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Organizational.Configurations;

[ExcludeFromCodeCoverage]
public sealed class OrganizationalAppSettings : AppSettings
{
    /// <summary>
    /// Срок действия SAS-токена для доступа к Blob Storage в часах.
    /// </summary>
    public int SasExpiryHours { get; set; }

    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Organizational Service API",
            Summary = "Сервис организаций",
            Description = "Управление организациями, группами и участниками организации.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
