using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Groups.Features.Directories.Subjects.Update;

internal sealed class UpdateSubjectValidator : AbstractValidator<UpdateSubjectCommand>
{
    public UpdateSubjectValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Идентификатор предмета обязателен.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название предмета обязательно.")
            .MinimumLength(OrganizationScopedLookup.MinNameLength)
            .WithMessage($"Название предмета должно содержать не менее {OrganizationScopedLookup.MinNameLength} символа.")
            .MaximumLength(OrganizationScopedLookup.MaxNameLength)
            .WithMessage($"Название предмета не может превышать {OrganizationScopedLookup.MaxNameLength} символов.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Код предмета обязателен.")
            .Matches(@"^[A-Z0-9]{1,10}$")
            .WithMessage(
                "Код предмета должен содержать только заглавные латинские буквы и цифры, и не превышать 10 символов."
            );

        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("Цвет предмета обязателен.")
            .Matches(@"^#[0-9A-Fa-f]{6}$")
            .WithMessage("Цвет предмета должен быть в формате #RRGGBB (напр. #6366F1).");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Описание предмета не может превышать 500 символов.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Order).GreaterThanOrEqualTo(0).WithMessage("Порядок сортировки не может быть отрицательным.");
    }
}
