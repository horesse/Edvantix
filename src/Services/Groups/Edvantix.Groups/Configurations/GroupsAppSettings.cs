using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.OpenApi;

namespace Edvantix.Groups.Configurations;

/// <summary>
/// Настройки приложения для сервиса групп.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class GroupsAppSettings : AppSettings
{
    /// <inheritdoc />
    public override OpenApiInfo? OpenApi { get; set; } =
        new()
        {
            Title = "Groups Service API",
            Summary = "Сервис управления группами обучающихся",
            Description =
                "Управление учебными группами: создание, обновление, состав участников и связанные операции.",
            Contact = new()
            {
                Name = "horesse",
                Email = "deepheath322@gmail.com",
                Url = new("https://github.com/Edvantix/Edvantix"),
            },
            License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") },
        };
}
