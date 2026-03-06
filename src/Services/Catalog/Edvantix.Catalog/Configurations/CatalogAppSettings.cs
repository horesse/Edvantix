using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Catalog.Configurations;

[ExcludeFromCodeCoverage]
public sealed class CatalogAppSettings : AppSettings
{
    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Catalog Service API",
            Summary = "Сервис общих справочников",
            Description =
                "Управление глобальными справочниками: валюты, страны, часовые пояса, языки, регионы.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
