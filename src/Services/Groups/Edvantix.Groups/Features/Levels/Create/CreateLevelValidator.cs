namespace Edvantix.Groups.Features.Levels.Create;

internal sealed class CreateLevelValidator : AbstractValidator<CreateLevelCommand>
{
    public CreateLevelValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Код уровня обязателен.")
            .Matches(@"^[A-Z0-9_-]{1,16}$")
            .WithMessage(
                "Код уровня должен содержать только заглавные латинские буквы, цифры, дефисы и подчёркивания, и не превышать 16 символов."
            );

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название уровня обязательно.")
            .MaximumLength(64)
            .WithMessage("Название уровня не может превышать 64 символа.");

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .WithMessage("Описание уровня не может превышать 256 символов.")
            .When(x => x.Description is not null);
    }
}
