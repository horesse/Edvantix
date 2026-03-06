namespace Edvantix.Catalog.Features.CurrencyFeature.UpdateCurrency;

/// <summary>
/// Валидатор команды обновления валюты.
/// </summary>
internal sealed class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>
{
    public UpdateCurrencyCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Код валюты обязателен.");

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

        RuleFor(x => x.Symbol)
            .NotEmpty()
            .WithMessage("Символ валюты обязателен.")
            .MaximumLength(10)
            .WithMessage("Символ валюты не должен превышать 10 символов.");
    }
}
