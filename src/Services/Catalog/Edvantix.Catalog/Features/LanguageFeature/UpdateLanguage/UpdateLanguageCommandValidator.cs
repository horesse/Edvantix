namespace Edvantix.Catalog.Features.LanguageFeature.UpdateLanguage;

/// <summary>
/// Валидатор команды обновления языка.
/// </summary>
internal sealed class UpdateLanguageCommandValidator : AbstractValidator<UpdateLanguageCommand>
{
    public UpdateLanguageCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Код языка обязателен.");

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
