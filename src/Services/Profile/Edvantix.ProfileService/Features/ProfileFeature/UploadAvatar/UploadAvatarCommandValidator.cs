using FluentValidation;

namespace Edvantix.ProfileService.Features.ProfileFeature.UploadAvatar;

/// <summary>
/// Валидатор для команды загрузки аватара
/// </summary>
public sealed class UploadAvatarCommandValidator : AbstractValidator<UploadAvatarCommand>
{
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp",
    ];

    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public UploadAvatarCommandValidator()
    {
        When(
            x => x.Image is not null,
            () =>
            {
                RuleFor(x => x.Image!.Length)
                    .LessThanOrEqualTo(MaxFileSizeInBytes)
                    .WithMessage(
                        $"Размер файла не должен превышать {MaxFileSizeInBytes / 1024 / 1024} МБ"
                    );

                RuleFor(x => x.Image!.ContentType)
                    .Must(contentType => AllowedContentTypes.Contains(contentType))
                    .WithMessage(
                        $"Недопустимый тип файла. Разрешены только: {string.Join(", ", AllowedContentTypes)}"
                    );

                RuleFor(x => x.Image!.FileName)
                    .Must(fileName =>
                    {
                        var extension = Path.GetExtension(fileName).ToLowerInvariant();
                        return AllowedExtensions.Contains(extension);
                    })
                    .WithMessage(
                        $"Недопустимое расширение файла. Разрешены только: {string.Join(", ", AllowedExtensions)}"
                    )
                    .When(x => x.Image is not null && !string.IsNullOrWhiteSpace(x.Image.FileName));
            }
        );
    }
}
