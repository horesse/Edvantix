using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Curriculum.Configurations;

/// <summary>
/// Настройки приложения для сервиса каталога учебных программ.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class CurriculumAppSettings : AppSettings
{
    /// <inheritdoc />
    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Curriculum Service API",
            Summary = "Сервис управления учебными программами, курсами и модулями",
            Description =
                "Каталог курсов, модулей и уроков. Предоставляет данные о программах обучения для сервисов организации и расписания.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
