using Edvantix.Constants.Core;
using FluentValidation;

namespace Edvantix.ProfileService.Features.ProfileFeature.Registration;

public sealed class Validator : AbstractValidator<RegistrationCommand>
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

    public Validator()
    {
        RuleFor(x => x.Gender).IsInEnum().WithMessage("Указан некорректный пол");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Имя является обязательным полем")
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Имя не должно превышать {DataSchemaLength.Large} символов");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Фамилия является обязательным полем")
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Фамилия не должна превышать {DataSchemaLength.Large} символов");

        RuleFor(x => x.MiddleName)
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Отчество не должно превышать {DataSchemaLength.Large} символов");

        When(
            x => x.Avatar is not null,
            () =>
            {
                RuleFor(x => x.Avatar!.Length)
                    .LessThanOrEqualTo(MaxFileSizeInBytes)
                    .WithMessage(
                        $"Размер файла не должен превышать {MaxFileSizeInBytes / 1024 / 1024} МБ"
                    );

                RuleFor(x => x.Avatar!.ContentType)
                    .Must(contentType => AllowedContentTypes.Contains(contentType))
                    .WithMessage(
                        $"Недопустимый тип файла. Разрешены только: {string.Join(", ", AllowedContentTypes)}"
                    );

                RuleFor(x => x.Avatar!.FileName)
                    .Must(fileName =>
                    {
                        var extension = Path.GetExtension(fileName).ToLowerInvariant();
                        return AllowedExtensions.Contains(extension);
                    })
                    .WithMessage(
                        $"Недопустимое расширение файла. Разрешены только: {string.Join(", ", AllowedExtensions)}"
                    )
                    .When(x =>
                        x.Avatar is not null && !string.IsNullOrWhiteSpace(x.Avatar.FileName)
                    );
            }
        );
    }
}
