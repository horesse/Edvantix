using System.Net.Mime;

namespace Edvantix.Persona.Validators;

/// <summary>Validates uploaded image files. Enforces 1 MB size limit and JPEG/PNG content type.</summary>
public sealed class ImageValidator : AbstractValidator<IFormFile>
{
    private const int MaxFileSize = 1048576; // 1 MB

    public ImageValidator()
    {
        RuleFor(x => x.Length)
            .LessThanOrEqualTo(MaxFileSize)
            .WithMessage($"Размер файла не должен превышать {MaxFileSize / 1024 / 1024} МБ");

        RuleFor(x => x.ContentType)
            .Must(x => x is MediaTypeNames.Image.Jpeg or MediaTypeNames.Image.Png)
            .WithMessage("Допустимые форматы: JPEG, PNG");
    }
}
