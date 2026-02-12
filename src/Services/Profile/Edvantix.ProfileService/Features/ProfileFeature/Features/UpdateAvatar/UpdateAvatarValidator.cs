using FluentValidation;

namespace Edvantix.ProfileService.Features.ProfileFeature.Features.UpdateAvatar;

/// <summary>
/// Валидатор команды обновления аватара
/// </summary>
public sealed class UpdateAvatarValidator : AbstractValidator<UpdateAvatarCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp",
    ];

    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public UpdateAvatarValidator()
    {
        RuleFor(x => x.Image).NotNull().WithMessage("Изображение является обязательным");

        RuleFor(x => x.Image.Length)
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .When(x => x.Image is not null)
            .WithMessage("Размер файла не должен превышать 5 МБ");

        RuleFor(x => x.Image.ContentType)
            .Must(ct => AllowedContentTypes.Contains(ct))
            .When(x => x.Image is not null)
            .WithMessage("Допустимые форматы: JPEG, PNG, GIF, WebP");
    }
}
