namespace Edvantix.Persona.Features.Profiles.UpdateAvatar;

public sealed class UpdateAvatarValidator : AbstractValidator<UpdateAvatarCommand>
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

    public UpdateAvatarValidator()
    {
        RuleFor(x => x.Avatar).NotNull().WithMessage("Файл аватара не передан.");

        RuleFor(x => x.Avatar.Length)
            .LessThanOrEqualTo(MaxFileSizeInBytes)
            .WithMessage($"Размер файла не должен превышать {MaxFileSizeInBytes / 1024 / 1024} МБ");

        RuleFor(x => x.Avatar.ContentType)
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage(
                $"Недопустимый тип файла. Разрешены: {string.Join(", ", AllowedContentTypes)}"
            );

        RuleFor(x => x.Avatar.FileName)
            .Must(name => AllowedExtensions.Contains(Path.GetExtension(name).ToLowerInvariant()))
            .WithMessage(
                $"Недопустимое расширение. Разрешены: {string.Join(", ", AllowedExtensions)}"
            );
    }
}
