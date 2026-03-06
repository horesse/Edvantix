namespace Edvantix.Catalog.Features.LanguageFeature.CreateLanguage;

/// <summary>
/// Валидатор команды создания языка.
/// </summary>
internal sealed class CreateLanguageCommandValidator : AbstractValidator<CreateLanguageCommand>
{
    public CreateLanguageCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Код языка обязателен.")
            .Length(2)
            .WithMessage("Код языка должен состоять из 2 символов (ISO 639-1).")
            .Matches(@"^[A-Za-z]{2}$")
            .WithMessage("Код языка должен содержать только буквы (ISO 639-1).");

        RuleFor(x => x.NameRu)
            .NotEmpty()
            .WithMessage("Наименование на русском обязательно.")
            .MaximumLength(100)
            .WithMessage("Наименование на русском не должно превышать 100 символов.");

        RuleFor(x => x.NameEn)
            .NotEmpty()
            .WithMessage("Наименование на английском обязательно.")
            .MaximumLength(100)
            .WithMessage("Наименование на английском не должно превышать 100 символов.");

        RuleFor(x => x.NativeName)
            .NotEmpty()
            .WithMessage("Название на родном языке обязательно.")
            .MaximumLength(100)
            .WithMessage("Название на родном языке не должно превышать 100 символов.");
    }
}
