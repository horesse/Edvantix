using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Persona.Configurations;

[ExcludeFromCodeCoverage]
public sealed class PersonaAppSettings : AppSettings
{
    /// <summary>
    /// Срок действия SAS-токена для доступа к Blob Storage в часах.
    /// </summary>
    public int SasExpiryHours { get; set; }

    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Persona Service API",
            Summary = "Сервис профилей пользователей",
            Description = "Управление профилями, аватарами и персональными данными пользователей.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
