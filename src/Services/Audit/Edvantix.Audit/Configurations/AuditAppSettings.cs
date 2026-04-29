using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Audit.Configurations;

/// <summary>
/// Настройки приложения для сервиса аудита.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AuditAppSettings : AppSettings
{
    /// <inheritdoc />
    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Audit Service API",
            Summary = "Сервис аудита действий пользователей в организации",
            Description =
                "Запись, хранение и просмотр журнала аудита действий пользователей платформы.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
