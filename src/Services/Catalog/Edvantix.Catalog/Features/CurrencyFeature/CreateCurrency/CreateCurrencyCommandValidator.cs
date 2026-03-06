namespace Edvantix.Catalog.Features.CurrencyFeature.CreateCurrency;

/// <summary>
/// Валидатор команды создания валюты.
/// </summary>
internal sealed class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Код валюты обязателен.")
            .Length(3)
            .WithMessage("Код валюты должен состоять из 3 символов (ISO 4217).")
            .Matches(@"^[A-Za-z]{3}$")
            .WithMessage("Код валюты должен содержать только буквы (ISO 4217).");

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

        RuleFor(x => x.NumericCode)
            .InclusiveBetween(1, 999)
            .WithMessage("Числовой код ISO 4217 должен быть в диапазоне 1–999.");

        RuleFor(x => x.DecimalDigits)
            .InclusiveBetween(0, 4)
            .WithMessage("Количество десятичных знаков должно быть в диапазоне 0–4.");
    }
}
