namespace Edvantix.Groups.Features.Levels.Update;

internal sealed class UpdateLevelValidator : AbstractValidator<UpdateLevelCommand>
{
    public UpdateLevelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

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
