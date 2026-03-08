using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Blog.Configurations;

[ExcludeFromCodeCoverage]
internal sealed class BlogAppSettings : AppSettings
{
    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Blog Service API",
            Summary = "Сервис новостей",
            Description = "Постинг новостей, списков изменений.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
