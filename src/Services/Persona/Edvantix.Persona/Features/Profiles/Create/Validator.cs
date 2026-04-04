namespace Edvantix.Persona.Features.Profiles.Create;

public sealed class Validator : AbstractValidator<RegistrationCommand>
{
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

        When(x => x.Avatar is not null, () => RuleFor(x => x.Avatar!).ApplyImageRules());
    }
}
