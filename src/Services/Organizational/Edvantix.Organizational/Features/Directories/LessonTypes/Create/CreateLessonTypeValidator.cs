using Edvantix.Organizational.Domain.LessonTypeAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Features.Directories.LessonTypes.Create;

internal sealed class CreateLessonTypeValidator : AbstractValidator<CreateLessonTypeCommand>
{
    public CreateLessonTypeValidator(ILessonTypeUniqueChecker uniqueChecker)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Имя типа занятия обязательно.")
            .Length(OrganizationScopedLookup.MinNameLength, OrganizationScopedLookup.MaxNameLength)
            .WithMessage(
                $"Имя должно содержать от {OrganizationScopedLookup.MinNameLength} до {OrganizationScopedLookup.MaxNameLength} символов."
            );

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Код типа занятия обязателен.")
            .Matches(@"^[A-Z0-9_-]{1,20}$")
            .WithMessage(
                "Код должен содержать только заглавные латинские буквы, цифры, дефисы и подчёркивания, и не превышать 20 символов."
            );

        RuleFor(x => x.DefaultDurationMinutes)
            .InclusiveBetween(LessonType.MinDurationMinutes, LessonType.MaxDurationMinutes)
            .WithMessage(
                $"Длительность должна быть от {LessonType.MinDurationMinutes} до {LessonType.MaxDurationMinutes} минут."
            );

        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("Цвет типа занятия обязателен.")
            .Matches(@"^#[0-9A-Fa-f]{6}$")
            .WithMessage("Цвет должен быть в формате HEX (#RRGGBB).");

        RuleFor(x => x.Icon)
            .MaximumLength(LessonType.MaxIconLength)
            .WithMessage($"Имя иконки не может превышать {LessonType.MaxIconLength} символов.")
            .When(x => x.Icon is not null);

        RuleFor(x => x)
            .MustAsync(
                async (cmd, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(cmd.Name) || cmd.OrganizationId == Guid.Empty)
                        return true;

                    return !await uniqueChecker.NameExistsAsync(
                        cmd.OrganizationId,
                        cmd.Name.Trim(),
                        excludeId: null,
                        ct
                    );
                }
            )
            .WithName(nameof(CreateLessonTypeCommand.Name))
            .WithMessage("Тип занятия с таким именем уже существует в справочнике.");

        RuleFor(x => x)
            .MustAsync(
                async (cmd, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(cmd.Code) || cmd.OrganizationId == Guid.Empty)
                        return true;

                    return !await uniqueChecker.CodeExistsAsync(
                        cmd.OrganizationId,
                        cmd.Code.Trim().ToUpperInvariant(),
                        excludeId: null,
                        ct
                    );
                }
            )
            .WithName(nameof(CreateLessonTypeCommand.Code))
            .WithMessage("Тип занятия с таким кодом уже существует в справочнике.");
    }
}
