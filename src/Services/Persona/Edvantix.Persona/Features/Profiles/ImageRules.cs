using System.Collections.Frozen;
using System.Net.Mime;

namespace Edvantix.Persona.Features.Profiles;

internal static class ImageRules
{
    private const int MaxFileSize = 1048576;

    private static readonly FrozenDictionary<string, FrozenSet<string>> AllowedTypes =
        new Dictionary<string, FrozenSet<string>>
        {
            [MediaTypeNames.Image.Jpeg] = new[] { ".jpg", ".jpeg" }.ToFrozenSet(
                StringComparer.OrdinalIgnoreCase
            ),
            [MediaTypeNames.Image.Png] = new[] { ".png" }.ToFrozenSet(
                StringComparer.OrdinalIgnoreCase
            ),
            [MediaTypeNames.Image.Webp] = new[] { ".webp" }.ToFrozenSet(
                StringComparer.OrdinalIgnoreCase
            ),
        }.ToFrozenDictionary();

    public static void ApplyImageRules<T>(this IRuleBuilderInitial<T, IFormFile?> ruleBuilder)
    {
        ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(x => x?.Length > 0)
            .WithMessage("Файл не должен быть пустым.")
            .Must(x => x?.Length <= MaxFileSize)
            .WithMessage($"Размер файла не должен превышать {MaxFileSize / 1024} КБ.")
            .Must(x => x?.ContentType is not null && AllowedTypes.ContainsKey(x.ContentType))
            .WithMessage("Недопустимый тип файла. Разрешены только JPEG, PNG и WebP.")
            .Must(x =>
            {
                var ext = Path.GetExtension(x!.FileName);

                return AllowedTypes.TryGetValue(x.ContentType!, out var extensions)
                    && extensions.Contains(ext);
            })
            .WithMessage("Расширение файла не соответствует его типу содержимого.");
    }
}
